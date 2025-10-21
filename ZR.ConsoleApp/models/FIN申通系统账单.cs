using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("FIN申通系统账单")]
    public partial class FIN申通系统账单
    {
           public FIN申通系统账单(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,IsIdentity=true)]
           public long ID {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("业务日期")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? F业务日期 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("记账日期")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? F记账日期 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("网点编号")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F网点编号 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("网点名称")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F网点名称 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("业务类型")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F业务类型 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("业务摘要")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F业务摘要 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("费用名称")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F费用名称 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("发生额(收入)")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F发生额收入 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("发生额(支出)")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F发生额支出 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("余额")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F余额 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("报表业务日期")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? F报表业务日期 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("联系方式")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F联系方式 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("银行账号")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F银行账号 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("费用科目收付类型")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F费用科目收付类型 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("关联单号")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F关联单号 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("费用科目编码")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F费用科目编码 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("账单类型")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F账单类型 {get;set;}

    }
}
