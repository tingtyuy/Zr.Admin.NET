using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using ZR.Model;
using ZR.Model.System;
using ZR.Model.System.Dto;

namespace ZR.ServiceCore.Services
{
    /// <summary>
    /// ComfyUI服务接口
    /// </summary>
    public interface IComfyuiService : IBaseService<ComfyuiWorkflow>
    {
        /// <summary>
        /// 获取ComfyUI服务端地址（运行配置优先）
        /// </summary>
        string GetServerUrl();

        /// <summary>
        /// 保存ComfyUI服务端地址
        /// </summary>
        bool SaveServerUrl(string serverUrl);

        /// <summary>
        /// 测试ComfyUI服务连通性
        /// </summary>
        bool TestConnection(string serverUrl, out string message);

        #region 工作流
        /// <summary>
        /// 批量导入工作流
        /// </summary>
        int BatchImportWorkflows(ComfyuiWorkflowBatchImportDto dto, long userId);

        /// <summary>
        /// 工作流分页列表
        /// </summary>
        PagedInfo<ComfyuiWorkflow> GetWorkflowList(ComfyuiWorkflowListDto parm, long userId);

        /// <summary>
        /// 获取工作流详情
        /// </summary>
        ComfyuiWorkflow GetWorkflowDetail(long id, long userId);

        /// <summary>
        /// 设置工作流分类
        /// </summary>
        bool SetWorkflowCategory(ComfyuiWorkflowCategoryDto dto, long userId);

        /// <summary>
        /// 删除工作流
        /// </summary>
        bool DeleteWorkflow(long id, long userId);

        /// <summary>
        /// 更新工作流基本信息与内容
        /// </summary>
        bool UpdateWorkflow(long id, ComfyuiWorkflowImportDto dto, long userId);

        /// <summary>
        /// 更新工作流可变节点配置
        /// </summary>
        bool UpdateWorkflowVariables(long id, string variableNodes, long userId);

        /// <summary>
        /// 获取全部可变节点（按工作流过滤）
        /// </summary>
        object GetWorkflowVariableNodes(long workflowId, long userId);

        /// <summary>
        /// 解析工作流，筛查可编辑节点（带描述）
        /// </summary>
        List<ComfyuiEditableNodeDto> GetEditableNodes(long workflowId, long userId);
        #endregion

        #region 任务
        /// <summary>
        /// 创建ComfyUI任务（表单→草稿，可入队）。refs: nodeId->参考文件
        /// </summary>
        (List<long> taskNos, string validationError) CreateTask(ComfyuiTaskCreateDto dto, Dictionary<string, IFormFile> refs, long userId);

        /// <summary>
        /// 更新任务明细（仅草稿可编辑；校验通过后自动入队）。refs: nodeId->参考文件
        /// </summary>
        (List<long> taskNos, string validationError) UpdateTask(long taskId, ComfyuiTaskCreateDto dto, Dictionary<string, IFormFile> refs, long userId);

        /// <summary>
        /// 任务分页列表（含执行队列信息与输出）
        /// </summary>
        PagedInfo<ComfyuiTaskView> GetTaskList(ComfyuiTaskListDto parm, long userId);

        /// <summary>
        /// 获取任务详情
        /// </summary>
        ComfyuiTask GetTaskDetail(long id, long userId);

        /// <summary>
        /// 删除任务（仅草稿/失败可删）
        /// </summary>
        bool DeleteTask(long id, long userId);

        /// <summary>
        /// 任务入队（草稿→执行队列）
        /// </summary>
        int EnqueueTasks(List<long> taskIds, long userId);

        /// <summary>
        /// 批量删除任务
        /// </summary>
        int BatchDeleteTask(List<long> ids, long userId);

        /// <summary>
        /// 更新任务自媒体发布状态（pending=待发布 / published=已发布）
        /// </summary>
        bool UpdatePublishStatus(long id, string publishStatus, long userId);

        /// <summary>
        /// 批量更新任务自媒体发布状态
        /// </summary>
        int BatchUpdatePublishStatus(List<long> ids, string publishStatus, long userId);

        /// <summary>
        /// 文本翻译（联网翻译，目标语言 zh-CN / en）
        /// </summary>
        Task<string> TranslateAsync(string text, string target);
        #endregion

        #region 执行队列
        /// <summary>
        /// 执行队列分页列表
        /// </summary>
        PagedInfo<ComfyuiQueue> GetQueueList(ComfyuiQueueListDto parm, long userId);

        /// <summary>
        /// 从执行队列移除（取消）
        /// </summary>
        bool CancelQueue(long id, long userId);

        /// <summary>
        /// 出队（失败/完成的任务从队列移除，回到任务列表，可重新编辑）
        /// </summary>
        bool Dequeue(long id, long userId);

        /// <summary>
        /// 重试（失败的队列记录重置为待执行，Worker 重新构建并执行）
        /// </summary>
        bool RetryQueue(long id, long userId);

        /// <summary>
        /// 批量重试（跳过不满足重试条件的记录），返回成功数
        /// </summary>
        int BatchRetryQueue(List<long> ids, long userId);

        /// <summary>
        /// 上传任务参考文件到ComfyUI input目录（回填ComfyName）
        /// </summary>
        void UploadReferenceToComfy(ComfyuiTask task);

        /// <summary>
        /// 构建提交给ComfyUI的prompt请求体
        /// </summary>
        string BuildPromptJson(ComfyuiTask task, ComfyuiWorkflow workflow);

        /// <summary>
        /// 提交prompt到ComfyUI，返回promptId
        /// </summary>
        string SubmitToComfy(string promptJson);

        /// <summary>
        /// 查询ComfyUI执行历史/输出（输出文件会下载到本地存储返回本地URL）
        /// </summary>
        List<object> QueryHistory(string promptId, long taskId = 0);

        /// <summary>
        /// 执行超时秒数
        /// </summary>
        int GetTimeoutSeconds();
        #endregion
    }
}
