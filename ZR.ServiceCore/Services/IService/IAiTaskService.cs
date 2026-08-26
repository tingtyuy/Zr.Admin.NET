using Microsoft.AspNetCore.Http;
using ZR.Model;
using ZR.Model.System;
using ZR.Model.System.Dto;

namespace ZR.ServiceCore.Services
{
    /// <summary>
    /// AI任务服务接口
    /// </summary>
    public interface IAiTaskService : IBaseService<AiTask>
    {
        /// <summary>
        /// 提交AI任务
        /// </summary>
        /// <param name="dto">提交参数</param>
        /// <param name="formFile">上传的图片</param>
        /// <param name="userId">用户ID</param>
        /// <returns>任务号</returns>
        long SubmitTask(AiTaskSubmitDto dto, IFormFile formFile, long userId);

        /// <summary>
        /// 获取任务状态
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <param name="userId">用户ID</param>
        /// <returns>任务信息</returns>
        AiTask GetTaskStatus(long taskNo, long userId);

        /// <summary>
        /// 获取我的任务列表
        /// </summary>
        /// <param name="parm">查询参数</param>
        /// <param name="userId">用户ID</param>
        /// <returns>分页列表</returns>
        PagedInfo<AiTask> GetMyTaskList(AiTaskListDto parm, long userId);

        /// <summary>
        /// N8N取一个待处理任务
        /// </summary>
        /// <returns>任务信息</returns>
        N8nFetchTaskDto FetchPendingTask();

        /// <summary>
        /// N8N上传结果图
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <param name="file">结果图文件</param>
        /// <returns>结果图访问URL</returns>
        string UploadResultImage(long taskNo, IFormFile file);

        /// <summary>
        /// 上传Base64图片
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <param name="base64Image">Base64图片数据</param>
        /// <returns>结果图访问URL</returns>
        string UploadBase64Image(long taskNo, string base64Image, int? fetchAttemptCount = null);

        /// <summary>
        /// 更新任务
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <param name="prompt">提示词</param>
        /// <param name="userId">用户ID</param>
        /// <param name="tags">标签</param>
        /// <returns>是否成功</returns>
        bool UpdateTask(long taskNo, string prompt, long userId, string tags = null, string taskName = null);

        /// <summary>
        /// N8N成功回调
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <param name="outputImageUrl">结果图URL</param>
        /// <returns>是否成功</returns>
        bool CallbackSuccess(long taskNo, string outputImageUrl, int? fetchAttemptCount = null);

        /// <summary>
        /// N8N失败回调
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns>是否成功</returns>
        bool CallbackFailed(long taskNo, string errorMsg);

        /// <summary>
        /// 重试任务
        /// </summary>
        /// <param name="taskNo">任务号</param>
        /// <returns>是否成功</returns>
        bool RetryTask(long taskNo);

        /// <summary>
        /// 批量重试失败任务
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>重试的任务数</returns>
        int BatchRetryFailed(long userId);

        /// <summary>
        /// 检测超时任务
        /// </summary>
        /// <param name="timeoutMinutes">超时分钟数</param>
        /// <returns>处理的任务数</returns>
        int CheckTimeout(int timeoutMinutes);

        List<AiPromptTemplate> GetMyTemplates(long userId);
        AiPromptTemplate SaveTemplate(AiPromptTemplateDto dto, long userId);
        bool DeleteTemplate(long id, long userId);
        bool DeleteTask(long taskNo, long userId);

        /// <summary>
        /// 批量管理标签
        /// </summary>
        int BatchAddTags(List<long> taskNos, string tags, string removeTags, long userId);

        /// <summary>
        /// 批量下载结果图（打包ZIP）
        /// </summary>
        MemoryStream BatchDownloadResult(List<long> taskNos, long userId);

        /// <summary>
        /// 获取结果图存储路径
        /// </summary>
        string GetResultStoragePath();

        /// <summary>
        /// 获取结果图列表（匿名可访问）
        /// </summary>
        object GetResultImageList(AiTaskListDto parm);

        /// <summary>
        /// 批量标记任务为已发布
        /// </summary>
        int BatchMarkPublished(List<long> taskNos, long userId);
    }
}
