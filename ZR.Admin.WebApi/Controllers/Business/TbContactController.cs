using Masuit.Tools;
using Microsoft.AspNetCore.Mvc;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Service.Business;
using ZR.Service.Business.IBusinessService;

//创建时间：2025-09-03
namespace ZR.Admin.WebApi.Controllers.Business
{
    /// <summary>
    /// 
    /// </summary>
    [Route("business/TbContact")]
    public class TbContactController : BaseController
    {
        /// <summary>
        /// 接口
        /// </summary>
        private readonly ITbContactService _TbContactService;
        private readonly ITbWxGroupMemberService _tbWxGroupMemberService;

        public TbContactController(ITbContactService TbContactService, ITbWxGroupMemberService tbWxGroupMemberService)
        {
            _TbContactService = TbContactService;
            _tbWxGroupMemberService = tbWxGroupMemberService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("list")]
        //[ActionPermissionFilter(Permission = "tbcontact:list")]
        public IActionResult QueryTbContact([FromQuery] TbContactQueryDto parm)
        {
            var response = _TbContactService.GetList(parm);
            return SUCCESS(response);
        }


        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        //[ActionPermissionFilter(Permission = "tbcontact:query")]
        public IActionResult GetTbContact(int Id)
        {
            var response = _TbContactService.GetInfo(Id);

            var info = response.Adapt<TbContactDto>();
            return SUCCESS(info);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[ActionPermissionFilter(Permission = "tbcontact:add")]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddTbContact([FromBody] TbContactDto parm)
        {
            var modal = parm.Adapt<TbContact>().ToCreate(HttpContext);

            var response = _TbContactService.AddTbContact(modal);

            return SUCCESS(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        //[ActionPermissionFilter(Permission = "tbcontact:edit")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateTbContact([FromBody] TbContactDto parm)
        {
            var modal = parm.Adapt<TbContact>().ToUpdate(HttpContext);
            var response = _TbContactService.UpdateTbContact(modal);

            return ToResponse(response);
        }

        /// <summary>
        /// 设定匹配规则
        /// </summary>
        /// <returns></returns>
        [HttpPut("match")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult MatchTbContact([FromBody] TbContactMatchDto parm)
        {
            //var modal = parm.Adapt<TbContact>().ToUpdate(HttpContext);
            _tbWxGroupMemberService.Update(w => w.ContactId == parm.Id, a => new TbWxGroupMember
            {
                IsInternal = false
            });

            if (parm.MIds is not null && parm.MIds.Any())
            {
             
                _tbWxGroupMemberService.Update(w => parm.MIds.Contains(w.Id), a => new TbWxGroupMember
                {
                    IsInternal = true
                });
            }

            var response = _TbContactService.Update(w => w.Id == parm.Id, a => new TbContact
            {
                IsEnable = parm.IsEnable
                 ,
                IsMatch = true,
                商户名匹配 = parm.商户名匹配,
                发件人匹配 = parm.发件人匹配,
                联系电话匹配 = parm.联系电话匹配,
                地址匹配 = parm.地址匹配,
                MatchParam = parm.MatchParam

            });

            return ToResponse(response);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete/{ids}")]
        //[ActionPermissionFilter(Permission = "tbcontact:delete")]
        [Log(Title = "", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteTbContact([FromRoute] string ids)
        {
            var idArr = Tools.SplitAndConvert<int>(ids);

            return ToResponse(_TbContactService.Delete(idArr));
        }

    }
}