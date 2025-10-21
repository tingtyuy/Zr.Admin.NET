using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("VW_DT数据字典类别视图")]
    public partial class VW_DT数据字典类别视图
    {
           public VW_DT数据字典类别视图(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string 类别 {get;set;}

    }
}
