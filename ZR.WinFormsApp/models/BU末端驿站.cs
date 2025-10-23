using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("BU末端驿站")]
    public partial class BU末端驿站
    {
           public BU末端驿站(){


           }
           /// <summary>
           /// Desc:驿站编号
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string FUID {get;set;}

           /// <summary>
           /// Desc:驿站名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string F驿站名称 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F实体公司 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F联系方式 {get;set;}

           /// <summary>
           /// Desc:自建|三方|合作
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F经营类型 {get;set;}

           /// <summary>
           /// Desc:专业|超市
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F站点类型 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string 派费标准 {get;set;}

           /// <summary>
           /// Desc:
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
           public string F所属网点UID {get;set;}

           /// <summary>
           /// Desc:备注
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F备注 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public bool? F禁用 {get;set;}

    }
}
