using Microsoft.AspNetCore.Mvc;
using ZR.Model.Business.Dto;
using ZR.Model.Business;
using ZR.Service.Business.IBusinessService;
using ZR.Service.Business;
using SqlSugar;
using System.Linq.Expressions;
using Azure;

//创建时间：2025-09-16
namespace ZR.Admin.WebApi.Controllers.Business
{
    /// <summary>
    /// 
    /// </summary>
    [Route("business/Company")]
    public class CompanyController : BaseController
    {
        /// <summary>
        /// 接口
        /// </summary>
        private readonly ICompanyService _CompanyService;       

        public CompanyController(ICompanyService CompanyService)
        {
            _CompanyService = CompanyService;
        }
                
        /// <summary>
        /// 查询列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("list")]
        [ActionPermissionFilter(Permission = "company:list")]
        public IActionResult QueryCompany([FromQuery] CompanyQueryDto parm)
        {
            var response = _CompanyService.GetList(parm);
            return SUCCESS(response);
        }


        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        [ActionPermissionFilter(Permission = "company:query")]
        public IActionResult GetCompany(int Id)
        {
            var response = _CompanyService.GetInfo(Id);
            
            var info = response.Adapt<CompanyDto>();
            return SUCCESS(info);
        }

        /// <summary>
        /// 查询详情
        /// </summary>
        
        /// <returns></returns>
        [HttpGet("getData_2")]
        //[ActionPermissionFilter(Permission = "company:query")]
        [AllowAnonymous]
        public IActionResult GetCompany(string  strCompanyId)
        {           
            Expression<Func<Company, bool>> exp = Expressionable.Create<Company>() //创建表达式
                .And(x => x.CompanyId == strCompanyId)
                .ToExpression();//注意 这一句 不能少

            var response = _CompanyService.GetFirst(exp);
            return SUCCESS(response);           
        }



        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionPermissionFilter(Permission = "company:add")]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddCompany([FromBody] CompanyDto parm)
        {
            var modal = parm.Adapt<Company>().ToCreate(HttpContext);

            var response = _CompanyService.AddCompany(modal);

            return SUCCESS(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ActionPermissionFilter(Permission = "company:edit")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateCompany([FromBody] CompanyDto parm)
        {
            var modal = parm.Adapt<Company>().ToUpdate(HttpContext);
            var response = _CompanyService.UpdateCompany(modal);

            return ToResponse(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPost("updateCompanyEmail")]
        //[ActionPermissionFilter(Permission = "company:edit")]
        [AllowAnonymous]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateCompanyEmail([FromBody] Company theObject)
        {            
            var response = _CompanyService.Update(theObject, t => new { t.OperationEmailTo, t.OperationEmailCC });

            return ToResponse(response);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete/{ids}")]
        [ActionPermissionFilter(Permission = "company:delete")]
        [Log(Title = "", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteCompany([FromRoute]string ids)
        {
            var idArr = Tools.SplitAndConvert<int>(ids);

            return ToResponse(_CompanyService.Delete(idArr));
        }

    }
}