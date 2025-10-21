using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("BU业务代理")]
    public partial class BU业务代理
    {
           public BU业务代理(){


           }
           /// <summary>
           /// Desc:代理编号
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string FUID {get;set;}

           /// <summary>
           /// Desc:代理名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string F代理名称 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F实体公司 {get;set;}

           /// <summary>
           /// Desc:预充值的单票价格
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F预充单价 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:月
           /// Nullable:True
           /// </summary>           
           public string F结算周期 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F联系方式 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F所属网点UID {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F备注 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsIdentity=true)]
           public int F排序 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public bool? F禁用 {get;set;}

    }
}
