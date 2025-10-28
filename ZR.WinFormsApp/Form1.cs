using Infrastructure.Extensions;
using Infrastructure.Helper;
using Mapster;
using Masuit.Tools;
using Masuit.Tools.Database;
using Microsoft.VisualBasic.ApplicationServices;
using MiniExcelLibs;
using Models;
using Serilog.Events;
using SharpCompress.Common;
using SqlSugar;
using System.Data;
using ZR.Common;
using ZR.Common.ExcelHelper;
using ZR.WinFormsApp.models;
using static NPOI.SS.Formula.Functions.Countif;
namespace ZR.WinFormsApp
{
    public partial class Form1 : Form
    {
        public readonly DbHelper dbHelper;
        public readonly LogHelper logHelper;
        public Form1()
        {
            InitializeComponent();
            dbHelper = new DbHelper();
            logHelper = new LogHelper();
        }
        /// <summary>
        /// 选择目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.Description = "请选择文件夹";
            //folderBrowserDialog1.RootFolder = @"D:\\123456789\\md\\运单-账单计算\\MD-2025-09-账单数据"; 
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog1.SelectedPath;
                //MessageBox.Show("您选择的文件夹路径是: " + selectedPath);
                //Test(selectedPath);
            }



        }

        public void Test(string selectedPath)
        {
            var files = new List<string>();
            GetAllFiles(selectedPath, files);

            foreach (var file in files)
            {
                var rows = MiniExcel.Query(file).ToList();
                // 获取 A1 单元格的值（第0行第0列）
                if (rows.Count > 6)
                {
                    var userName = rows[2].B;
                    var orderCount = rows[5].D;
                    var orderMoney = rows[5].F;
                    //listView1.Items.Add(new ListViewItem($"userName:{userName},orderCount:{orderCount},orderMoney:{orderMoney}"));
                }
            }

        }
        //递归遍历文件夹
        public void GetAllFiles(string path, List<string> files)
        {
            try
            {
                foreach (string file in Directory.GetFiles(path))
                {
                    if (file.Contains("xlsx"))
                    {
                        files.Add(file);
                    }
                }
                foreach (string directory in Directory.GetDirectories(path))
                {
                    GetAllFiles(directory, files);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing path {path}: {ex.Message}");
            }
        }
        /// <summary>
        /// 总数核对（停止）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var result = new button4_Click_Result();
            result.RealTotalImportOrderInfos = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>().Distinct().Select(s => s.F运单编号).ToList();
            result.RealTotalMdOrderInfos = dbHelper.db.Queryable<BillBak>().Distinct().Select(s => s.OrderNo).ToList();
            result.ImportShortInfos = result.RealTotalImportOrderInfos.Except(result.RealTotalMdOrderInfos).ToList();
            result.MdShortInfos = result.RealTotalMdOrderInfos.Except(result.RealTotalMdOrderInfos).ToList();


            var list = dbHelper.db.Queryable<BillBak>().Distinct()
                  .LeftJoin<FIN快递出港账单_运单计算数据>((b, f) => b.OrderNo == f.F运单编号)
                  .Where(b => b.OrderDate >= new DateTime(2025, 9, 5))
                  .Select((b, f) => new InfoList() { UserName = b.UserName, UserGroup = b.UserGroup, OrderNo = b.OrderNo, F运单编号 = f.F运单编号 })
                  .ToList();
            //result.Infos= list.GroupBy(g => g.UserName).Select(s => new Info()
            //{
            //    UserName = s.Key,
            //    MdOrders = s.Select(x => x.OrderNo).ToList(),
            //    ImportOrders = s.Select(x => x.F运单编号).ToList(),
            //    ImportShortInfos = s.Select(x => x.OrderNo).ToList().Except(s.Select(x => x.F运单编号).ToList()).ToList()
            //}).ToList();


        }



        public class InfoList
        {

            public string OrderNo { get; set; }
            public string UserName { get; set; }

            public string UserGroup { get; set; }

            public string F运单编号 { get; set; }

        }

        public class button4_Click_Result
        {
            //public List<Info> Infos { get; set; }
            public int RealTotalImportOrderCount => RealTotalImportOrderInfos.Count;
            public int RealTotalMdOrderCount => RealTotalMdOrderInfos.Count;

            public List<string> RealTotalImportOrderInfos { get; set; }
            public List<string> RealTotalMdOrderInfos { get; set; }
            //public int TotalMdOrderCount => Infos.Sum(i => i.MdOrders.Count);
            //public int TotalImportOrderCount => Infos.Sum(i => i.ImportOrders.Count);
            //public bool IsAllSame => TotalMdOrderCount == TotalImportOrderCount;

            public List<string> ImportShortInfos { get; set; } = new List<string>();
            public List<string> MdShortInfos { get; set; } = new List<string>();

            public int ImportShortInfosCount => ImportShortInfos.Count;
            public int MdShortInfosCount => MdShortInfos.Count;


        }

        public class Info
        {
            public string UserName { get; set; }
            public List<string> MdOrders { get; set; } = new List<string>();
            public List<string> ImportOrders { get; set; } = new List<string>();
            public bool IsSame => MdOrders.Count == ImportOrders.Count;
            //List<string> MdShortInfos { get; set; } 
            public List<string> ImportShortInfos { get; set; } = new List<string>();


        }


        /// <summary>
        /// 导入账单订单号数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {

            //var directoryPaths = new List<string> {
            //    @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单"
            //  , @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\仓里账单" };


            var directoryPaths = new List<string> {
                @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单3"
            };
            //遍历目录
            foreach (var directoryPath in directoryPaths)
            {

                var directoryFiles = new DirectoryInfo(directoryPath).GetFiles("*.xlsx", SearchOption.AllDirectories);

                //遍历文件
                foreach (var file in directoryFiles)
                {
                    bool flowControl = FillBill(file);
                    //bool flowControl = FillBill2(file);
                    if (!flowControl)
                    {
                        continue;
                    }

                }

                logHelper.Logger.Error($"导入完成：{directoryPath}");

            }
            logHelper.Logger.Error($"全部导入完成");
        }
        /// <summary>
        /// dynamic再转list导入完整账单数据（速度快，处理大数据）
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool FillBill3(FileInfo file)
        {

            var maybeSheets = new List<string>()
            {
                "快递费", "账单明细", "账单明细总", "申通",
            };
            //var maybeSheets = new List<string>()
            //{
            //    "3-5.5公斤中货", "重货六部","定州四部"
            //};

            //var maybeSheets = new List<string>()
            //{
            //    "小胖哥优选",
            //};

            var selectColumns = new List<SelectColumn>();
            selectColumns.AddRange(
             new SelectColumn()
             {
                 Name = "运单编号",
                 MaybeName = new string[] { "运单编号", "运单号", "物流编号", "物流单号", "快递单号" }
             }
             ,
             new SelectColumn()
             {
                 Name = "业务日期",
                 MaybeName = new string[] { "业务时间", "业务日期", "打单时间", "发货时间"}
             },
             new SelectColumn()
             {
                 Name = "目的省份",
                 MaybeName = new string[] { "目的省份", "省份", }
             }
              ,
             new SelectColumn()
             {
                 Name = "目的城市",
                 MaybeName = new string[] { "目的城市", "城市", }
             }
              ,
             new SelectColumn()
             {
                 Name = "结算重量",
                 MaybeName = new string[] { "结算重量", "重量", }
             }
              ,
             new SelectColumn()
             {
                 Name = "业务日期",
                 MaybeName = new string[] { "业务时间", "业务日期", }
             }
              ,
             new SelectColumn()
             {
                 Name = "快递运费",
                 MaybeName = new string[] { "快递运费", "结算金额", "金额", "费用" }
             }
              ,
             new SelectColumn()
             {
                 Name = "加收费用",
                 MaybeName = new string[] { "加收费用", "加收", }
             }
                 ,
             new SelectColumn()
             {
                 Name = "店铺账号",
                 MaybeName = new string[] { "店铺账号", "店铺", "店铺名称" }
             }
                 ,
             new SelectColumn()
             {
                 Name = "退回状态",
                 MaybeName = new string[] { "退回状态", "状态", }
             }
            );
            var list = ExcelHelper.GetDynamicData(file.FullName, maybeSheets, selectColumns);
            var listBill2 = list.Adapt<List<Bill2>>();
            listBill2.ForEach(bill =>
            {
                var UserName = file.Name.FilterSpecial();
                var UserGroup = file.DirectoryName?.Split(Path.DirectorySeparatorChar).Last() ?? "";
                bill.UserName = UserName;
                bill.UserGroup = UserGroup;
            });

            try
            {
                //              dbHelper.db.Insertable(list)
                //.AS("Bill2")
                //.ExecuteCommand();
                dbHelper.db.Insertable<Bill2>(listBill2).PageSize(50000).ExecuteCommand();

                //var count = dbHelper.db.Fastest<Bill2>().AS("Bill2").PageSize(50000).BulkCopy(listBill2);
                logHelper.Logger.Information($"表 {file.FullName} 数据更新成功！");
                file.MoveTo(file.FullName + ".bak");
            }
            catch (Exception ex)
            {

                logHelper.Logger.Error($"插入数据失败：{file.FullName}=》{ex.Message}");
            }

            return true;
        }
        /// <summary>
        /// datatable方式导入完整账单数据(数据量一大会卡死)（可以弃用）
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool FillBill2(FileInfo file)
        {
            var npoiExcelHelper = new ExcelHelper(file.FullName);


            // 1. 确定sheet
            var sheet = npoiExcelHelper.GetSheet(new string[] { "快递费", "账单明细", "账单明细总", "申通" });
            if (sheet is null)
            {
                logHelper.Logger.Error($"未找到指定工作表：{file.Name}");
                return false;
            }
            // 2. 确定要导入的列名

            var selectColumns = new List<SelectColumn>();
            selectColumns.AddRange(
             new SelectColumn()
             {
                 Name = "运单编号",
                 MaybeName = new string[] { "运单编号", "运单号" }
             }
             ,
             new SelectColumn()
             {
                 Name = "业务日期",
                 MaybeName = new string[] { "业务时间", "业务日期", }
             },
             new SelectColumn()
             {
                 Name = "目的省份",
                 MaybeName = new string[] { "目的省份", "省份", }
             }
              ,
             new SelectColumn()
             {
                 Name = "目的城市",
                 MaybeName = new string[] { "目的城市", "城市", }
             }
              ,
             new SelectColumn()
             {
                 Name = "结算重量",
                 MaybeName = new string[] { "结算重量", "重量", }
             }
              ,
             new SelectColumn()
             {
                 Name = "业务日期",
                 MaybeName = new string[] { "业务时间", "业务日期", }
             }
              ,
             new SelectColumn()
             {
                 Name = "快递运费",
                 MaybeName = new string[] { "快递运费", "结算金额", "金额" }
             }
              ,
             new SelectColumn()
             {
                 Name = "加收费用",
                 MaybeName = new string[] { "加收费用", "加收", }
             }
                 ,
             new SelectColumn()
             {
                 Name = "店铺账号",
                 MaybeName = new string[] { "店铺账号", "店铺", }
             }
                 ,
             new SelectColumn()
             {
                 Name = "退回状态",
                 MaybeName = new string[] { "退回状态", "状态", }
             }
            );

            // 3. 读取数据

            var tableData = new System.Data.DataTable();

            try
            {

                tableData = npoiExcelHelper.GetTableData(sheet, selectColumns);

                logHelper.Logger.Error($"读取到成功：{file.Name}=》{tableData.Rows.Count}");
            }
            catch (Exception ex)
            {

                logHelper.Logger.Error($"读取数据失败：{file.Name}=》{ex.Message}");
            }
            // 转换数据

            var listBill = new List<Bill2>();

            var listTemp = tableData.ToList<TableColumn2>();
            var UserName = file.Name.FilterSpecial();
            var UserGroup = file.DirectoryName?.Split(Path.DirectorySeparatorChar).Last() ?? "";
            foreach (var item in listTemp)
            {
                var bill = new Bill2();
                bill.运单编号 = item.运单编号;
                bill.运单编号 = item.运单编号;
                bill.目的省份 = item.目的省份;
                bill.目的城市 = item.目的城市;
                bill.结算重量 = item.结算重量;
                bill.快递运费 = item.快递运费;
                bill.加收费用 = item.加收费用;
                bill.店铺账号 = item.店铺账号;
                bill.退回状态 = item.退回状态;

                //if (DateTime.TryParse(item.业务日期, out DateTime businessDate))
                //{
                //    bill.业务日期 = businessDate;
                //}
                bill.UserName = UserName;
                bill.UserGroup = UserGroup;
                listBill.Add(bill);
            }

            // 4. 插入数据

            try
            {
                dbHelper.db.Insertable(listBill).ExecuteCommand();

                //var count =   dbHelper.db.Fastest<Bill>().PageSize(5000).BulkCopy(listBill);
                logHelper.Logger.Information($"表 {UserName} 数据更新成功！");
            }
            catch (Exception ex)
            {

                logHelper.Logger.Error($"插入数据失败：{file.FullName}=》{ex.Message}");
            }

            return true;
        }
        /// <summary>
        /// 导入账单订单号数据
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private bool FillBill(FileInfo file)
        {
            var npoiExcelHelper = new ExcelHelper(file.FullName);


            // 1. 确定sheet
            var sheet = npoiExcelHelper.GetSheet(new string[] { "快递费", "账单明细", "账单明细总", "申通" });
            if (sheet is null)
            {
                logHelper.Logger.Error($"未找到指定工作表：{file.Name}");
                return false;
            }
            // 2. 确定要导入的列名

            var selectColumns = new List<SelectColumn>();
            selectColumns.AddRange(
             new SelectColumn()
             {
                 Name = "运单编号",
                 MaybeName = new string[] { "运单编号", "运单号" }
             }
             ,
             new SelectColumn()
             {
                 Name = "业务日期",
                 MaybeName = new string[] { "业务时间", "业务日期", }
             }
            );

            // 3. 读取数据

            var tableData = new System.Data.DataTable();

            try
            {

                tableData = npoiExcelHelper.GetTableData(sheet, selectColumns);

                logHelper.Logger.Error($"读取到成功：{file.Name}=》{tableData.Rows.Count}");
            }
            catch (Exception ex)
            {

                logHelper.Logger.Error($"读取数据失败：{file.Name}=》{ex.Message}");
            }
            // 转换数据

            var listBill = new List<Bill>();

            var listTemp = tableData.ToList<TableColumn>();
            var UserName = file.Name.FilterSpecial();
            var UserGroup = file.DirectoryName?.Split(Path.DirectorySeparatorChar).Last() ?? "";
            foreach (var item in listTemp)
            {
                var bill = new Bill();
                bill.OrderNo = item.运单编号;
                if (DateTime.TryParse(item.业务日期, out DateTime businessDate))
                {
                    bill.OrderDate = businessDate;
                }
                bill.UserName = UserName;
                bill.UserGroup = UserGroup;
                listBill.Add(bill);
            }

            // 4. 插入数据

            try
            {
                dbHelper.db.Insertable(listBill).ExecuteCommand();

                file.MoveTo(file.FullName + ".bak");

                //var count =   dbHelper.db.Fastest<Bill>().PageSize(5000).BulkCopy(listBill);
                logHelper.Logger.Information($"表 {UserName} 数据更新成功！");
            }
            catch (Exception ex)
            {

                logHelper.Logger.Error($"插入数据失败：{file.FullName}=》{ex.Message}");
            }

            return true;
        }

        class TableColumn
        {
            public string 运单编号 { get; set; }
            public string 业务日期 { get; set; }
        }

        class TableColumn2
        {


            public string 运单编号 { get; set; }
            public string 业务日期 { get; set; }
            public string 目的省份 { get; set; }

            public string 目的城市 { get; set; }

            public string 结算重量 { get; set; }

            public string 快递运费 { get; set; }

            public string 加收费用 { get; set; }
            public string 店铺账号 { get; set; }
            public string 退回状态 { get; set; }
        }
        /// <summary>
        /// 导入账单完整数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            var directoryPaths = new List<string> {
                @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单"
              , @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\仓里账单" };
            //遍历目录
            foreach (var directoryPath in directoryPaths)
            {

                var directoryFiles = new DirectoryInfo(directoryPath).GetFiles("*.xlsx", SearchOption.AllDirectories);

                //遍历文件
                foreach (var file in directoryFiles)
                {
                    //bool flowControl = FillBill(file);
                    bool flowControl = FillBill3(file);
                    if (!flowControl)
                    {
                        continue;
                    }

                }

                logHelper.Logger.Information($"导入完成：{directoryPath}");

            }
            logHelper.Logger.Information($"全部导入完成");
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 生成表Bill2结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click_1(object sender, EventArgs e)
        {
            dbHelper.db.CodeFirst.InitTables(typeof(Bill2));
        }
        /// <summary>
        /// 对比总运单量
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
        {
            //美达全部客户的总运单量对比
            var billList = dbHelper.db.Queryable<Bill2>().Where(w => w.UserGroup == "揽收账单");





        }
        /// <summary>
        /// 查看缺失的网点公司信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            var importList = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>().GroupBy(s => s.F所属网点).Select(s => s.F所属网点).ToList();

            var wdList = dbHelper.db.Queryable<BU网点公司>().Select(s => s.F网点全称).ToList();

            var shortList = importList.Except(wdList).ToList();

            leftBox.Text = string.Join(Environment.NewLine, shortList);

        }
        /// <summary>
        /// 给所有客户添加全部的共享店铺
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 没有计算的运单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            var list = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
                  .Where(w => w.F计算状态 == 1)
                   .WithCache(60 * 30)
                  .ToList();

            var take10 = list.Select(s => s.F运单编号).Take(10);
            leftBox.Text = string.Join(Environment.NewLine, "取出10条");
            leftBox.Text += string.Join(Environment.NewLine, take10);

            rightBox.Text = string.Join(Environment.NewLine, $"全部数据{list.Count()}");

        }
        /// <summary>
        /// 且没有发运表的运单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            var list = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
            .LeftJoin<FIN发运表>((j, f) => f.F运单编号 == j.F运单编号)
            .Where((j, f) => j.F计算状态 == -1 && string.IsNullOrEmpty(f.F运单编号))
            .WithCache(60 * 30)
            .ToList();
            var take10 = list.Select(s => s.F运单编号).Take(10);

            leftBox.Text = string.Join(Environment.NewLine, "取出10条");
            leftBox.Text += string.Join(Environment.NewLine, take10);

            rightBox.Text = string.Join(Environment.NewLine, $"全部数据{list.Count()}");

        }
    }
}
