using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("CRM平台店铺账号")]
    public partial class CRM平台店铺账号
    {
           public CRM平台店铺账号(){


           }
           /// <summary>
           /// Desc:店铺名称
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string F店铺账号 {get;set;}

           /// <summary>
           /// Desc:所在平台
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F所属平台 {get;set;}

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
           /// Nullable:False
           /// </summary>           
           public bool F是否共享店铺 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F备注 {get;set;}

    }
}
