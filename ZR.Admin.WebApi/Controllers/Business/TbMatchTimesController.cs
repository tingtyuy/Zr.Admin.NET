using Microsoft.AspNetCore.Mvc;
using ZR.Model.Business.Dto;
using ZR.Model.Business;
using ZR.Service.Business;
using ZR.Service.Business.IBusinessService;
using ZR.ServiceCore.Services;
using SqlSugar;
using System.Linq.Expressions;

namespace ZR.Admin.WebApi.Controllers.Business
{
    [Route("business/TbMatchTimes")]
    public class TbMatchTimesController :BaseController
    {
        private readonly ITbMatchTimesService _TbMatchTimesService;

        public TbMatchTimesController(ITbMatchTimesService theService)
        {
            _TbMatchTimesService = theService;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost("Add")]
        [AllowAnonymous]
        //[ActionPermissionFilter(Permission = "tbcontact:add")]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddTbContact([FromBody] TbMatchTimes theParam)
        {
            string strDate = DateTime.Now.ToString("yyyy-MM-dd");

            theParam.fdate = Convert.ToDateTime(strDate );
            theParam.matchTime = DateTime.Now;
            var response = _TbMatchTimesService.Add(theParam);
            return SUCCESS(response);
        }

        /// <summary>
        /// 获取用户已匹配的商户群数
        /// </summary>
        /// <param name="strDate">日期 yyyy-MM-dd</param>
        /// <param name="strUserAccount">用户登录账户，不是昵称</param>
        /// <returns></returns>
        [HttpGet("FCount")]
        [ActionPermissionFilter(Permission = "tbresult:list")]
        public IActionResult GetMatchTimes(string strDate, string strUserAccount)
        {
            DateTime theDate=Convert.ToDateTime(strDate);

            Expression<Func<TbMatchTimes, bool>> theExp=Expressionable.Create< TbMatchTimes >()
                .And(x=>x.fdate== theDate && x.foperator== strUserAccount)
                .ToExpression();

            List<TbMatchTimes> dataList = _TbMatchTimesService.GetList(theExp);
            return SUCCESS(dataList.Count);
        }


    }
}
