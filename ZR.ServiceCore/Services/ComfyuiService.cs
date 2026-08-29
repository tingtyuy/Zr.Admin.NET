using System.Net;
using System.Net.Http;
using System.Text;
using Infrastructure.Attribute;
using Infrastructure.Extensions;
using Infrastructure.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using ZR.Common;
using ZR.Model;
using ZR.Model.System;
using ZR.Model.System.Dto;
using ZR.Repository;

namespace ZR.ServiceCore.Services
{
    /// <summary>
    /// ComfyUI服务
    /// </summary>
    [AppService(ServiceType = typeof(IComfyuiService), ServiceLifetime = LifeTime.Transient)]
    public class ComfyuiService : BaseService<ComfyuiWorkflow>, IComfyuiService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly OptionsSetting _optionsSetting;
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 功能类型前缀
        /// </summary>
        private static readonly Dictionary<string, string> FuncNameMap = new Dictionary<string, string>
        {
            { "txt2img", "文生图" },
            { "img2img", "图生图" },
            { "txt2video", "文生视频" },
            { "img2video", "图生视频" }
        };

        /// <summary>
        /// ComfyUI参考文件子目录类型
        /// </summary>
        private const int REF_TYPE_IMAGE = 1;
        private const int REF_TYPE_VIDEO = 2;

        public ComfyuiService(IWebHostEnvironment webHostEnvironment, IOptions<OptionsSetting> options)
        {
            _webHostEnvironment = webHostEnvironment;
            _optionsSetting = options.Value;
        }

        #region 配置
        /// <summary>
        /// 获取ComfyUI服务端地址（优先取数据库配置，其次appsettings.json，最后默认值）
        /// </summary>
        public string GetServerUrl()
        {
            string url = GetConfig("ServerUrl");
            if (string.IsNullOrWhiteSpace(url))
            {
                url = "http://127.0.0.1:8188";
            }
            return url.TrimEnd('/');
        }

        /// <summary>
        /// 保存ComfyUI服务端地址
        /// </summary>
        public bool SaveServerUrl(string serverUrl)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                throw new CustomException("ComfyUI服务地址不能为空");
            }
            return SaveConfig("ServerUrl", serverUrl.Trim().TrimEnd('/'));
        }

        /// <summary>
        /// 测试ComfyUI服务连通性（调用/session 或 /system 接口）
        /// </summary>
        public bool TestConnection(string serverUrl, out string message)
        {
            message = "";
            if (string.IsNullOrWhiteSpace(serverUrl))
            {
                message = "请输入ComfyUI服务地址";
                return false;
            }
            string url = serverUrl.Trim().TrimEnd('/');
            try
            {
                using (var http = new HttpClient())
                {
                    http.Timeout = TimeSpan.FromSeconds(8);
                    var response = http.GetAsync(url + "/system_stats").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        message = "连接成功，ComfyUI服务可用";
                        return true;
                    }
                    message = $"连接失败，HTTP {(int)response.StatusCode}";
                    return false;
                }
            }
            catch (Exception ex)
            {
                message = "连接失败: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取运行配置（key），无则回退appsettings
        /// </summary>
        private string GetConfig(string key)
        {
            try
            {
                var row = Context.Queryable<ComfyuiConfig>()
                    .Where(x => x.ConfigKey == key)
                    .OrderBy(x => x.Id, OrderByType.Desc)
                    .First();
                if (row != null && !string.IsNullOrWhiteSpace(row.ConfigValue))
                {
                    return row.ConfigValue;
                }
            }
            catch { }
            return AppSettings.GetConfig($"ComfyUI:{key}");
        }

        /// <summary>
        /// 保存运行配置（key-value，upsert）
        /// </summary>
        private bool SaveConfig(string key, string value)
        {
            var row = Context.Queryable<ComfyuiConfig>()
                .Where(x => x.ConfigKey == key)
                .OrderBy(x => x.Id, OrderByType.Desc)
                .First();
            if (row != null)
            {
                return Context.Updateable<ComfyuiConfig>()
                    .SetColumns(x => x.ConfigValue == value)
                    .Where(x => x.Id == row.Id)
                    .ExecuteCommand() > 0;
            }
            Context.Insertable(new ComfyuiConfig
            {
                Id = SnowFlakeSingle.Instance.NextId(),
                ConfigKey = key,
                ConfigValue = value,
                Create_time = DateTime.Now,
                Create_by = "system"
            }).ExecuteCommand();
            return true;
        }

        /// <summary>
        /// 超时时间（秒）
        /// </summary>
        public int GetTimeoutSeconds()
        {
            int seconds = 1800;
            string cfg = GetConfig("TimeoutSeconds");
            int.TryParse(cfg, out seconds);
            return seconds <= 0 ? 1800 : seconds;
        }
        #endregion

        #region 工作流
        public int BatchImportWorkflows(ComfyuiWorkflowBatchImportDto dto, long userId)
        {
            if (dto?.Workflows == null || dto.Workflows.Count == 0)
            {
                throw new CustomException("工作流列表不能为空");
            }

            int count = 0;
            foreach (var w in dto.Workflows)
            {
                ValidateApiFormat(w.WorkflowJson);
                var workflow = new ComfyuiWorkflow
                {
                    Id = SnowFlakeSingle.Instance.NextId(),
                    Name = w.Name,
                    Description = w.Description,
                    Category = string.IsNullOrWhiteSpace(w.Category) ? "default" : w.Category,
                    WorkflowJson = w.WorkflowJson,
                    VariableNodes = w.VariableNodes,
                    NodeCount = CountNodes(w.WorkflowJson),
                    Tags = w.Tags,
                    Status = "0",
                    Create_time = DateTime.Now,
                    Create_by = userId.ToString()
                };
                Insertable(workflow).ExecuteCommand();
                count++;
            }
            return count;
        }

        /// <summary>
        /// 校验工作流JSON必须为ComfyUI API格式（顶层对象属性=节点，节点须含class_type与inputs）
        /// </summary>
        private void ValidateApiFormat(string workflowJson)
        {
            if (string.IsNullOrWhiteSpace(workflowJson))
            {
                throw new CustomException("工作流JSON不能为空");
            }
            JObject root;
            try
            {
                root = JObject.Parse(workflowJson);
            }
            catch
            {
                throw new CustomException("工作流JSON格式无效，无法解析");
            }

            // UI/编辑器格式的典型特征
            if (root["nodes"] is JArray)
            {
                throw new CustomException("检测到ComfyUI UI/编辑器格式（含nodes数组），请使用【导出(API)】即“Export (API)”导出的JSON。仅支持API格式。");
            }

            if (root.Properties().Count() == 0)
            {
                throw new CustomException("工作流JSON为空，未包含任何节点");
            }

            // 每个顶层节点属性都应是对象且含 class_type / inputs
            foreach (var prop in root.Properties())
            {
                if (prop.Value.Type != JTokenType.Object)
                {
                    throw new CustomException($"节点 [{prop.Name}] 不是对象，不符合API格式");
                }
                var node = (JObject)prop.Value;
                if (string.IsNullOrWhiteSpace(node.Value<string>("class_type")))
                {
                    throw new CustomException($"节点 [{prop.Name}] 缺少 class_type，不符合API格式。UI格式导出的JSON需改用【导出(API)】重新导出。");
                }
            }
        }

        public PagedInfo<ComfyuiWorkflow> GetWorkflowList(ComfyuiWorkflowListDto parm, long userId)
        {
            var predicate = Expressionable.Create<ComfyuiWorkflow>();
            predicate.AndIF(parm.Name.IsNotEmpty(), x => x.Name.Contains(parm.Name));
            predicate.AndIF(parm.Category.IsNotEmpty(), x => x.Category == parm.Category);
            predicate.AndIF(parm.Tag.IsNotEmpty(), x => x.Tags != null && x.Tags.Contains(parm.Tag));

            return Queryable()
                .Where(predicate.ToExpression())
                .OrderBy(x => x.Create_time, OrderByType.Desc)
                .ToPage(parm);
        }

        public ComfyuiWorkflow GetWorkflowDetail(long id, long userId)
        {
            return Queryable().First(x => x.Id == id);
        }

        public bool SetWorkflowCategory(ComfyuiWorkflowCategoryDto dto, long userId)
        {
            if (dto?.Ids == null || dto.Ids.Count == 0)
            {
                throw new CustomException("请选择工作流");
            }
            var result = Context.Updateable<ComfyuiWorkflow>()
                .SetColumns(x => x.Category == dto.Category)
                .Where(x => dto.Ids.Contains(x.Id))
                .ExecuteCommand();
            return result > 0;
        }

        public bool DeleteWorkflow(long id, long userId)
        {
            return Context.Deleteable<ComfyuiWorkflow>()
                .Where(x => x.Id == id)
                .ExecuteCommand() > 0;
        }

        public bool UpdateWorkflowVariables(long id, string variableNodes, long userId)
        {
            return Context.Updateable<ComfyuiWorkflow>()
                .SetColumns(x => x.VariableNodes == variableNodes)
                .Where(x => x.Id == id)
                .ExecuteCommand() > 0;
        }

        public bool UpdateWorkflow(long id, ComfyuiWorkflowImportDto dto, long userId)
        {
            if (dto == null)
            {
                throw new CustomException("更新内容不能为空");
            }
            var workflow = Queryable().First(x => x.Id == id);
            if (workflow == null)
            {
                throw new CustomException("工作流不存在");
            }

            var updater = Context.Updateable<ComfyuiWorkflow>()
                .Where(x => x.Id == id);
            updater = updater.SetColumns(x => x.Name == dto.Name);
            updater = updater.SetColumns(x => x.Description == (dto.Description ?? ""));
            updater = updater.SetColumns(x => x.Category == (string.IsNullOrWhiteSpace(dto.Category) ? "default" : dto.Category));
            updater = updater.SetColumns(x => x.Tags == (dto.Tags ?? ""));
            if (dto.VariableNodes != null)
            {
                updater = updater.SetColumns(x => x.VariableNodes == dto.VariableNodes);
            }

            if (!string.IsNullOrWhiteSpace(dto.WorkflowJson))
            {
                ValidateApiFormat(dto.WorkflowJson);
                updater = updater
                    .SetColumns(x => x.WorkflowJson == dto.WorkflowJson)
                    .SetColumns(x => x.NodeCount == CountNodes(dto.WorkflowJson));
            }

            return updater.ExecuteCommand() > 0;
        }

        public object GetWorkflowVariableNodes(long workflowId, long userId)
        {
            var workflow = Queryable().First(x => x.Id == workflowId);
            if (workflow == null)
            {
                throw new CustomException("工作流不存在");
            }
            if (string.IsNullOrWhiteSpace(workflow.VariableNodes))
            {
                return new List<ComfyuiVariableNodeDto>();
            }
            try
            {
                return JsonConvert.DeserializeObject<List<ComfyuiVariableNodeDto>>(workflow.VariableNodes);
            }
            catch
            {
                return new List<ComfyuiVariableNodeDto>();
            }
        }

        /// <summary>
        /// 解析工作流JSON，筛查出可编辑节点（带描述），供前端选择可变节点
        /// </summary>
        public List<ComfyuiEditableNodeDto> GetEditableNodes(long workflowId, long userId)
        {
            var workflow = Queryable().First(x => x.Id == workflowId);
            if (workflow == null)
            {
                throw new CustomException("工作流不存在");
            }
            return ParseEditableNodes(workflow.WorkflowJson);
        }

        /// <summary>
        /// 筛查可编辑节点核心逻辑（纯函数）
        /// 支持两种工作流格式：
        ///  1) ComfyUI UI/编辑器格式: { id, revision, nodes:[{id,type,title,inputs:[{name,type,widget}],widgets_values:[...]}] }
        ///  2) ComfyUI API格式: { nodeId: { class_type, inputs, _meta, <widget字段> } }
        /// 且兼容 JSON 被二次编码（字符串套字符串）的情况。
        /// </summary>
        private List<ComfyuiEditableNodeDto> ParseEditableNodes(string workflowJson)
        {
            var result = new List<ComfyuiEditableNodeDto>();
            JToken token = UnwrapJson(workflowJson);
            if (token == null)
            {
                return result;
            }

            if (token is JObject root)
            {
                // UI/编辑器格式
                if (root["nodes"] is JArray uiNodes)
                {
                    return ParseUiNodes(uiNodes);
                }
                return ParseApiNodes(root);
            }
            return result;
        }

        /// <summary>
        /// 兼容JSON被二次编码：若解析出来是字符串，则再解析一次
        /// </summary>
        private JToken UnwrapJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            try
            {
                var token = JToken.Parse(json);
                int guard = 0;
                while (token is JValue value && value.Value is string inner && guard < 3)
                {
                    token = JToken.Parse(inner);
                    guard++;
                }
                return token;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 解析 ComfyUI UI/编辑器格式
        /// </summary>
        private List<ComfyuiEditableNodeDto> ParseUiNodes(JArray uiNodes)
        {
            var result = new List<ComfyuiEditableNodeDto>();
            foreach (var node in uiNodes)
            {
                if (node.Type != JTokenType.Object)
                {
                    continue;
                }
                var n = (JObject)node;
                string nodeId = n.Value<string>("id") ?? "";
                string classType = n.Value<string>("type") ?? "";
                string title = n.Value<string>("title");

                // 可编辑字段 = 带 widget 的输入
                var fields = new List<ComfyuiEditableFieldDto>();
                var widgetInputs = new List<(string Name, string WType)>();
                if (n["inputs"] is JArray inputs)
                {
                    foreach (var inp in inputs)
                    {
                        if (inp?.Type != JTokenType.Object)
                        {
                            continue;
                        }
                        var i = (JObject)inp;
                        if (i["widget"] == null || i["widget"].Type == JTokenType.Null)
                        {
                            continue;
                        }
                        string name = i.Value<string>("name") ?? "";
                        string wtype = i.Value<string>("type") ?? "STRING";
                        widgetInputs.Add((name, wtype));
                    }
                }

                // 对齐 widgets_values（best effort：值数量匹配时才有意义）
                var values = new List<string>();
                if (n["widgets_values"] is JArray wvals)
                {
                    foreach (var v in wvals)
                    {
                        values.Add(v.Type == JTokenType.Array ? string.Join(",", v.ToObject<object[]>()) : v.ToString());
                    }
                }

                var meta = GetNodeTypeMeta(classType);
                for (int idx = 0; idx < widgetInputs.Count; idx++)
                {
                    var (name, wtype) = widgetInputs[idx];
                    bool isNumber = wtype == "INT" || wtype == "FLOAT";
                    string fieldVal = idx < values.Count ? values[idx] : "";
                    fields.Add(new ComfyuiEditableFieldDto
                    {
                        Field = name,
                        FieldType = isNumber ? "number" : "text",
                        Type = isNumber ? "value" : (meta.Type == "prompt" && (name.Contains("text") || name.Contains("prompt")) ? "prompt" : "value"),
                        CurrentValue = fieldVal,
                        Description = isNumber ? "数值" : (meta.Type == "image" ? "图片" : meta.Type == "video" ? "视频" : "文本")
                    });
                }

                // 图片/视频加载节点即使没有widget文本也算可编辑（需上传文件）
                if (meta.Type == "image" || meta.Type == "video")
                {
                    if (fields.Count == 0)
                    {
                        fields.Add(new ComfyuiEditableFieldDto
                        {
                            Field = meta.Type == "image" ? "image" : "video",
                            FieldType = "text",
                            Type = meta.Type,
                            CurrentValue = "",
                            Description = meta.Type == "image" ? "图片" : "视频"
                        });
                    }
                    result.Add(new ComfyuiEditableNodeDto
                    {
                        NodeId = nodeId,
                        ClassType = classType,
                        Title = string.IsNullOrWhiteSpace(title) ? $"{classType} (#{nodeId})" : $"{title} (#{nodeId})",
                        Type = meta.Type,
                        Fields = fields,
                        Description = meta.Description
                    });
                    continue;
                }

                // 纯说明性节点（无widget输入）跳过
                if (fields.Count == 0)
                {
                    continue;
                }

                result.Add(new ComfyuiEditableNodeDto
                {
                    NodeId = nodeId,
                    ClassType = classType,
                    Title = string.IsNullOrWhiteSpace(title) ? $"{classType} (#{nodeId})" : $"{title} (#{nodeId})",
                    Type = meta.Type,
                    Fields = fields,
                    Description = meta.Description
                });
            }
            return result;
        }

        /// <summary>
        /// 解析 ComfyUI API格式
        /// 可编辑字段判定：inputs 中值为标量（非数组=非节点连线）的即直接可修改的widget值；
        /// 兼容部分导出把 widget 值放在节点根级别（非 class_type/inputs/_meta）的情况。
        /// </summary>
        private List<ComfyuiEditableNodeDto> ParseApiNodes(JObject root)
        {
            var result = new List<ComfyuiEditableNodeDto>();
            foreach (var prop in root.Properties())
            {
                string nodeId = prop.Name;
                if (prop.Value.Type != JTokenType.Object)
                {
                    continue;
                }
                var node = (JObject)prop.Value;
                string classType = node.Value<string>("class_type") ?? "";
                string title = node["_meta"]?["title"]?.ToString();

                var meta = GetNodeTypeMeta(classType);
                var fields = new List<ComfyuiEditableFieldDto>();

                // 1) inputs 中标量的字段（数组=节点连线，不可直接改）
                if (node["inputs"] is JObject inputs)
                {
                    foreach (var p in inputs.Properties())
                    {
                        if (p.Value.Type == JTokenType.Array || p.Value.Type == JTokenType.Object)
                        {
                            continue;
                        }
                        fields.Add(BuildEditableField(p.Name, p.Value));
                    }
                }

                // 2) 根级别 widget 字段（部分导出格式，如CLIPTextEncode的text在根级）
                foreach (var p in node.Properties())
                {
                    if (p.Name == "class_type" || p.Name == "inputs" || p.Name == "_meta")
                    {
                        continue;
                    }
                    if (fields.Any(f => f.Field == p.Name))
                    {
                        continue;
                    }
                    fields.Add(BuildEditableField(p.Name, p.Value));
                }

                // 图片/视频加载节点：即使没有标量widget也算可编辑（需上传文件）
                if (meta.Type == "image" || meta.Type == "video")
                {
                    if (fields.Count == 0)
                    {
                        fields.Add(new ComfyuiEditableFieldDto
                        {
                            Field = meta.Type == "image" ? "image" : "video",
                            FieldType = "text",
                            Type = meta.Type,
                            CurrentValue = "",
                            Description = meta.Type == "image" ? "图片" : "视频"
                        });
                    }
                }

                if (fields.Count == 0)
                {
                    continue;
                }

                result.Add(new ComfyuiEditableNodeDto
                {
                    NodeId = nodeId,
                    ClassType = classType,
                    Title = string.IsNullOrWhiteSpace(title) ? $"{classType} (#{nodeId})" : $"{title} (#{nodeId})",
                    Type = meta.Type,
                    Fields = fields,
                    Description = meta.Description
                });
            }
            return result;
        }

        /// <summary>
        /// 由字段名+值构造可编辑字段
        /// </summary>
        private ComfyuiEditableFieldDto BuildEditableField(string name, JToken value)
        {
            bool isNumber = value != null && (value.Type == JTokenType.Integer || value.Type == JTokenType.Float);
            return new ComfyuiEditableFieldDto
            {
                Field = name,
                CurrentValue = value?.ToString(),
                FieldType = isNumber ? "number" : "text",
                Type = isNumber ? "value" : "prompt",
                Description = isNumber ? "数值" : "文本"
            };
        }

        /// <summary>
        /// 按 class_type 推断节点用途/类型/描述
        /// </summary>
        private (string Type, string Description) GetNodeTypeMeta(string classType)
        {
            string upper = (classType ?? "").ToUpperInvariant();
            if (upper.Contains("CLIPTEXTENCODE") || upper.Contains("CLIPTEXTENCODEJOIN") || upper.Contains("CLIPPOSENCODING") || upper.Contains("TEXTENCODE"))
            {
                return ("prompt", "文本编码（提示词）节点，可修改提示词文本");
            }
            if (upper.Contains("LOADIMAGE"))
            {
                return ("image", "加载图片节点，可替换参考图");
            }
            if (upper.Contains("LOADVIDEO") || upper.Contains("VHS_LOAD") || upper.Contains("LOADAUDIO") || upper.Contains("LOADVIDEOPATH"))
            {
                return ("video", "加载视频/音频节点，可替换参考素材");
            }
            if (upper.Contains("LATENTIMAGE") || upper.Contains("EMPTYLATENT") || classType.Contains("EmptySD3LatentImage"))
            {
                return ("value", "空白潜空间/尺寸节点，可修改宽高/数量");
            }
            if (upper.Contains("KSAMPLER"))
            {
                return ("value", "采样器节点，可修改种子/步数/CFG等参数");
            }
            return ("value", classType);
        }

        private int CountNodes(string workflowJson)
        {
            if (string.IsNullOrWhiteSpace(workflowJson))
            {
                return 0;
            }
            try
            {
                var token = new JObject();
                var j = UnwrapJson(workflowJson);
                if (j is JObject root)
                {
                    if (root["nodes"] is JArray arr)
                    {
                        return arr.Count;
                    }
                    token = root;
                }
                return token.Properties().Count();
            }
            catch
            {
                return 0;
            }
        }
        #endregion

        #region 任务
        public (List<long> taskNos, string validationError) CreateTask(ComfyuiTaskCreateDto dto, Dictionary<string, IFormFile> refs, long userId)
        {
            if (dto.WorkflowId <= 0)
            {
                throw new CustomException("请选择工作流");
            }
            if (string.IsNullOrWhiteSpace(dto.FuncType))
            {
                throw new CustomException("功能类型不能为空");
            }
            int taskCount = dto.TaskCount;
            if (taskCount < 1) taskCount = 1;
            if (taskCount > 20) taskCount = 20;

            var workflow = Context.Queryable<ComfyuiWorkflow>().First(x => x.Id == dto.WorkflowId);
            if (workflow == null)
            {
                throw new CustomException("工作流不存在");
            }

            // 保存参考文件到本机存储（入队时再上传到ComfyUI input目录），按nodeId映射
            var refFiles = new List<ComfyuiRefFile>();
            if (refs != null && refs.Count > 0)
            {
                string storageRoot = "storage/comfyui";
                string batchDir = Path.Combine(_webHostEnvironment.WebRootPath, storageRoot, "input", userId.ToString(), SnowFlakeSingle.Instance.NextId().ToString());
                foreach (var kv in refs)
                {
                    var file = kv.Value;
                    if (file == null || file.Length == 0) continue;
                    string nodeDir = Path.Combine(batchDir, kv.Key ?? "ref");
                    Directory.CreateDirectory(nodeDir);
                    string ext = Path.GetExtension(file.FileName);
                    string fname = "ref" + ext;
                    string path = Path.Combine(nodeDir, fname);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    bool isVideo = ext.Equals(".mp4", StringComparison.OrdinalIgnoreCase)
                                || ext.Equals(".webm", StringComparison.OrdinalIgnoreCase)
                                || ext.Equals(".mov", StringComparison.OrdinalIgnoreCase)
                                || ext.Equals(".avi", StringComparison.OrdinalIgnoreCase);
                    refFiles.Add(new ComfyuiRefFile
                    {
                        NodeId = kv.Key,
                        OriginalName = file.FileName,
                        LocalPath = path,
                        ComfyType = isVideo ? "video" : "input"
                    });
                }
            }

            // 校验可变节点是否完整（存在 image/video 可变节点但未上传对应文件时记录警告）
            var variables = ParseVariableValues(dto.VariableValues);
            string validationError = ValidateReferences(workflow.VariableNodes, refFiles, dto.FuncType);

            string funcName = FuncNameMap.TryGetValue(dto.FuncType, out var n) ? n : "ComfyUI";
            string refFilesJson = JsonConvert.SerializeObject(refFiles);
            var taskNos = new List<long>();
            for (int i = 0; i < taskCount; i++)
            {
                long taskNo = SnowFlakeSingle.Instance.NextId();
                var task = new ComfyuiTask
                {
                    Id = taskNo,
                    UserId = userId,
                    TaskName = $"{funcName}{DateTime.Now:yyyyMMddHHmmss}-{i + 1}",
                    FuncType = dto.FuncType,
                    WorkflowId = dto.WorkflowId,
                    WorkflowName = workflow.Name,
                    VariableValues = dto.VariableValues,
                    RefFiles = refFilesJson,
                    Queued = 0,
                    Status = "draft",
                    ErrorMessage = validationError,
                    CreatedTime = DateTime.Now,
                    Create_time = DateTime.Now,
                    Create_by = userId.ToString()
                };
                // 校验通过的任务创建后直接加入执行队列
                bool autoQueue = string.IsNullOrEmpty(validationError);
                if (autoQueue)
                {
                    task.Queued = 1;
                    task.Status = "pending";
                    task.QueuedTime = DateTime.Now;
                }
                Context.Insertable(task).ExecuteCommand();

                if (autoQueue)
                {
                    Context.Insertable(new ComfyuiQueue
                    {
                        Id = SnowFlakeSingle.Instance.NextId(),
                        TaskId = taskNo,
                        TaskName = task.TaskName,
                        FuncType = dto.FuncType,
                        WorkflowId = dto.WorkflowId,
                        Status = "pending",
                        Progress = 0,
                        UserId = userId,
                        QueuedTime = DateTime.Now,
                        Create_time = DateTime.Now,
                        Create_by = userId.ToString()
                    }).ExecuteCommand();
                }
                taskNos.Add(taskNo);
            }
            return (taskNos, validationError);
        }

        /// <summary>
        /// 更新任务明细（仅草稿可编辑；校验通过后自动入队）
        /// </summary>
        public (List<long> taskNos, string validationError) UpdateTask(long taskId, ComfyuiTaskCreateDto dto, Dictionary<string, IFormFile> refs, long userId)
        {
            var task = Context.Queryable<ComfyuiTask>().First(x => x.Id == taskId && x.UserId == userId);
            if (task == null)
            {
                throw new CustomException("任务不存在");
            }
            if (task.Queued == 1)
            {
                // 仅执行中的任务禁止编辑，其余（待执行/完成/失败/取消）均可再编辑
                var runningQueue = Context.Queryable<ComfyuiQueue>().First(x => x.TaskId == taskId && x.Status == "processing");
                if (runningQueue != null)
                {
                    throw new CustomException("任务正在执行中，不能编辑");
                }
            }
            var workflow = Context.Queryable<ComfyuiWorkflow>().First(x => x.Id == task.WorkflowId);
            if (workflow == null)
            {
                throw new CustomException("工作流不存在");
            }

            // 已有参考文件
            var refFiles = new List<ComfyuiRefFile>();
            if (task.RefFiles.IsNotEmpty())
            {
                try { refFiles = JsonConvert.DeserializeObject<List<ComfyuiRefFile>>(task.RefFiles) ?? new List<ComfyuiRefFile>(); }
                catch { refFiles = new List<ComfyuiRefFile>(); }
            }
            // 若上传了新参考文件，则替换对应节点的旧文件
            if (refs != null && refs.Count > 0)
            {
                string storageRoot = "storage/comfyui";
                string batchDir = Path.Combine(_webHostEnvironment.WebRootPath, storageRoot, "input", userId.ToString(), SnowFlakeSingle.Instance.NextId().ToString());
                var newRefFiles = new List<ComfyuiRefFile>();
                foreach (var kv in refs)
                {
                    var file = kv.Value;
                    if (file == null || file.Length == 0) continue;
                    string nodeDir = Path.Combine(batchDir, kv.Key ?? "ref");
                    Directory.CreateDirectory(nodeDir);
                    string ext = Path.GetExtension(file.FileName);
                    string fname = "ref" + ext;
                    string path = Path.Combine(nodeDir, fname);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    bool isVideo = ext.Equals(".mp4", StringComparison.OrdinalIgnoreCase)
                                || ext.Equals(".webm", StringComparison.OrdinalIgnoreCase)
                                || ext.Equals(".mov", StringComparison.OrdinalIgnoreCase)
                                || ext.Equals(".avi", StringComparison.OrdinalIgnoreCase);
                    newRefFiles.Add(new ComfyuiRefFile
                    {
                        NodeId = kv.Key,
                        OriginalName = file.FileName,
                        LocalPath = path,
                        ComfyType = isVideo ? "video" : "input"
                    });
                }
                // 替换旧文件（删除被替换节点的旧目录）
                foreach (var nf in newRefFiles)
                {
                    var old = refFiles.FirstOrDefault(r => r.NodeId == nf.NodeId);
                    if (old != null)
                    {
                        if (old.LocalPath.IsNotEmpty() && old.LocalPath != nf.LocalPath)
                        {
                            string oldDir = Path.GetDirectoryName(old.LocalPath);
                            if (!string.IsNullOrEmpty(oldDir) && Directory.Exists(oldDir))
                            {
                                try { Directory.Delete(oldDir, true); } catch { }
                            }
                        }
                        refFiles.Remove(old);
                    }
                    refFiles.Add(nf);
                }
            }

            // 校验可变节点完整性
            string validationError = ValidateReferences(workflow.VariableNodes, refFiles, task.FuncType);

            string refFilesJson = JsonConvert.SerializeObject(refFiles);
            var updater = Context.Updateable<ComfyuiTask>()
                .SetColumns(x => x.VariableValues == dto.VariableValues)
                .SetColumns(x => x.RefFiles == refFilesJson)
                .SetColumns(x => x.ErrorMessage == validationError);
            if (validationError == null)
            {
                updater = updater.SetColumns(x => x.Queued == 1)
                    .SetColumns(x => x.Status == "pending")
                    .SetColumns(x => x.QueuedTime == DateTime.Now);
            }
            updater.Where(x => x.Id == taskId).ExecuteCommand();

            // 校验通过则（重新）入队
            if (validationError == null)
            {
                // 若已有待执行队列则不重复新增，只刷新排队时间；否则新增一条队列任务
                var pendingQueue = Context.Queryable<ComfyuiQueue>().First(x => x.TaskId == taskId && x.Status == "pending");
                if (pendingQueue != null)
                {
                    Context.Updateable<ComfyuiQueue>()
                        .SetColumns(x => x.QueuedTime == DateTime.Now)
                        .Where(x => x.Id == pendingQueue.Id)
                        .ExecuteCommand();
                }
                else
                {
                    Context.Insertable(new ComfyuiQueue
                    {
                        Id = SnowFlakeSingle.Instance.NextId(),
                        TaskId = taskId,
                        TaskName = task.TaskName,
                        FuncType = task.FuncType,
                        WorkflowId = task.WorkflowId,
                        Status = "pending",
                        Progress = 0,
                        UserId = userId,
                        QueuedTime = DateTime.Now,
                        Create_time = DateTime.Now,
                        Create_by = userId.ToString()
                    }).ExecuteCommand();
                }
            }
            return (new List<long> { taskId }, validationError);
        }

        /// <summary>
        /// 校验 image/video 可变节点是否都有对应上传文件
        /// </summary>
        private string ValidateReferences(string variableNodesJson, List<ComfyuiRefFile> refFiles, string funcType)
        {
            var nodes = new List<ComfyuiVariableNodeDto>();
            if (variableNodesJson.IsNotEmpty())
            {
                try { nodes = JsonConvert.DeserializeObject<List<ComfyuiVariableNodeDto>>(variableNodesJson); }
                catch { nodes = new List<ComfyuiVariableNodeDto>(); }
            }
            if (nodes.Count == 0) return null;

            var uploadedNodeIds = refFiles.Where(r => r.LocalPath.IsNotEmpty() && File.Exists(r.LocalPath)).Select(r => r.NodeId).ToHashSet();
            var missing = new List<string>();
            foreach (var node in nodes)
            {
                if (node.Type == "image" || node.Type == "video")
                {
                    if (!uploadedNodeIds.Contains(node.NodeId))
                    {
                        missing.Add($"节点{node.NodeId}({node.Label ?? node.Field})");
                    }
                }
            }
            if (missing.Count > 0)
            {
                return $"缺少参考{(nodes.Any(n => n.Type == "video") ? "视频" : "图片")}：{string.Join("、", missing)}";
            }
            return null;
        }

        public PagedInfo<ComfyuiTaskView> GetTaskList(ComfyuiTaskListDto parm, long userId)
        {
            DateTime? dateStart = null, dateEnd = null;
            if (parm.Date.IsNotEmpty() && DateTime.TryParse(parm.Date, out var dateVal))
            {
                dateStart = dateVal.Date;
                dateEnd = dateVal.Date.AddDays(1);
            }

            var query = Context.Queryable<ComfyuiTask>()
                .WhereIF(parm.Status.IsNotEmpty(), x => x.Status == parm.Status)
                .WhereIF(parm.FuncType.IsNotEmpty(), x => x.FuncType == parm.FuncType)
                .WhereIF(parm.Queued.HasValue, x => x.Queued == parm.Queued.Value)
                .WhereIF(dateStart.HasValue, x => x.CreatedTime >= dateStart.Value && x.CreatedTime < dateEnd.Value)
                .WhereIF(parm.Prompt.IsNotEmpty(), x => (x.TaskName != null && x.TaskName.Contains(parm.Prompt)) || x.WorkflowName.Contains(parm.Prompt));

            int total = query.Count();
            var page = query.OrderBy(x => x.CreatedTime, OrderByType.Desc)
                .Skip((parm.PageNum - 1) * parm.PageSize)
                .Take(parm.PageSize)
                .ToList();

            // 关联执行队列信息与输出
            List<ComfyuiQueue> queues = new List<ComfyuiQueue>();
            if (page.Count > 0)
            {
                queues = Context.Queryable<ComfyuiQueue>()
                    .Where(x => page.Select(t => t.Id).Contains(x.TaskId))
                    .Select(q => new ComfyuiQueue { Id = q.Id, TaskId = q.TaskId, Status = q.Status, Progress = q.Progress, OutputUrls = q.OutputUrls, ErrorMessage = q.ErrorMessage })
                    .ToList()
                    // 同一任务可能有多条队列记录（再次入队），取最新一条
                    .OrderByDescending(q => q.Id)
                    .GroupBy(q => q.TaskId)
                    .Select(g => g.First())
                    .ToList();
            }

            var result = page.Select(t =>
            {
                var q = queues.FirstOrDefault(x => x.TaskId == t.Id);
                return new ComfyuiTaskView
                {
                    Id = t.Id,
                    TaskName = t.TaskName,
                    FuncType = t.FuncType,
                    WorkflowId = t.WorkflowId,
                    WorkflowName = t.WorkflowName,
                    Queued = t.Queued,
                    Status = t.Status,
                    ErrorMessage = t.ErrorMessage,
                    CreatedTime = t.CreatedTime,
                    QueuedTime = t.QueuedTime,
                    CompleteTime = t.CompleteTime,
                    Create_time = t.Create_time.ToString("yyyy-MM-dd HH:mm:ss"),
                    QueueId = q?.Id,
                    QueueStatus = q?.Status,
                    Progress = q?.Progress ?? 0,
                    OutputUrls = q?.OutputUrls,
                    QueueErrorMessage = q?.ErrorMessage
                };
            }).ToList();

            return new PagedInfo<ComfyuiTaskView>
            {
                TotalNum = total,
                PageSize = parm.PageSize,
                PageIndex = parm.PageNum,
                Result = result
            };
        }

        public ComfyuiTask GetTaskDetail(long id, long userId)
        {
            return Context.Queryable<ComfyuiTask>().First(x => x.Id == id);
        }

        public bool DeleteTask(long id, long userId)
        {
            var task = Context.Queryable<ComfyuiTask>().First(x => x.Id == id && x.UserId == userId);
            if (task == null)
            {
                throw new CustomException("任务不存在");
            }
            var runningQueue = Context.Queryable<ComfyuiQueue>().First(x => x.TaskId == id && x.Status == "processing");
            if (runningQueue != null)
            {
                throw new CustomException("任务正在执行中，不能删除");
            }
            // 清理该任务的所有历史队列记录（含待执行取消、已完成/失败记录）
            Context.Deleteable<ComfyuiQueue>().Where(x => x.TaskId == id).ExecuteCommand();
            // 清理本地参考文件和输出文件
            try
            {
                string storageRoot = "storage/comfyui";
                string outDir = Path.Combine(_webHostEnvironment.WebRootPath, storageRoot, "output", task.Id.ToString());
                if (Directory.Exists(outDir))
                {
                    Directory.Delete(outDir, true);
                }
                if (task.RefFiles.IsNotEmpty())
                {
                    var files = JsonConvert.DeserializeObject<List<ComfyuiRefFile>>(task.RefFiles) ?? new List<ComfyuiRefFile>();
                    foreach (var f in files.Where(f => f.LocalPath.IsNotEmpty()))
                    {
                        string dir = Path.GetDirectoryName(f.LocalPath);
                        if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                        {
                            try { Directory.Delete(dir, true); } catch { }
                        }
                    }
                }
            }
            catch { }
            return Context.Deleteable<ComfyuiTask>().Where(x => x.Id == id).ExecuteCommand() > 0;
        }

        public int EnqueueTasks(List<long> taskIds, long userId)
        {
            if (taskIds == null || taskIds.Count == 0)
            {
                throw new CustomException("请选择任务");
            }
            int count = 0;
            var tasks = Context.Queryable<ComfyuiTask>()
                .Where(x => taskIds.Contains(x.Id))
                .ToList();
            // 执行中的任务不可重复入队
            var runningIds = Context.Queryable<ComfyuiQueue>()
                .Where(x => taskIds.Contains(x.TaskId) && x.Status == "processing")
                .Select(x => x.TaskId)
                .ToList();
            foreach (var task in tasks.Where(t => !runningIds.Contains(t.Id)))
            {
                // 已在待执行队列中则跳过（不重复排队）
                var pendingExists = Context.Queryable<ComfyuiQueue>()
                    .Any(x => x.TaskId == task.Id && x.Status == "pending");
                if (pendingExists)
                {
                    continue;
                }
                // 更新任务为pending入队状态
                Context.Updateable<ComfyuiTask>()
                    .SetColumns(x => x.Queued == 1)
                    .SetColumns(x => x.Status == "pending")
                    .SetColumns(x => x.QueuedTime == DateTime.Now)
                    .Where(x => x.Id == task.Id)
                    .ExecuteCommand();

                // 新增一条队列记录（已完成/失败任务再次入队即为新队列任务）
                var queue = new ComfyuiQueue
                {
                    Id = SnowFlakeSingle.Instance.NextId(),
                    TaskId = task.Id,
                    TaskName = task.TaskName,
                    FuncType = task.FuncType,
                    WorkflowId = task.WorkflowId,
                    Status = "pending",
                    Progress = 0,
                    UserId = task.UserId,
                    QueuedTime = DateTime.Now,
                    Create_time = DateTime.Now,
                    Create_by = userId.ToString()
                };
                Context.Insertable(queue).ExecuteCommand();
                count++;
            }
            return count;
        }

        public int BatchDeleteTask(List<long> ids, long userId)
        {
            if (ids == null || ids.Count == 0)
            {
                return 0;
            }
            // 排除执行中的任务
            var runningIds = Context.Queryable<ComfyuiQueue>()
                .Where(x => ids.Contains(x.TaskId) && x.Status == "processing")
                .Select(x => x.TaskId)
                .ToList();
            var deletable = ids.Where(i => !runningIds.Contains(i)).ToList();
            if (deletable.Count == 0)
            {
                return 0;
            }
            // 清理关联队列记录
            Context.Deleteable<ComfyuiQueue>().Where(x => deletable.Contains(x.TaskId)).ExecuteCommand();
            return Context.Deleteable<ComfyuiTask>()
                .Where(x => deletable.Contains(x.Id))
                .ExecuteCommand();
        }

        /// <summary>
        /// 文本翻译：联网调用免费翻译接口，支持中英文互译
        /// </summary>
        public async Task<string> TranslateAsync(string text, string target)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new CustomException("请输入要翻译的内容");
            }
            text = text.Trim();
            if (target != "zh-CN" && target != "en")
            {
                target = "zh-CN";
            }
            // 已是目标语言则原文返回
            bool isChinese = text.Any(c => c >= 0x4E00 && c <= 0x9FFF);
            if ((target == "zh-CN" && isChinese) || (target == "en" && !isChinese))
            {
                return text;
            }
            string source = isChinese ? "zh-CN" : "en";
            string url = "https://api.mymemory.translated.net/get" +
                $"?q={Uri.EscapeDataString(text)}&langpair={source}|{target}";
            try
            {
                using var resp = await _httpClient.GetAsync(url);
                resp.EnsureSuccessStatusCode();
                string json = await resp.Content.ReadAsStringAsync();
                var jobj = JObject.Parse(json);
                string translated = jobj["responseData"]?["translatedText"]?.ToString();
                if (string.IsNullOrWhiteSpace(translated) || translated.Contains("MYMEMORY WARNING"))
                {
                    throw new CustomException("翻译失败，请稍后再试");
                }
                return WebUtility.HtmlDecode(translated);
            }
            catch (CustomException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "ComfyUI文本翻译失败");
                throw new CustomException("翻译服务暂时不可用，请稍后再试");
            }
        }
        #endregion

        #region 执行队列
        public PagedInfo<ComfyuiQueue> GetQueueList(ComfyuiQueueListDto parm, long userId)
        {
            DateTime? dateStart = null, dateEnd = null;
            if (parm.Date.IsNotEmpty() && DateTime.TryParse(parm.Date, out var dateVal))
            {
                dateStart = dateVal.Date;
                dateEnd = dateVal.Date.AddDays(1);
            }

            return Context.Queryable<ComfyuiQueue>()
                .WhereIF(parm.Status.IsNotEmpty(), x => x.Status == parm.Status)
                .WhereIF(parm.FuncType.IsNotEmpty(), x => x.FuncType == parm.FuncType)
                .WhereIF(dateStart.HasValue, x => x.QueuedTime >= dateStart.Value && x.QueuedTime < dateEnd.Value)
                .OrderBy(x => x.QueuedTime, OrderByType.Desc)
                .ToPage(parm);
        }

        public bool CancelQueue(long id, long userId)
        {
            var queue = Context.Queryable<ComfyuiQueue>().First(x => x.Id == id);
            if (queue == null)
            {
                throw new CustomException("队列记录不存在");
            }

            // 还原对应任务
            Context.Updateable<ComfyuiTask>()
                .SetColumns(x => x.Queued == 0)
                .SetColumns(x => x.Status == "draft")
                .Where(x => x.Id == queue.TaskId)
                .ExecuteCommand();

            return Context.Updateable<ComfyuiQueue>()
                .SetColumns(x => x.Status == "cancelled")
                .SetColumns(x => x.CompleteTime == DateTime.Now)
                .Where(x => x.Id == id)
                .ExecuteCommand() > 0;
        }

        public bool Dequeue(long id, long userId)
        {
            var queue = Context.Queryable<ComfyuiQueue>().First(x => x.Id == id);
            if (queue == null)
            {
                throw new CustomException("队列记录不存在");
            }
            // 还原任务，可重新编辑
            Context.Updateable<ComfyuiTask>()
                .SetColumns(x => x.Queued == 0)
                .SetColumns(x => x.Status == "draft")
                .Where(x => x.Id == queue.TaskId)
                .ExecuteCommand();

            return Context.Deleteable<ComfyuiQueue>().Where(x => x.Id == id).ExecuteCommand() > 0;
        }
        #endregion

        #region 内部工具
        private Dictionary<string, object> ParseVariableValues(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Dictionary<string, object>();
            }
            try
            {
                var jobj = JObject.Parse(json);
                var dict = new Dictionary<string, object>();
                foreach (var prop in jobj.Properties())
                {
                    dict[prop.Name] = prop.Value;
                }
                return dict;
            }
            catch
            {
                return new Dictionary<string, object>();
            }
        }

        private bool NeedReference(string variableNodesJson, string type)
        {
            if (string.IsNullOrWhiteSpace(variableNodesJson))
            {
                return false;
            }
            try
            {
                var nodes = JsonConvert.DeserializeObject<List<ComfyuiVariableNodeDto>>(variableNodesJson);
                return nodes.Any(n => n.Type == type);
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 供后台Worker调用的构建/执行逻辑
        /// <summary>
        /// 构建提交给ComfyUI的prompt请求体（替换可变节点值，参考文件上传到ComfyUI input）
        /// </summary>
        public string BuildPromptJson(ComfyuiTask task, ComfyuiWorkflow workflow)
        {
            JObject prompt = JObject.Parse(workflow.WorkflowJson);

            // 可变节点配置
            var nodes = new List<ComfyuiVariableNodeDto>();
            if (workflow.VariableNodes.IsNotEmpty())
            {
                try { nodes = JsonConvert.DeserializeObject<List<ComfyuiVariableNodeDto>>(workflow.VariableNodes); }
                catch { nodes = new List<ComfyuiVariableNodeDto>(); }
            }

            // 可变节点最终值
            var values = ParseVariableValues(task.VariableValues);
            var refFiles = ParseRefFiles(task.RefFiles);

            var handledNodeIds = new HashSet<string>();
            foreach (var node in nodes)
            {
                var nodeObj = prompt[node.NodeId] as JObject;
                if (nodeObj == null) continue;

                string value = values.ContainsKey(node.NodeId) ? values[node.NodeId]?.ToString() : null;

                switch (node.Type)
                {
                    case "prompt":
                    case "value":
                        if (!string.IsNullOrEmpty(value) && nodeObj["inputs"] is JObject inputs)
                        {
                            inputs[node.Field] = ConvertToNativeJToken(inputs[node.Field], value);
                        }
                        break;
                    case "bool":
                        if (!string.IsNullOrEmpty(value) && nodeObj["inputs"] is JObject boolInputs)
                        {
                            boolInputs[node.Field] = ConvertToNativeJToken(boolInputs[node.Field], value);
                        }
                        break;
                    case "image":
                    case "video":
                        if (nodeObj["inputs"] is JObject refInputs)
                        {
                            var refFile = refFiles.FirstOrDefault(r => r.NodeId == node.NodeId);
                            if (refFile != null && refFile.ComfyName.IsNotEmpty())
                            {
                                refInputs[node.Field] = refFile.ComfyName;
                            }
                        }
                        break;
                }
                handledNodeIds.Add(node.NodeId);
            }

            // fallback: VariableNodes 未配置的节点，尝试自动替换 inputs 中的 text 字段
            foreach (var kv in values)
            {
                if (handledNodeIds.Contains(kv.Key)) continue;
                string nodeId = kv.Key;
                string val = kv.Value?.ToString();
                if (string.IsNullOrEmpty(val)) continue;
                var nodeObj = prompt[nodeId] as JObject;
                if (nodeObj == null) continue;
                if (nodeObj["inputs"] is JObject inp && inp["text"] != null)
                {
                    inp["text"] = val;
                }
            }

            return prompt.ToString(Formatting.None);
        }

        private List<ComfyuiRefFile> ParseRefFiles(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<ComfyuiRefFile>();
            }
            try
            {
                var list = JsonConvert.DeserializeObject<List<ComfyuiRefFile>>(json);
                return list ?? new List<ComfyuiRefFile>();
            }
            catch
            {
                return new List<ComfyuiRefFile>();
            }
        }

        /// <summary>
        /// 按工作流原字段类型转换新值，避免数字/布尔字段被写成字符串导致ComfyUI校验失败
        /// </summary>
        private JToken ConvertToNativeJToken(JToken original, string value)
        {
            if (original == null || original.Type == JTokenType.Null)
            {
                return value;
            }
            switch (original.Type)
            {
                case JTokenType.Integer:
                    return long.TryParse(value, out var l) ? new JValue(l) : value;
                case JTokenType.Float:
                    return double.TryParse(value, out var d) ? new JValue(d) : value;
                case JTokenType.Boolean:
                    return bool.TryParse(value, out var b) ? new JValue(b) : value;
                default:
                    return value;
            }
        }

        /// <summary>
        /// 将所有参考文件上传到ComfyUI input目录，并按nodeId回填comfy文件名
        /// </summary>
        public void UploadReferenceToComfy(ComfyuiTask task)
        {
            var refFiles = ParseRefFiles(task.RefFiles);
            foreach (var rf in refFiles)
            {
                if (rf.LocalPath.IsEmpty() || !File.Exists(rf.LocalPath)) continue;

                string ext = Path.GetExtension(rf.LocalPath);
                string targetName = $"{task.Id}_{rf.NodeId}_{DateTime.Now:yyyyMMddHHmmss}{ext}";

                string endpoint = "/upload/image";
                try
                {
                    using (var form = new MultipartFormDataContent())
                    {
                        var fileContent = new ByteArrayContent(File.ReadAllBytes(rf.LocalPath));
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        form.Add(fileContent, "image", targetName);
                        form.Add(new StringContent("input"), "type");
                        form.Add(new StringContent("true"), "overwrite");

                        var response = _httpClient.PostAsync(GetServerUrl() + endpoint, form).Result;
                        response.EnsureSuccessStatusCode();
                        var json = response.Content.ReadAsStringAsync().Result;
                        var jobj = JObject.Parse(json);
                        rf.ComfyName = jobj["name"]?.ToString() ?? targetName;
                        rf.Subfolder = jobj["subfolder"]?.ToString() ?? "";
                        rf.ComfyType = jobj["type"]?.ToString() ?? "input";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex, $"上传参考文件到ComfyUI失败: nodeId={rf.NodeId}");
                    throw new CustomException($"参考文件上传到ComfyUI失败（节点 {rf.NodeId}）：{ex.Message}");
                }
            }

            // 回写最新ComfyName
            task.RefFiles = JsonConvert.SerializeObject(refFiles);
        }

        /// <summary>
        /// 提交prompt到ComfyUI，返回prompt_id
        /// </summary>
        public string SubmitToComfy(string promptJson)
        {
            var body = new
            {
                prompt = JObject.Parse(promptJson),
                client_id = "zr_admin_comfyui"
            };
            var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = _httpClient.PostAsync(GetServerUrl() + "/prompt", content).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new CustomException($"ComfyUI提交失败: {result}");
            }
            var jobj = JObject.Parse(result);
            if (jobj["error"] != null)
            {
                throw new CustomException($"ComfyUI错误: {jobj["error"]}");
            }
            return jobj["prompt_id"]?.ToString();
        }

        /// <summary>
        /// 查询ComfyUI执行历史，返回输出文件列表（image/video），输出文件会下载到本地存储
        /// </summary>
        public List<object> QueryHistory(string promptId, long taskId = 0)
        {
            var result = new List<object>();
            if (string.IsNullOrWhiteSpace(promptId)) return result;
            var response = _httpClient.GetAsync($"{GetServerUrl()}/history/{promptId}").Result;
            if (!response.IsSuccessStatusCode) return result;
            var json = response.Content.ReadAsStringAsync().Result;
            var jobj = JObject.Parse(json);
            var entry = jobj[promptId];
            if (entry == null) return result;
            var outputs = entry["outputs"] as JObject;
            if (outputs == null) return result;
            foreach (var outProp in outputs.Properties())
            {
                var images = outProp.Value["images"] as JArray;
                if (images != null)
                {
                    foreach (var img in images)
                    {
                        string filename = img["filename"]?.ToString();
                        string subfolder = img["subfolder"]?.ToString();
                        string type = img["type"]?.ToString();
                        if (filename.IsNotEmpty())
                        {
                            string url = $"{GetServerUrl()}/view?filename={filename}&subfolder={subfolder}&type={type}";
                            bool isVideo = filename.EndsWith(".mp4") || filename.EndsWith(".webm") || filename.EndsWith(".avi")
                                        || filename.EndsWith(".mov") || filename.EndsWith(".gif");
                            string localUrl = DownloadOutputToLocal(url, filename, taskId);
                            result.Add(new { url = localUrl, type = isVideo ? "video" : "image", filename = filename });
                        }
                    }
                }
                var gifs = outProp.Value["gifs"] as JArray;
                if (gifs != null)
                {
                    foreach (var g in gifs)
                    {
                        string filename = g["filename"]?.ToString();
                        string subfolder = g["subfolder"]?.ToString();
                        string type = g["type"]?.ToString();
                        if (filename.IsNotEmpty())
                        {
                            string url = $"{GetServerUrl()}/view?filename={filename}&subfolder={subfolder}&type={type}";
                            string localUrl = DownloadOutputToLocal(url, filename, taskId);
                            result.Add(new { url = localUrl, type = "video", filename = filename });
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 获取ComfyUI输出存储根目录
        /// </summary>
        private string GetStorageRoot()
        {
            var root = GetConfig("StorageRoot");
            return string.IsNullOrWhiteSpace(root) ? "storage/comfyui" : root;
        }

        /// <summary>
        /// 将ComfyUI输出文件下载到本地存储目录，返回本地访问URL；失败则回退远程URL
        /// </summary>
        private string DownloadOutputToLocal(string comfyUrl, string filename, long taskId)
        {
            if (string.IsNullOrWhiteSpace(comfyUrl) || taskId <= 0) return comfyUrl;
            try
            {
                using var response = _httpClient.GetAsync(comfyUrl).Result;
                if (!response.IsSuccessStatusCode) return comfyUrl;
                var bytes = response.Content.ReadAsByteArrayAsync().Result;
                if (bytes == null || bytes.Length == 0) return comfyUrl;

                string storageRoot = GetStorageRoot();
                string outputDir = Path.Combine(_webHostEnvironment.WebRootPath, storageRoot, "output", taskId.ToString());
                Directory.CreateDirectory(outputDir);

                string ext = Path.GetExtension(filename);
                string name = Path.GetFileNameWithoutExtension(filename);
                string targetName = $"{name}_{DateTime.Now:yyyyMMddHHmmssfff}{ext}";
                string filePath = Path.Combine(outputDir, targetName);
                File.WriteAllBytes(filePath, bytes);

                string accessUrl = string.Concat(
                    _optionsSetting.Upload.UploadUrl.TrimEnd('/'), "/",
                    storageRoot.Replace("\\", "/"), "/output/",
                    taskId, "/", targetName);
                logger.Info($"ComfyUI输出文件已下载到本地: taskId={taskId}, file={targetName}");
                return accessUrl;
            }
            catch (Exception ex)
            {
                logger.Warn(ex, $"下载ComfyUI输出文件到本地失败，回退远程URL: {comfyUrl}");
                return comfyUrl;
            }
        }
        #endregion
    }
}
