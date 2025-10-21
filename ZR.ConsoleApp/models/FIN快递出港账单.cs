using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Models
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("FIN快递出港账单")]
    public partial class FIN快递出港账单
    {
           public FIN快递出港账单(){


           }
           /// <summary>
           /// Desc:[ExcelColumnName("运单号")]
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string F运单编号 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("所属网点")]
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public string F所属网点 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F所属网点UID {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("订单/面单网点")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F面单网点 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? F业务日期 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("业务时间")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? F业务时间 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("订单客户")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F店铺账号 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F共享别名 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F订单重量 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("结算对象")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F结算对象 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("结算对象编号")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F结算对象编号 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("结算类型")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F结算类型 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("网点称重")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F网点重量 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F集包重量 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("体积重")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F计泡重量 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("中心称重")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F中心重量 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("重量")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F总部重量 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("结算重量")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F结算重量 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F代收货款 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F退回状态 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F退回费用 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("寄件人")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F寄件人 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("始发省份")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F寄件省份 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("始发城市")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F寄件城市 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("寄件地址")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F寄件地址 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("收件人")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F收件人 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("收件地址")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F收件地址 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("目的省份")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F目的省份 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public long? F目的地Flag {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("目的城市")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F目的城市 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F声明价值 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("物品类别")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F物品类别 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F寄递物品 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("收件网点")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F收件网点 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("集包网点")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F集包网点 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("出港中心")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F出港中心 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("中心下一站")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F目的网点 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("签收网点")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F签收网点 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("签收时间")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? F签收时间 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("送货上门")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F送货上门 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("长宽高")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F长宽高 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F总部面单费 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F总部中转费 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F总部加收费 {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("面单来源")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F面单来源 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F客户UID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F客户业务关系ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public decimal F客户应收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F客户运费金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F客户加收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:True
           /// </summary>           
           public decimal? F客户结算重量 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F客户取重方式 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F客户报价ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int F客户计算状态 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F代理UID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F代理业务关系ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F代理应收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F代理运费金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F代理加收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F代理取重方式 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F代理结算重量 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F代理报价ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int F代理计算状态 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F承包区UID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F承包区业务关系ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F承包区应收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F承包区运费金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F承包区加收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F承包区取重方式 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F承包区结算重量 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F承包区报价ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int F承包区计算状态 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F业务员UID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F业务员业务关系ID {get;set;}

           /// <summary>
           /// Desc:[ExcelColumnName("收件员")]
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F业务员 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F业务员应收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F业务员运费金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F业务员加收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F业务员取重方式 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F业务员结算重量 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F业务员报价ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int F业务员计算状态 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F收件网点UID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F收件网点业务关系ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F收件网点应收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F收件网点运费金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F收件网点加收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F收件网点取重方式 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F收件网点结算重量 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F收件网点报价ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int F收件网点计算状态 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F驿站UID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F驿站业务关系ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F驿站应收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F驿站运费金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public decimal F驿站加收金额 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F驿站取重方式 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public decimal? F驿站结算重量 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F驿站报价ID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:0
           /// Nullable:False
           /// </summary>           
           public int F驿站计算状态 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int F账单状态 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F审核备注 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? F审核员UID {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string F审核员 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? F审核日期 {get;set;}

    }
}
