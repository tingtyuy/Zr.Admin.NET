using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Service.Business.IBusinessService;

//创建时间：2025-09-03
namespace ZR.Admin.WebApi.Controllers.Business
{
    /// <summary>
    /// 
    /// </summary>
    [Route("business/TbWxGroupMember")]
    public class TbWxGroupMemberController : BaseController
    {
        /// <summary>
        /// 接口
        /// </summary>
        private readonly ITbWxGroupMemberService _TbWxGroupMemberService;

        public TbWxGroupMemberController(ITbWxGroupMemberService TbWxGroupMemberService)
        {
            _TbWxGroupMemberService = TbWxGroupMemberService;
        }

        /// <summary>
        /// 查询Options
        /// </summary>
        /// <returns></returns>
        [HttpGet("options")]
        public IActionResult QueryTbWxGroupMemberOptions([FromQuery] TbWxGroupMemberQueryDto parm)
        {
            var response = _TbWxGroupMemberService.Queryable()
                .WhereIF(parm.ContactId.HasValue, w => w.ContactId == parm.ContactId)
                .WhereIF(string.IsNullOrEmpty(parm.GroupName), w => w.GroupName == parm.GroupName)
                .WhereIF(parm.IsInternal.HasValue, w=>w.IsInternal == parm.IsInternal).ToList();
            return SUCCESS(response);
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [ActionPermissionFilter(Permission = "tbwxgroupmember:list")]
        public IActionResult QueryTbWxGroupMember([FromQuery] TbWxGroupMemberQueryDto parm)
        {
            var response = _TbWxGroupMemberService.GetList(parm);
            return SUCCESS(response);
        }



        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        [ActionPermissionFilter(Permission = "tbwxgroupmember:query")]
        public IActionResult GetTbWxGroupMember(int Id)
        {
            var response = _TbWxGroupMemberService.GetInfo(Id);
            
            var info = response.Adapt<TbWxGroupMemberDto>();
            return SUCCESS(info);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionPermissionFilter(Permission = "tbwxgroupmember:add")]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddTbWxGroupMember([FromBody] TbWxGroupMemberDto parm)
        {
            var modal = parm.Adapt<TbWxGroupMember>().ToCreate(HttpContext);

            var response = _TbWxGroupMemberService.AddTbWxGroupMember(modal);

            return SUCCESS(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ActionPermissionFilter(Permission = "tbwxgroupmember:edit")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateTbWxGroupMember([FromBody] TbWxGroupMemberDto parm)
        {
            var modal = parm.Adapt<TbWxGroupMember>().ToUpdate(HttpContext);
            var response = _TbWxGroupMemberService.UpdateTbWxGroupMember(modal);

            return ToResponse(response);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete/{ids}")]
        [ActionPermissionFilter(Permission = "tbwxgroupmember:delete")]
        [Log(Title = "", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteTbWxGroupMember([FromRoute]string ids)
        {
            var idArr = Tools.SplitAndConvert<int>(ids);

            return ToResponse(_TbWxGroupMemberService.Delete(idArr));
        }

    }
}