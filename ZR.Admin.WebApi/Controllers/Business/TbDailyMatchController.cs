using Microsoft.AspNetCore.Mvc;
using ZR.Model.Business.Dto;
using ZR.Model.Business;
using ZR.Service.Business.IBusinessService;
using ZR.Service.Business;

namespace ZR.Admin.WebApi.Controllers.Business
{
    [Route("tbDailyMatch")]
    public class TbDailyMatchController:BaseController
    {
        private readonly ITbDailyMatchService _TbDailyMatchService;
        private readonly ITbWxGroupMemberService _tbWxGroupMemberService;
        private readonly ISysUserService sysUserService;


        public TbDailyMatchController(ITbDailyMatchService theService)
        {
            _TbDailyMatchService = theService;
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
            var response = _TbDailyMatchService.GetInfo(id);

            return SUCCESS(response);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddData([FromBody] TbDailyMatch parm)
        {
            var modal = parm.Adapt<TbDailyMatch>().ToCreate(HttpContext);

            var response = _TbDailyMatchService.AddRecord(modal);

            return SUCCESS(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [AllowAnonymous]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateData([FromBody] TbDailyMatch parm)
        {
            var modal = parm.Adapt<TbDailyMatch>().ToUpdate(HttpContext);
            var response = _TbDailyMatchService.UpdateRecord(modal);

            return ToResponse(response);
        }
                
    }
}
