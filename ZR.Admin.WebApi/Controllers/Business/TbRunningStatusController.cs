using Microsoft.AspNetCore.Mvc;
using ZR.Model.Business;
using ZR.Service.Business;
using ZR.Service.Business.IBusinessService;

namespace ZR.Admin.WebApi.Controllers.Business
{
    [Route("business/TbRunningStatus")]
    public class TbRunningStatusController :BaseController
    {
        private readonly ITbRunningStatusService _TbRunningStatusService;
        

        public TbRunningStatusController(TbRunningStatusService theService)
        {
            _TbRunningStatusService = theService;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult GetData(int id)
        {
            var response = _TbRunningStatusService.GetInfo(id);

            return SUCCESS(response);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddData([FromBody] TbRobotRunningStatus parm)
        {
            var modal = parm.Adapt<TbRobotRunningStatus>().ToCreate(HttpContext);
            var response = _TbRunningStatusService.AddRecord(modal);
            return SUCCESS(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [AllowAnonymous]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateData([FromBody] TbRobotRunningStatus parm)
        {
            var modal = parm.Adapt<TbRobotRunningStatus>().ToUpdate(HttpContext);
            var response = _TbRunningStatusService.UpdateRecord(modal);
            return ToResponse(response);
        }
    }
}
