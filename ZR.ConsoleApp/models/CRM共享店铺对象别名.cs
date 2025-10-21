using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("CRM共享店铺对象别名")]
    public partial class CRM共享店铺对象别名
    {
           public CRM共享店铺对象别名(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string F别名 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string F对象UID {get;set;}

           /// <summary>
           /// Desc:直接客户|业务代理|业务员|收件网点|承包区|驿站
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F对象类型 {get;set;}

    }
}
