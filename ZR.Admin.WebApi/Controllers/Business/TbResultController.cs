using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Service.Business;
using ZR.Service.Business.IBusinessService;
using ZR.ServiceCore.Services;

//创建时间：2025-08-25
namespace ZR.Admin.WebApi.Controllers.Business
{
    /// <summary>
    /// 
    /// </summary>
    [Route("business/TbResult")]
    public class TbResultController : BaseController
    {
        /// <summary>
        /// 接口
        /// </summary>
        private readonly ITbResultService _TbResultService;
        private readonly ITbContactService _tbContactService;
        private readonly ISysUserService sysUserService;

        public TbResultController(ITbResultService TbResultService, ITbContactService tbContactService, ISysUserService sysUserService)
        {
            _TbResultService = TbResultService;
            _tbContactService = tbContactService;
            this.sysUserService = sysUserService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [ActionPermissionFilter(Permission = "tbresult:list")]
        public IActionResult QueryTbResult([FromQuery] TbResultQueryDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            var response = _TbResultService.GetList(parm);
            return SUCCESS(response);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("distinctlist")]
        //[ActionPermissionFilter(Permission = "tbresult:distinctlist")]
        public async Task<IActionResult> QueryTbResultDistinctList([FromQuery] TbResultQueryDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            var response = await _TbResultService.GetDistinctList(parm);
            return SUCCESS(response);
        }



        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        [ActionPermissionFilter(Permission = "tbresult:query")]
        public IActionResult GetTbResult(int Id)
        {
            var response = _TbResultService.GetInfo(Id);

            var info = response.Adapt<TbResultDto>();
            return SUCCESS(info);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionPermissionFilter(Permission = "tbresult:add")]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddTbResult([FromBody] TbResultDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            var modal = parm.Adapt<TbResult>().ToCreate(HttpContext);

            var response = _TbResultService.AddTbResult(modal);

            return SUCCESS(response);
        }

        /// <summary>
        /// 问题件匹配
        /// </summary>
        /// <returns></returns>
        [HttpPost("match")]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult Match([FromBody] TbResultMatchDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            if (parm.Ids.Any())
            {
                foreach (var item in parm.Ids)
                {
                    var model = _TbResultService.GetById(item);
                    model.处理状态 = "已匹配";
                    _TbResultService.Update(model);
                }
            }

            var model2 = _tbContactService.GetFirst(w => w.CompanyId == parm.CompanyId
              && w.IsEnable == false
              && w.客户 == parm.收件人信息
              && w.客户商家名称 == parm.商家名称
              && w.对接方式 == "微信"
              && w.群名称 == parm.群名称
              );

            if (model2 != null)
            {
                return SUCCESS(model2);
            }
            var tbContactModel = new TbContact();
            tbContactModel.CompanyId = parm.CompanyId;
            tbContactModel.IsEnable = false;
            //tbContactModel.IsMatch = true;
            //tbContactModel.MatchParam = "";
            tbContactModel.客户 = parm.收件人信息;
            tbContactModel.客户商家名称 = parm.商家名称;
            tbContactModel.对接方式 = "微信";
            tbContactModel.群名称 = parm.群名称;


            var response = _tbContactService.AddTbContact(tbContactModel);
            return SUCCESS(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ActionPermissionFilter(Permission = "tbresult:edit")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateTbResult([FromBody] TbResultDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            var modal = parm.Adapt<TbResult>().ToUpdate(HttpContext);
            var response = _TbResultService.UpdateTbResult(modal);

            return ToResponse(response);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete/{ids}")]
        [ActionPermissionFilter(Permission = "tbresult:delete")]
        [Log(Title = "", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteTbResult([FromRoute] string ids)
        {
            var idArr = Tools.SplitAndConvert<int>(ids);

            return ToResponse(_TbResultService.Delete(idArr));
        }

        /// <summary>
        /// 获取转发信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("forward/{ids}")]
        [ActionPermissionFilter(Permission = "tbresult:forward")]
        public async Task<IActionResult> GetForwardMessageResult([FromRoute] string ids)
        {
            var idArr = Tools.SplitAndConvert<long>(ids);
            return SUCCESS(await _TbResultService.GetForwardMessageResult(idArr));

        }
        /// <summary>
        /// 复制信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("copy/{ids}")]
        [ActionPermissionFilter(Permission = "tbresult:copy")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateTbResultStatus([FromRoute] string ids)
        {
            var idArr = Tools.SplitAndConvert<long>(ids);
            var updateNumber = _TbResultService.UpdateTbResultStatus(idArr);
            return ToResponse(updateNumber);

        }

    }
}