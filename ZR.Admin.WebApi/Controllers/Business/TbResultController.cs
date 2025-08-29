using Microsoft.AspNetCore.Mvc;
using ZR.Model.Business.Dto;
using ZR.Model.Business;
using ZR.Service.Business.IBusinessService;
using System.Threading.Tasks;

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

        public TbResultController(ITbResultService TbResultService)
        {
            _TbResultService = TbResultService;
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
            var response = _TbResultService.GetList(parm);
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
            var modal = parm.Adapt<TbResult>().ToCreate(HttpContext);

            var response = _TbResultService.AddTbResult(modal);

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