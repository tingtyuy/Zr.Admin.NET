using Microsoft.AspNetCore.Mvc;
using ZR.Model.Business.Dto;
using ZR.Model.Business;
using ZR.Service.Business.IBusinessService;
using FastExpressionCompiler.ImTools;
using SqlSugar;
using System.Linq.Expressions;
using ZR.Service.Business;

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
        [AllowAnonymous]
        //[ActionPermissionFilter(Permission = "tborder:query")]
        public IActionResult GetTbOrder(long Id)
        {
            var response = _TbOrderService.GetInfo(Id);
            
            var info = response.Adapt<TbOrderDto>();
            return SUCCESS(info);
        }


        /// <summary>
        /// 查询列表
        /// </summary>
        
        /// <param name="strDate">日期，格式为 yyyy-MM-dd</param>
        /// <param name="strCompanyId">公司ID</param>
        /// <returns></returns>
        [HttpGet("dailyData")]
        //[ActionPermissionFilter(Permission = "tborder:list")]
        [AllowAnonymous]
        public IActionResult QueryDailyData(string strDate, string strCompanyId)
        {
            DateTime theDate=Convert.ToDateTime(strDate);
            DateTime nextDay=theDate.AddDays(1);    //第二天
            Expression<Func<TbOrder, bool>> exp = Expressionable.Create<TbOrder>() //创建表达式
                 .And(x => x.useTime>= theDate && x.useTime<nextDay)
                 .And(x => x.CompanyId == strCompanyId)
                 .ToExpression();//注意 这一句 不能少

            List<TbOrder> dataList = _TbOrderService.GetList(exp);

            if (dataList.Count > 0)
            {
                ProcessOrder processOrder = new ProcessOrder();
                processOrder.TotalNumber = dataList.Where(x => x.状态 == "被读取").Count();
                processOrder.FReject = dataList.Where(x => x.状态 == "被读取" && x.问题件类型 == "拒收").Count();
                processOrder.FDamage = dataList.Where(x => x.状态 == "被读取" && x.问题件类型 == "破损件").Count();
                processOrder.Funknown = dataList.Where(x => x.状态 == "被读取" && x.问题件类型 == "信息有误-收货信息不详").Count();

                processOrder.FOther = processOrder.TotalNumber - processOrder.FReject - processOrder.FDamage - processOrder.Funknown;
                return SUCCESS(processOrder);
            }
            else
            {
                return SUCCESS(new ProcessOrder());
            }
            
        }


        /// <summary>
        /// 获取一个阶段的问题件数据，这是自动化工具处理过后的问题件
        /// </summary>
        ///<param name="strStartDate">起始日期 yyyy-MM-dd HH:mm:ss</param>
        ///<param name="strEndDate">截止日期 yyyy-MM-dd HH:mm:ss</param>
        ///<param name="strCompanyId">公司ID</param>
        /// <returns></returns>
        [HttpGet("monthlyData_2")]
        //[ActionPermissionFilter(Permission = "tborder:list")]
        [AllowAnonymous]
        public IActionResult SearchPeriodData(string strStartDate, string  strEndDate, string strCompanyId)
        {
            DateTime startDate=Convert.ToDateTime(strStartDate);
            DateTime endDate = Convert.ToDateTime(strEndDate);

            Expression<Func<TbOrder, bool>> exp = Expressionable.Create<TbOrder>() //创建表达式
                 .And(x => x.useTime >= startDate && x.useTime <= endDate)
                 .And(x => x.CompanyId == strCompanyId)
                 .ToExpression();//注意 这一句 不能少

            List<TbOrder> dataList = _TbOrderService.GetList(exp);

            if (dataList.Count > 0)
            {               
                ProcessOrder processOrder = new ProcessOrder();
                processOrder.TotalNumber = dataList.Where(x => x.状态 == "被读取").Count();
                processOrder.FReject = dataList.Where(x => x.状态 == "被读取" && x.问题件类型 == "拒收").Count();
                processOrder.FDamage = dataList.Where(x => x.状态 == "被读取" && x.问题件类型 == "破损件").Count();
                processOrder.Funknown = dataList.Where(x => x.状态 == "被读取" && x.问题件类型 == "信息有误-收货信息不详").Count();

                processOrder.FOther = processOrder.TotalNumber - processOrder.FReject - processOrder.FDamage - processOrder.Funknown;

                List<TbOrder> sortedList=dataList.OrderBy(x=> x.useTime).ToList();
                DateTime minUseTime = sortedList[0].useTime;
                DateTime maxUseTime = sortedList[sortedList.Count-1].useTime;
                TimeSpan ts=maxUseTime - minUseTime;
                processOrder.TotalUseTime= ts.TotalSeconds;     //总耗时

                return SUCCESS(processOrder);
            }
            else
            {
                return SUCCESS(new ProcessOrder());
            }

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