namespace ZR.Model.System.Dto
{
    /// <summary>
    /// 提交AI任务DTO
    /// </summary>
    public class AiTaskSubmitDto
    {
        /// <summary>
        /// 提示词
        /// </summary>
        [Display(Name = "提示词")]
        [Required(ErrorMessage = "提示词不能为空")]
        public string Prompt { get; set; }
    }

    /// <summary>
    /// 查询AI任务状态DTO
    /// </summary>
    public class AiTaskQueryDto
    {
        /// <summary>
        /// 任务号
        /// </summary>
        [Display(Name = "任务号")]
        [Required(ErrorMessage = "任务号不能为空")]
        public long TaskNo { get; set; }
    }

    /// <summary>
    /// AI任务列表查询DTO
    /// </summary>
    public class AiTaskListDto : PagerInfo
    {
        /// <summary>
        /// 提示词模糊搜索
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// 状态筛选
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 功能类型筛选
        /// </summary>
        public string FuncType { get; set; }
    }

    /// <summary>
    /// N8N取任务返回DTO
    /// </summary>
    public class N8nFetchTaskDto
    {
        /// <summary>
        /// 任务号
        /// </summary>
        public long TaskNo { get; set; }

        /// <summary>
        /// 原图访问URL
        /// </summary>
        public string InputImageUrl { get; set; }

        /// <summary>
        /// 提示词
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        public string ExtParams { get; set; }
    }

    /// <summary>
    /// N8N成功回调DTO
    /// </summary>
    public class N8nCallbackSuccessDto
    {
        /// <summary>
        /// 任务号
        /// </summary>
        [Display(Name = "任务号")]
        [Required(ErrorMessage = "任务号不能为空")]
        public long TaskNo { get; set; }

        /// <summary>
        /// 结果图访问URL
        /// </summary>
        [Display(Name = "结果图URL")]
        [Required(ErrorMessage = "结果图URL不能为空")]
        public string OutputImageUrl { get; set; }
    }

    /// <summary>
    /// N8N失败回调DTO
    /// </summary>
    public class N8nCallbackFailedDto
    {
        /// <summary>
        /// 任务号
        /// </summary>
        [Display(Name = "任务号")]
        [Required(ErrorMessage = "任务号不能为空")]
        public long TaskNo { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [Display(Name = "错误信息")]
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// N8N上传结果图DTO
    /// </summary>
    public class N8nUploadResultDto
    {
        /// <summary>
        /// 任务号
        /// </summary>
        [Display(Name = "任务号")]
        [Required(ErrorMessage = "任务号不能为空")]
        public string TaskNo { get; set; }
    }

    /// <summary>
    /// AI任务更新DTO
    /// </summary>
    public class AiTaskUpdateDto
    {
        /// <summary>
        /// 提示词
        /// </summary>
        [Display(Name = "提示词")]
        [Required(ErrorMessage = "提示词不能为空")]
        public string Prompt { get; set; }
    }

    /// <summary>
    /// AI提示词模板DTO
    /// </summary>
    public class AiPromptTemplateDto
    {
        public long Id { get; set; }

        [Display(Name = "模板名称")]
        [Required(ErrorMessage = "模板名称不能为空")]
        public string Name { get; set; }

        [Display(Name = "提示词")]
        [Required(ErrorMessage = "提示词不能为空")]
        public string Prompt { get; set; }

        public string FuncType { get; set; } = "img2img";
    }
}
