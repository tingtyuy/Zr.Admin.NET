using Masuit.Tools;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq.Expressions;
using ZR.Model.Business;
using ZR.Model.Business.Dto;
using ZR.Service.Business;
using ZR.Service.Business.IBusinessService;
using ZR.ServiceCore.Services;

//创建时间：2025-09-03
namespace ZR.Admin.WebApi.Controllers.Business
{
    /// <summary>
    /// 
    /// </summary>
    [Route("business/TbContact")]
    public class TbContactController : BaseController
    {
        /// <summary>
        /// 接口
        /// </summary>
        private readonly ITbContactService _TbContactService;
        private readonly ITbWxGroupMemberService _tbWxGroupMemberService;
        private readonly ISysUserService  sysUserService;

        public TbContactController(ITbContactService TbContactService, ITbWxGroupMemberService tbWxGroupMemberService, ISysUserService sysUserService)
        {
            _TbContactService = TbContactService;
            _tbWxGroupMemberService = tbWxGroupMemberService;
            this.sysUserService = sysUserService;
        }


        /// <summary>
        /// 商户群列表
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("list2")]
        public IActionResult QueryTbContact2([FromQuery] TbContactQueryDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            var response = _TbContactService.GetList2(parm);
            return SUCCESS(response);
        }


        /// <summary>
        /// 商户群管理
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        [HttpGet("list")]
        
        //[ActionPermissionFilter(Permission = "tbcontact:list")]
        public IActionResult QueryTbContact([FromQuery] TbContactQueryDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            var response = _TbContactService.GetList(parm);
            return SUCCESS(response);
        }


        /// <summary>
        /// 查询详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet("{Id}")]
        
        //[ActionPermissionFilter(Permission = "tbcontact:query")]
        public IActionResult GetTbContact(int Id)
        {
            var response = _TbContactService.GetInfo(Id);

            var info = response.Adapt<TbContactDto>();
            return SUCCESS(info);
        }


        /// <summary>
        /// 查询微信群的数量
        /// </summary>
        
        /// <returns></returns>
        [HttpGet("totalGroupNumber")]

        //[ActionPermissionFilter(Permission = "tbcontact:query")]
        public IActionResult GetTotalGroupNumber()
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);

            Expression<Func<TbContact, bool>> exp = Expressionable.Create<TbContact>() //创建表达式
            .And(x => x.CompanyId == user.Remark && (x.IsEnable==null || x.IsEnable==false) )
            .ToExpression();//注意 这一句 不能少

            List<TbContact>  WXGroupList= _TbContactService.GetList(exp);

            WeiXinGroupCount weiXinGroupCount = new WeiXinGroupCount();
            weiXinGroupCount.TotalNumber = WXGroupList.Count;
            weiXinGroupCount.MatchedNumber = WXGroupList.Where(x=> x.客户商家名称!=null).Count();
            weiXinGroupCount.UNMatchedNumber = WXGroupList.Where(x => x.客户商家名称 == null).Count();
            return SUCCESS(weiXinGroupCount);
        }

        /// <summary>
        /// 查询微信群的数量,从tb_weixin_group_statistics 数据表取数据
        /// </summary>
        /// <param name="strDate">日期,格式为yyyy-mm-dd </param>
        /// <returns></returns>
        [HttpGet("totalGroupNumber_2")]

        //[ActionPermissionFilter(Permission = "tbcontact:query")]
        public IActionResult GetTotalGroupNumber_2(string strDate)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            string strSql = " select  a.companyId, a.fdate, a.totalNumber, b.totalNumber as lastMonthNumber, a.matchedNumber, a.unMatchedNumber  from tb_weixin_group_statistics a left join tb_weixin_group_statistics b on a.companyId=b.companyId and a.fdate=date_add(b.fdate, INTERVAL 1 MONTH) where a.companyId='" + user.Remark + "' and a.fdate=str_to_date('"+ strDate + "', '%Y-%m-%d') ";

            List<WeiXinGroupStatisticsDto_2>  dataList= _TbContactService.AsSugarClient().Ado.SqlQuery<WeiXinGroupStatisticsDto_2>(strSql);
            return SUCCESS(dataList);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        //[ActionPermissionFilter(Permission = "tbcontact:add")]
        [Log(Title = "", BusinessType = BusinessType.INSERT)]
        public IActionResult AddTbContact([FromBody] TbContactDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            var modal = parm.Adapt<TbContact>().ToCreate(HttpContext);

            var response = _TbContactService.AddTbContact(modal);

            return SUCCESS(response);
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        //[ActionPermissionFilter(Permission = "tbcontact:edit")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateTbContact([FromBody] TbContactDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            var modal = parm.Adapt<TbContact>().ToUpdate(HttpContext);
            var response = _TbContactService.UpdateTbContact(modal);

            return ToResponse(response);
        }

        /// <summary>
        /// 商户群列表修改私人群状态和群名称
        /// </summary>
        /// <returns></returns>
        [HttpPut("update")]
        //[ActionPermissionFilter(Permission = "tbcontact:edit")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult UpdateTbContact2([FromBody] TbContactDto parm)
        {
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            parm.CompanyId = user.Remark;
            var modal = parm.Adapt<TbContact>().ToUpdate(HttpContext);
            //修改群状态
            var currentRow= _TbContactService.GetInfo(parm.Id);
            var modal2 = _TbContactService.Queryable().Where(w => w.CompanyId == currentRow.CompanyId && w.群名称 == currentRow.群名称 && string.IsNullOrEmpty(w.客户)).First();
            modal2.IsEnable = parm.IsEnable;
            _TbContactService.UpdateTbContact(modal2);
            //修改群名称
            var response = _TbContactService.Update(w=>w.群名称==currentRow.群名称,c=> new TbContact() {  群名称=parm.群名称});
            return ToResponse(response);
        }

        /// <summary>
        /// 设定匹配规则.通过id进行更新
        /// </summary>
        /// <returns></returns>
        [HttpPut("match")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult MatchTbContact([FromBody] TbContactMatchDto parm)
        {
            //var modal = parm.Adapt<TbContact>().ToUpdate(HttpContext);
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            _tbWxGroupMemberService.Update(w => w.ContactId == parm.Id, a => new TbWxGroupMember
            {
                IsInternal = false
            });

            if (parm.MIds is not null && parm.MIds.Any())
            {
             
                _tbWxGroupMemberService.Update(w => parm.MIds.Contains(w.Id), a => new TbWxGroupMember
                {
                    IsInternal = true
                });
            }

            var response = _TbContactService.Update(w => w.Id == parm.Id, a => new TbContact
            {
                IsEnable = parm.IsEnable
                 ,
                IsMatch = true,
                商户名匹配 = parm.商户名匹配,
                发件人匹配 = parm.发件人匹配,
                联系电话匹配 = parm.联系电话匹配,
                地址匹配 = parm.地址匹配,
                MatchParam = parm.MatchParam,
                CompanyId=user.Remark

            });

            return ToResponse(response);
        }

        //Liangzw  2025-11-07
        /// <summary>
        /// 设定匹配规则.通过微信群进行更新
        /// </summary>
        /// <returns></returns>
        [HttpPut("matchByWeiXinGroup")]
        [Log(Title = "", BusinessType = BusinessType.UPDATE)]
        public IActionResult MatchTbContactByWeiXinGroup([FromBody] TbContactMatchDto parm)
        {
            //var modal = parm.Adapt<TbContact>().ToUpdate(HttpContext);
            long userId = HttpContext.GetUId();
            var user = sysUserService.SelectUserById(userId);
            _tbWxGroupMemberService.Update(w => w.ContactId == parm.Id, a => new TbWxGroupMember
            {
                IsInternal = false
            });

            if (parm.MIds is not null && parm.MIds.Any())
            {

                _tbWxGroupMemberService.Update(w => parm.MIds.Contains(w.Id), a => new TbWxGroupMember
                {
                    IsInternal = true
                });
            }

            var response = _TbContactService.Update(w => w.群名称 == parm.群名称, a => new TbContact
            {
                IsEnable = parm.IsEnable
                 ,
                IsMatch = true,
                商户名匹配 = parm.商户名匹配,
                发件人匹配 = parm.发件人匹配,
                联系电话匹配 = parm.联系电话匹配,
                地址匹配 = parm.地址匹配,
                MatchParam = parm.MatchParam,
                CompanyId = user.Remark

            });

            return ToResponse(response);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("delete/{ids}")]
        //[ActionPermissionFilter(Permission = "tbcontact:delete")]
        [Log(Title = "", BusinessType = BusinessType.DELETE)]
        public IActionResult DeleteTbContact([FromRoute] string ids)
        {
            var idArr = Tools.SplitAndConvert<int>(ids);

            return ToResponse(_TbContactService.Delete(idArr));
        }

    }
}