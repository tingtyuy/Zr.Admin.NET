using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Collections;
using System.Linq.Expressions;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Service.Business;
using ZR.Service.Business.IBusinessService;

namespace ZR.Admin.WebApi.Controllers.Business
{
    [Route("business/Robot")]
    public class RobotController :BaseController
    {        
        /// <summary>
        /// 接口
        /// </summary>
        private readonly IRobotService _RobotService;
        private readonly ITbWxGroupMemberService _tbWxGroupMemberService;
        private readonly ISysUserService sysUserService;

        public RobotController(IRobotService theService, ITbWxGroupMemberService tbWxGroupMemberService, ISysUserService sysUserService)
        {
            _RobotService = theService;
            _tbWxGroupMemberService = tbWxGroupMemberService;
            this.sysUserService = sysUserService;
        }


        /// <summary>
        /// 机器人列表
        /// </summary>
        /// <param name="strCompanyId"></param>
        /// <returns></returns>
        [HttpGet("dataList")]
        //[ActionPermissionFilter(Permission = "Robot:dataList")]
        [AllowAnonymous]
        public IActionResult GetDataList(string strCompanyId)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);

            Expression<Func<Robot, bool>> exp = Expressionable.Create<Robot>() //创建表达式
             .And(x => x.companyId == strCompanyId)
             .ToExpression();//注意 这一句 不能少

            List<Robot> dataList= _RobotService.GetList(exp);            
            return SUCCESS(dataList);
        }


        /// <summary>
        /// 机器人列表
        /// </summary>
        
        /// <returns></returns>
        [HttpGet("dataList_2")]
        //[ActionPermissionFilter(Permission = "Robot:dataList")]
        [AllowAnonymous]
        public IActionResult GetDataList2()
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);

            if (user != null)
            {
                Expression<Func<Robot, bool>> exp = Expressionable.Create<Robot>() //创建表达式
                 .And(x => x.companyId == user.Remark)
                 .ToExpression();//注意 这一句 不能少
                List<Robot> dataList = _RobotService.GetList(exp);
                return SUCCESS(dataList);
            }
            else
            {
                return SUCCESS(new ArrayList());
            }
            
        }

        /// <summary>
        /// 商户群列表
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        [HttpPost("modify")]
        //[ActionPermissionFilter(Permission = "Robot:modify")]
        [AllowAnonymous]
        public IActionResult UpdateRecord([FromBody]  List<Robot>  dataList)
        {
            //只更新startTime 列
            int theResult = 0;      //默认值0表示执行失败, 1表示执行成功。
            try
            {
                if (dataList.Count > 0)
                {
                    foreach (var theObject in dataList)
                    {
                        theResult = _RobotService.Update(theObject, t => new { t.startTime });
                    }
                    return SUCCESS(1);
                }
                else
                {
                    return SUCCESS(0);
                }
                
            }
            catch (Exception ex)
            {
                return SUCCESS(0);
            }
        }

    }
}
