using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZR.WinFormsApp.models
{
    [SugarTable("Bill")]
    public class Bill
    {
        //[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        //public int Id { get; set; }


        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }

        public string UserGroup { get; set; }
    }


    [SugarTable("BillBak")]
    public class BillBak
    {
        //[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        //public int Id { get; set; }


        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public string UserName { get; set; }

        public string UserGroup { get; set; }
    }

    [SugarTable("Bill2")]
    public class Bill2
    {
        //[SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        //public int Id { get; set; }

        public string 运单编号 { get; set; }
        public DateTime 业务日期 { get; set; }
        public string 目的省份 { get; set; }

        public string 目的城市 { get; set; }

        public string 结算重量 { get; set; }

        public string 快递运费 { get; set; }

        public string 加收费用 { get; set; }
        public string 店铺账号 { get; set; }
        public string 退回状态 { get; set; }

        public string UserName { get; set; }

        public string UserGroup { get; set; }

    }
}
