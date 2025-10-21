using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("BU网点公司")]
    public partial class BU网点公司
    {
           public BU网点公司(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string FUID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string F网点简称 {get;set;}

           /// <summary>
           /// Desc:快递公司名称
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F网点全称 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F实体公司 {get;set;}

           /// <summary>
           /// Desc:所属快递品牌
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F快递品牌 {get;set;}

           /// <summary>
           /// Desc:联系方式
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F联系方式 {get;set;}

           /// <summary>
           /// Desc:描述
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F备注 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public int? F排序 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public bool? F禁用 {get;set;}

    }
}
