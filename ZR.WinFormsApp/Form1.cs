using Infrastructure.Extensions;
using Infrastructure.Helper;
using Mapster;
using Masuit.Tools;
using Masuit.Tools.Database;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Logging;
using Microsoft.VisualBasic.ApplicationServices;
using MiniExcelLibs;
using Models;
using Serilog.Events;
using SharpCompress.Common;
using SqlSugar;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZR.Common;
using ZR.Common.ExcelHelper;
using ZR.WinFormsApp.models;
using static NPOI.SS.Formula.Functions.Countif;
namespace ZR.WinFormsApp
{
    public partial class Form1 : Form
    {
        public readonly DbHelper dbHelper;
        public readonly Common.LogHelper logHelper;
        public Form1()
        {
            InitializeComponent();
            dbHelper = new DbHelper();
            logHelper = new Common.LogHelper(false);
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
        private bool FillBill4(FileInfo file)
        {

            var maybeSheets = new List<string>()
            {
                "运单明细_1"
            };

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
                Name = "所属网点",
                MaybeName = new string[] { "所属网点" }
            }

             ,
             new SelectColumn()
             {
                 Name = "业务日期",
                 MaybeName = new string[] { "业务时间", "业务日期", "打单时间", "发货时间" }
             },
             new SelectColumn()
             {
                 Name = "目的省份",
                 MaybeName = new string[] { "目的省份", "省份", "目的份" }
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
                 Name = "结算价格",
                 MaybeName = new string[] { "结算价格" }
             }
              ,

             new SelectColumn()
             {
                 Name = "退回状态",
                 MaybeName = new string[] { "退回状态", "状态", "退件状态" }
             }
                       ,

             new SelectColumn()
             {
                 Name = "退回费用",
                 MaybeName = new string[] { "退回费用", "状态", "退件状态" }
             }
            );
            var list = ExcelHelper.GetDynamicData(file.FullName, maybeSheets, selectColumns);
            var listBill2 = list.Adapt<List<Bill3>>();
            listBill2.ForEach(bill =>
            {
                var UserName = Path.GetFileNameWithoutExtension(file.Name).FilterSpecial();
                //var UserGroup = file.DirectoryName?.Split(Path.DirectorySeparatorChar).Last() ?? "";
                bill.UserName = UserName;
                //bill.UserGroup = UserGroup;
            });

            try
            {
                //              dbHelper.db.Insertable(list)
                //.AS("Bill2")
                //.ExecuteCommand();
                dbHelper.db.Storageable<Bill3>(listBill2).PageSize(2000).ExecuteCommand();

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

        private async Task<bool> FillBill3(FileInfo file)
        {

            var maybeSheets = new List<string>()
            {
                "快递费", "账单明细", "账单明细总", "申通", "3-5.5公斤中货", "重货六部","定州四部","小胖哥优选"
            };

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
                 MaybeName = new string[] { "业务时间", "业务日期", "打单时间", "发货时间" }
             },
             new SelectColumn()
             {
                 Name = "目的省份",
                 MaybeName = new string[] { "目的省份", "省份", "目的份", "省" }
             }
              ,
             new SelectColumn()
             {
                 Name = "目的城市",
                 MaybeName = new string[] { "目的城市", "城市", "市" }
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
                 MaybeName = new string[] { "快递运费", "结算金额", "金额", "费用", "快递费", "基础运费", "成本" }
             }
              ,
             new SelectColumn()
             {
                 Name = "加收费用",
                 MaybeName = new string[] { "加收费用", "加收", "加收费", "加收运费" }
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
                 MaybeName = new string[] { "退回状态", "状态", "退件状态" }
             }
            );
            var list = ExcelHelper.GetDynamicData(file.FullName, maybeSheets, selectColumns);
            var listBill2 = list.Adapt<List<Bill2>>().Where(w => !string.IsNullOrEmpty(w.运单编号)).ToList(); //去除空运单号的数据

            var AnyDulication = listBill2.GroupBy(g => new { g.运单编号 }).Any(g => g.Count() > 1);
            if (AnyDulication)
            {
                logHelper.Logger.Error($"表 {file.FullName} 主键有重复！");
                return false;
            }
            listBill2.ForEach(bill =>
            {
                var UserName = file.Name.FilterSpecial().Replace("xlsx", "");
                var UserGroup = file.DirectoryName?.Split(Path.DirectorySeparatorChar).Last() ?? "";
                bill.UserName = UserName;
                bill.UserGroup = UserGroup;
            });


            try
            {

                await Task.Run(() =>
                {
                    var ss = dbHelper.db.Fastest<Bill2>().PageSize(10000).BulkMerge(listBill2);
                    logHelper.Logger.Information($"表 {file.FullName} 数据更新成功{ss}条！");
                    file.MoveTo(file.FullName.Replace(".xlsx", "") + ".bak"); //防止重复导入
                });
            }
            catch (Exception ex)
            {

                logHelper.Logger.Error($"插入数据失败：{file.FullName}=》{ex.Message}");
            }
            MessageBox.Show("ok");
            return true;
        }

        /// <summary>
        /// 导入账单完整数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button6_Click(object sender, EventArgs e)
        {
            //dbHelper.db.Deleteable<Bill2>().ExecuteCommand();
            var directoryPaths = new List<string> {
                 @"C:\Users\ms363\Desktop\11月对账\xiaoqian\仓里",
                      @"C:\Users\ms363\Desktop\11月对账\xiaoqian\揽收" };

            //遍历目录
            foreach (var directoryPath in directoryPaths)
            {

                var directoryFiles = new DirectoryInfo(directoryPath).GetFiles("*.xlsx", SearchOption.AllDirectories);

                //遍历文件
                foreach (var file in directoryFiles)
                {
                    //bool flowControl = FillBill(file);
                    bool flowControl = await FillBill3(file);
                    if (!flowControl)
                    {
                        continue;
                    }

                }

                logHelper.Logger.Information($"导入完成：{directoryPath}");

            }
            logHelper.Logger.Information($"全部导入完成");
        }

        /// <summary>
        /// 生成表Bill2结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click_1(object sender, EventArgs e)
        {
            dbHelper.db.DbMaintenance.DropTable(typeof(Bill2));
            dbHelper.db.CodeFirst.InitTables(typeof(Bill2));
            MessageBox.Show("ok");
        }

        /// <summary>
        /// 没计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            var total = 0;
            var list = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
                  .Where(w => w.F计算状态 == 1)
                  .Select(s => s.F运单编号)
                  .ToPageList(0, 10, ref total);

            leftBox.Text = string.Join(Environment.NewLine, $"全部数据{total}\n");
            leftBox.Text += string.Join(Environment.NewLine, "取出10条\n");
            leftBox.Text += string.Join(Environment.NewLine, list);

        }

        /// <summary>
        /// 导入2.0账单完整数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            dbHelper.db.Deleteable<Bill3>().ExecuteCommand();
            var directoryPaths = new List<string> {
                @"D:\123456789\md\运单-账单计算\2.0账单"
         };
            //遍历目录
            foreach (var directoryPath in directoryPaths)
            {

                var directoryFiles = new DirectoryInfo(directoryPath).GetFiles("*.xlsx", SearchOption.AllDirectories);

                //遍历文件
                foreach (var file in directoryFiles)
                {
                    //bool flowControl = FillBill(file);
                    bool flowControl = FillBill4(file);
                    if (!flowControl)
                    {
                        continue;
                    }

                }

                logHelper.Logger.Information($"导入完成：{directoryPath}");

            }
            logHelper.Logger.Information($"全部导入完成");
        }
        /// <summary>
        /// 生成表Bill3结构
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button18_Click(object sender, EventArgs e)
        {

            dbHelper.db.CodeFirst.InitTables(typeof(Bill3));
            dbHelper.db.Deleteable<Bill3>().ExecuteCommand();

        }

        /// <summary>
        /// bak改为xlsx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            //dbHelper.db.Deleteable<Bill2>().ExecuteCommand();
            //var directoryPaths = new List<string> {
            //    @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单"
            //  , @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\仓里账单" };


            var directoryPaths = new List<string> {
                @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单"
         };
            //遍历目录
            foreach (var directoryPath in directoryPaths)
            {

                var directoryFiles = new DirectoryInfo(directoryPath).GetFiles("*.bak", SearchOption.AllDirectories);

                //遍历文件
                foreach (var file in directoryFiles)
                {

                    file.MoveTo(file.FullName.Replace(".bak", "") + ".xlsx"); //防止重复导入
                }

                //logHelper.Logger.Information($"导入完成：{directoryPath}");

            }
            //logHelper.Logger.Information($"全部导入完成");
            MessageBox.Show("ok");
        }
    }
}
