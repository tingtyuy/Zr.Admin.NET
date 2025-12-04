using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.ComponentModel.Design;
using System.Linq;
using System.Linq.Expressions;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Service.Business;
using ZR.Service.Business.IBusinessService;
using ZR.ServiceCore.Services;

namespace ZR.Admin.WebApi.Controllers.Business
{
    [Route("business/FDataCount")]
    public class FDataCountController :BaseController
    {
        private readonly IFDataCountService _FDataCountService;
        
        public FDataCountController(IFDataCountService theService)
        {
            _FDataCountService = theService;            
        }

        /// <summary>
        /// 从网点管家抓取的数据
        /// </summary>
        /// <param name="strCompanyId">公司ID</param>
        /// <param name="strEndDate">截止日,格式为 yyyy-MM-dd</param>
        /// <param name="strStartDate">起始日,格式为 yyyy-MM-dd</param>
        /// <returns></returns>
        [HttpGet("dataRow")]
        //[ActionPermissionFilter(Permission = "Robot:dataList")]
        [AllowAnonymous]
        public IActionResult GetDataList(string strCompanyId, string strStartDate, string strEndDate)
        {
            DateTime startTime = Convert.ToDateTime(strStartDate);
            DateTime endTime = Convert.ToDateTime(strEndDate );
            Expression<Func<FDataCount, bool>> exp = Expressionable.Create<FDataCount>() //创建表达式
             .And(x => x.companyid == strCompanyId && x.fdate >= startTime && x.fdate <= endTime)
             .ToExpression();//注意 这一句 不能少

            List<FDataCount> dataList = _FDataCountService.GetList(exp);

            FDataCountDto  theObject= dataList.GroupBy(x => x.companyid)
                .Select(g => new FDataCountDto
                {
                    companyid = g.Key,                   
                    allcount = g.Sum(t => t.allcount),
                    checkcount = g.Sum(t => t.checkcount),
                    noticecount = g.Sum(t => t.noticecount)                    
                })
                .FirstOrDefault();
             
            return SUCCESS(theObject);
        }

    }
}
