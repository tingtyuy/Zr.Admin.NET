using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("CRM客户")]
    public partial class CRM客户
    {
           public CRM客户(){


           }
           /// <summary>
           /// Desc:客户编号
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string FUID {get;set;}

           /// <summary>
           /// Desc:客户名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string F客户名称 {get;set;}

           /// <summary>
           /// Desc:客户类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F客户类型 {get;set;}

           /// <summary>
           /// Desc:联系人
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F联系人 {get;set;}

           /// <summary>
           /// Desc:联系方式
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F联系方式 {get;set;}

           /// <summary>
           /// Desc:地址
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F发件地址 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F办公地址 {get;set;}

           /// <summary>
           /// Desc:客户所属人员(销售人员)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F业务员UID {get;set;}

           /// <summary>
           /// Desc:客户状态(0正常，1公海)
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F客户状态 {get;set;}

           /// <summary>
           /// Desc:货物类型
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F货物类型 {get;set;}

           /// <summary>
           /// Desc:预充值的单票价格
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F预充单价 {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F备注 {get;set;}

           /// <summary>
           /// Desc:附件
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F附件 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public bool F禁用 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F所属网点UID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F客户临时ID {get;set;}

    }
}
