using Microsoft.AspNetCore.Mvc;
using ZR.Model.Business.Dto;
using ZR.Model.Business;
using ZR.Service.Business.IBusinessService;

//创建时间：2025-09-16
namespace ZR.Admin.WebApi.Controllers.Business
{
    /// <summary>
    /// 
    /// </summary>
    [Route("business/UserWxacount")]
    public class UserWxacountController : BaseController
    {
        /// <summary>
        /// 接口
        /// </summary>
        private readonly IUserWxacountService _UserWxacountService;

        public UserWxacountController(IUserWxacountService UserWxacountService)
        {
            _UserWxacountService = UserWxacountService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [ActionPermissionFilter(Permission = "userwxacount:list")]
        public IActionResult QueryUserWxacount([FromQuery] UserWxacountQueryDto parm)
        {
            var response = _UserWxacountService.GetList(parm);
            return SUCCESS(response);
        }


        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        [ActionPermissionFilter(Permission = "userwxacount:query")]
        public IActionResult GetUserWxacount(int Id)
        {
            var response = _UserWxacountService.GetInfo(Id);
            
            var info = response.Adapt<UserWxacountDto>();
            return SUCCESS(info);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionPermissionFilter(Permission = "userwxacount:add")]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddUserWxacount([FromBody] UserWxacountDto parm)
        {
            var modal = parm.Adapt<UserWxacount>().ToCreate(HttpContext);

            var response = _UserWxacountService.AddUserWxacount(modal);

            return SUCCESS(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ActionPermissionFilter(Permission = "userwxacount:edit")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateUserWxacount([FromBody] UserWxacountDto parm)
        {
            var modal = parm.Adapt<UserWxacount>().ToUpdate(HttpContext);
            var response = _UserWxacountService.UpdateUserWxacount(modal);

            return ToResponse(response);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete/{ids}")]
        [ActionPermissionFilter(Permission = "userwxacount:delete")]
        [Log(Title = "", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteUserWxacount([FromRoute]string ids)
        {
            var idArr = Tools.SplitAndConvert<int>(ids);

            return ToResponse(_UserWxacountService.Delete(idArr));
        }

    }
}