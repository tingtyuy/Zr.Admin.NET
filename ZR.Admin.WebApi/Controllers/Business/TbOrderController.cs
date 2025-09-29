using Microsoft.AspNetCore.Mvc;
using ZR.Model.Business.Dto;
using ZR.Model.Business;
using ZR.Service.Business.IBusinessService;

//创建时间：2025-09-29
namespace ZR.Admin.WebApi.Controllers.Business
{
    /// <summary>
    /// 
    /// </summary>
    [Route("business/TbOrder")]
    public class TbOrderController : BaseController
    {
        /// <summary>
        /// 接口
        /// </summary>
        private readonly ITbOrderService _TbOrderService;

        public TbOrderController(ITbOrderService TbOrderService)
        {
            _TbOrderService = TbOrderService;
        }

        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [ActionPermissionFilter(Permission = "tborder:list")]
        public IActionResult QueryTbOrder([FromQuery] TbOrderQueryDto parm)
        {
            var response = _TbOrderService.GetList(parm);
            return SUCCESS(response);
        }


        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        [ActionPermissionFilter(Permission = "tborder:query")]
        public IActionResult GetTbOrder(long Id)
        {
            var response = _TbOrderService.GetInfo(Id);
            
            var info = response.Adapt<TbOrderDto>();
            return SUCCESS(info);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionPermissionFilter(Permission = "tborder:add")]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddTbOrder([FromBody] TbOrderDto parm)
        {
            var modal = parm.Adapt<TbOrder>().ToCreate(HttpContext);

            var response = _TbOrderService.AddTbOrder(modal);

            return SUCCESS(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ActionPermissionFilter(Permission = "tborder:edit")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateTbOrder([FromBody] TbOrderDto parm)
        {
            var modal = parm.Adapt<TbOrder>().ToUpdate(HttpContext);
            var response = _TbOrderService.UpdateTbOrder(modal);

            return ToResponse(response);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete/{ids}")]
        [ActionPermissionFilter(Permission = "tborder:delete")]
        [Log(Title = "", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteTbOrder([FromRoute]string ids)
        {
            var idArr = Tools.SplitAndConvert<long>(ids);

            return ToResponse(_TbOrderService.Delete(idArr));
        }

    }
}