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
            //var directoryPaths = new List<string> {
            //    @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单"
            //  , @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\仓里账单" };


            var directoryPaths = new List<string> {
                @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单"
         };
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
        /// 差异报告
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            dbHelper.db.Deleteable<Bill10>();

            //多个用户使用了同一个店铺账号 
            NewMethod6();
            logHelper.Logger.Information("NewMethod6");
            MessageBox.Show("NewMethod6");

            //有真实店铺 ，没报价关系
            NewMethod1();
            logHelper.Logger.Information("NewMethod1");
            MessageBox.Show("NewMethod1");

            //没有店铺或者是共享店铺 ，没有发运表  ，没报价关系
            NewMethod2();
            logHelper.Logger.Information("NewMethod2");
            MessageBox.Show("NewMethod2");

            //没有店铺或者是共享店铺 ， 有发运表，没报价关系(没发运表店铺)
            NewMethod5();
            logHelper.Logger.Information("NewMethod5");
            MessageBox.Show("NewMethod5");

            //共享店铺，有发运表 ， 没报价关系(没发运表客户 和 没计算表店铺)
            NewMethod4();
            logHelper.Logger.Information("NewMethod4");
            MessageBox.Show("NewMethod4");

            //没有店铺或者是共享店铺 ，有发运表 ，没报价关系(没发运表客户 和 没发运表店铺)
            NewMethod3();
            logHelper.Logger.Information("NewMethod3");
            MessageBox.Show("NewMethod3");

            //其他错误
            NewMethod7();
            logHelper.Logger.Information("差异报告完成");
            MessageBox.Show("差异报告完成");

        }

        private void NewMethod7()
        {
            var pageIndex = 0;
            var pageSize = 50000;
            var totalCount = 0;
            do
            {
                var baseList = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
              .LeftJoin<FIN发运表>((j, f) => f.F运单编号 == j.F运单编号)
              .LeftJoin<CRM店铺业务关系>((j, f, g) => g.F店铺名称 == f.F店铺名称)
              .LeftJoin<CRM平台店铺账号>((j, f, g, d) => d.F店铺账号 == j.F店铺账号)
                    .LeftJoin<Bill10>((j, f, g, d, e) => e.运单编号 == j.F运单编号)
              .Where((j, f, g, d, e) =>
            j.F计算状态 == 1
               && string.IsNullOrEmpty(e.运单编号)
                                 ).Select((j, f, g, d, e) => new
                                 {
                                     j.F运单编号,
                                     j.F店铺账号,
                                     d.F是否共享店铺,
                                     f.F客户名,
                                     f.F店铺名称,
                                     g.F关系ID
                                 });

                var newList = baseList.Select(j => new Bill10() { 运单编号 = j.F运单编号, Remark = "没找到原因的" }).ToList();

                dbHelper.db.Insertable(newList).ExecuteCommand();
                pageIndex++;
            } while (totalCount > pageIndex * pageSize);
        }
        /// <summary>
        /// 多个用户使用了同一个店铺账号 
        /// </summary>
        private void NewMethod6()
        {
            var list = dbHelper.db.Queryable<CRM店铺业务关系>()
                 .LeftJoin<FIN快递业务报价_主表>((a, b) => a.F报价主表ID == b.FID)
                 .LeftJoin<CRM平台店铺账号>((a, b, c) => a.F店铺名称 == c.F店铺账号)
                 .Where((a, b, c) => c.F是否共享店铺 == false).ToList();

            var resultList = list.GroupBy(g => new { g.F店铺名称, g.F业务对象名称 }).Select(s => new { s.Key.F店铺名称, s.Key.F业务对象名称 })
                .GroupBy(g => g.F店铺名称).Select(g => new
                {
                    F店铺名称 = g.Key,
                    F业务对象名称 = g.ToList().Select(s => s.F业务对象名称).First(),
                    使用次数 = g.Count()
                }).Where(w => w.使用次数 > 1).OrderByDescending(O => O.使用次数).ToList();

            foreach (var item in resultList)
            {
                leftBox.Text += $"店铺名称:{item.F店铺名称} 业务对象名称:{item.F业务对象名称} 使用次数:{item.使用次数}\n";
            }
        }

        private void NewMethod5()
        {
            var pageIndex = 0;
            var pageSize = 50000;
            var totalCount = 0;
            do
            {

                ///共享店铺
                var baseList = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
                       .LeftJoin<FIN发运表>((j, f) => f.F运单编号 == j.F运单编号)
                       .LeftJoin<CRM店铺业务关系>((j, f, g) => g.F店铺名称 == f.F店铺名称)
                       .LeftJoin<CRM平台店铺账号>((j, f, g, d) => d.F店铺账号 == j.F店铺账号)

                       .Where((j, f, g, d) =>
                       j.F计算状态 == 1

                         && ((!string.IsNullOrEmpty(d.F店铺账号) && d.F是否共享店铺 == true))      //没有店铺或者是共享店铺
                         && !string.IsNullOrEmpty(f.F运单编号) // 有发运表
                         && string.IsNullOrEmpty(g.F店铺名称) // ，没报价关系
                       );

                var newList = baseList.Select((j, f, g, d) => new Bill10()
                {
                    运单编号 = j.F运单编号,
                    Remark = "有共享店铺，有发运表，发运表店铺没有设置报价关系"

                                ,
                    计算表店铺 = j.F店铺账号
                ,
                    店铺账号表店铺 = d.F店铺账号
                ,
                    发运表客户 = f.F客户名
                ,
                    发运表店铺 = f.F店铺名称
                }).ToPageList(pageIndex, pageSize, ref totalCount);

                dbHelper.db.Storageable(newList).ExecuteCommand();
                pageIndex++;
            } while (totalCount > pageIndex * pageSize);



            var pageIndex2 = 0;
            var pageSize2 = 50000;
            var totalCount2 = 0;
            do
            {
                ///没有店铺
                var baseList2 = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
              .LeftJoin<FIN发运表>((j, f) => f.F运单编号 == j.F运单编号)
              .LeftJoin<CRM店铺业务关系>((j, f, g) => g.F店铺名称 == f.F店铺名称)
              .LeftJoin<CRM平台店铺账号>((j, f, g, d) => d.F店铺账号 == j.F店铺账号)

                       .Where((j, f, g, d) =>
                       j.F计算状态 == 1

                && (string.IsNullOrEmpty(d.F店铺账号))      //没有店铺或者是共享店铺
                && !string.IsNullOrEmpty(f.F运单编号) // 有发运表
                && string.IsNullOrEmpty(g.F店铺名称) // ，没报价关系
              );

                var newList2 = baseList2.Select((j, f, g, d) => new Bill10()
                {
                    运单编号 = j.F运单编号,
                    Remark = "没有店铺，有发运表，发运表店铺没有设置报价关系"

                         ,
                    计算表店铺 = j.F店铺账号
                ,
                    店铺账号表店铺 = d.F店铺账号
                ,
                    发运表客户 = f.F客户名
                ,
                    发运表店铺 = f.F店铺名称

                }).ToPageList(pageIndex2, pageSize2, ref totalCount2);

                dbHelper.db.Storageable(newList2).ExecuteCommand();
                pageIndex2++;
            } while (totalCount2 > pageIndex2 * pageSize2);


        }

        private void NewMethod4()
        {
            var pageIndex = 0;
            var pageSize = 50000;
            var totalCount = 0;
            do
            {
                //共享店铺
                var baseList = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
                     .LeftJoin<FIN发运表>((j, f) => f.F运单编号 == j.F运单编号)
                     .LeftJoin<CRM店铺业务关系>((j, f, g) => g.F店铺名称 == j.F店铺账号 && g.F业务对象名称 == f.F客户名)
                     .LeftJoin<CRM平台店铺账号>((j, f, g, d) => d.F店铺账号 == j.F店铺账号)

                       .Where((j, f, g, d) =>
                       j.F计算状态 == 1

                       && ((!string.IsNullOrEmpty(d.F店铺账号) && d.F是否共享店铺 == true))    //共享店铺
                       && !string.IsNullOrEmpty(f.F运单编号) // 有发运表
                       && string.IsNullOrEmpty(g.F店铺名称) // ，没报价关系
                     );

                var newList = baseList.Select((j, f, g, d) => new Bill10()
                {
                    运单编号 = j.F运单编号,
                    Remark = "有共享店铺，有发运表 ，发运表客户和共享店铺没有设置报价关系"
                 ,
                    计算表店铺 = j.F店铺账号
                ,
                    店铺账号表店铺 = d.F店铺账号
                ,
                    发运表客户 = f.F客户名
                ,
                    发运表店铺 = f.F店铺名称

                }).ToPageList(pageIndex, pageSize, ref totalCount);

                dbHelper.db.Storageable(newList).ExecuteCommand();
                pageIndex++;

            } while (totalCount > pageIndex * pageSize);


        }

        private void NewMethod3()
        {
            var pageIndex = 0;
            var pageSize = 50000;
            var totalCount = 0;
            do
            {
                //共享店铺
                var baseList = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
                    .LeftJoin<FIN发运表>((j, f) => f.F运单编号 == j.F运单编号)
                    .LeftJoin<CRM店铺业务关系>((j, f, g) => g.F店铺名称 == f.F店铺名称 && g.F业务对象名称 == f.F客户名)
                    .LeftJoin<CRM平台店铺账号>((j, f, g, d) => d.F店铺账号 == j.F店铺账号)

                       .Where((j, f, g, d) =>
                       j.F计算状态 == 1

                      && ((!string.IsNullOrEmpty(d.F店铺账号) && d.F是否共享店铺 == true))      //共享店铺
                      && !string.IsNullOrEmpty(f.F运单编号) // 有发运表
                      && string.IsNullOrEmpty(g.F店铺名称) // ，没报价关系
                    );

                var newList = baseList.Select((j, f, g, d) => new Bill10()
                {
                    运单编号 = j.F运单编号,
                    Remark = "有共享店铺，有发运表 ，发运表客户和发运表店铺没有设置报价关系"
                    ,
                    计算表店铺 = j.F店铺账号
                ,
                    店铺账号表店铺 = d.F店铺账号
                ,
                    发运表客户 = f.F客户名
                ,
                    发运表店铺 = f.F店铺名称

                }).ToPageList(pageIndex, pageSize, ref totalCount);

                dbHelper.db.Storageable(newList).ExecuteCommand();
                pageIndex++;

            } while (totalCount > pageIndex * pageSize);



            var pageIndex2 = 0;
            var pageSize2 = 50000;
            var totalCount2 = 0;
            do
            {
                ///没有店铺
                var baseList2 = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
                                            .LeftJoin<FIN发运表>((j, f) => f.F运单编号 == j.F运单编号)
                                            .LeftJoin<CRM店铺业务关系>((j, f, g) => g.F店铺名称 == f.F店铺名称 && g.F业务对象名称 == f.F客户名)
                                            .LeftJoin<CRM平台店铺账号>((j, f, g, d) => d.F店铺账号 == j.F店铺账号)

                       .Where((j, f, g, d) =>
                       j.F计算状态 == 1

                                              && (string.IsNullOrEmpty(d.F店铺账号))      //没有店铺
                                              && !string.IsNullOrEmpty(f.F运单编号) // 有发运表
                                              && string.IsNullOrEmpty(g.F店铺名称) // ，没报价关系
                                            );

                var newList2 = baseList2.Select((j, f, g, d) => new Bill10()
                {
                    运单编号 = j.F运单编号,
                    Remark = "没有店铺，有发运表 ，发运表客户和发运表店铺没有设置报价关系"
                ,
                    计算表店铺 = j.F店铺账号
                ,
                    店铺账号表店铺 = d.F店铺账号
                ,
                    发运表客户 = f.F客户名
                ,
                    发运表店铺 = f.F店铺名称

                }).ToPageList(pageIndex2, pageSize2, ref totalCount2);

                dbHelper.db.Storageable(newList2).ExecuteCommand();
                pageIndex2++;

            } while (totalCount2 > pageIndex2 * pageSize2);

        }

        private void NewMethod2()
        {
            var pageIndex = 0;
            var pageSize = 50000;
            var totalCount = 0;
            do
            {
                //共享店铺
                var baseList = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
                    //.LeftJoin<CRM店铺业务关系>((j, g) => g.F店铺名称 == j.F店铺账号)
                    .LeftJoin<CRM平台店铺账号>((j, d) => d.F店铺账号 == j.F店铺账号)
                    .LeftJoin<FIN发运表>((j, d, f) => f.F运单编号 == j.F运单编号)

                    .Where((j, d, f) =>
                    j.F计算状态 == 1

                      && ((!string.IsNullOrEmpty(d.F店铺账号) && d.F是否共享店铺 == true))      //共享店铺
                      && string.IsNullOrEmpty(f.F运单编号) // 没有发运表
                                                       //&& string.IsNullOrEmpty(g.F店铺名称) // ，没报价关系
                    );

                var newList = baseList.Select((j, d, f) => new Bill10()
                {
                    运单编号 = j.F运单编号,
                    Remark = "共享店铺，没有发运表 ，共享店铺缺少发运表信息导致找不到报价关系"

                     ,
                    计算表店铺 = j.F店铺账号
                ,
                    店铺账号表店铺 = d.F店铺账号
                ,
                    发运表客户 = f.F客户名
                ,
                    发运表店铺 = f.F店铺名称
                }).ToPageList(pageIndex, pageSize, ref totalCount);

                dbHelper.db.Storageable(newList).ExecuteCommand();
                pageIndex++;

            } while (totalCount > pageIndex * pageSize);



            var pageIndex2 = 0;
            var pageSize2 = 50000;
            var totalCount2 = 0;
            do
            {
                var baseList2 = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
                       //.LeftJoin<CRM店铺业务关系>((j, g) => g.F店铺名称 == j.F店铺账号)
                       .LeftJoin<CRM平台店铺账号>((j, d) => d.F店铺账号 == j.F店铺账号)
                       .LeftJoin<FIN发运表>((j, d, f) => f.F运单编号 == j.F运单编号)

               .Where((j, d, f) =>
               j.F计算状态 == 1

                         && (string.IsNullOrEmpty(d.F店铺账号))      //没有店铺
                         && string.IsNullOrEmpty(f.F运单编号) // 没有发运表
                                                          //&& string.IsNullOrEmpty(g.F店铺名称) // ，没报价关系
                       );
                var newList2 = baseList2.Select((j, d, f) => new Bill10()
                {
                    运单编号 = j.F运单编号,
                    Remark = "没有店铺，没有发运表 ，共享店铺缺少发运表信息导致找不到报价关系"
                                             ,
                    计算表店铺 = j.F店铺账号
                ,
                    店铺账号表店铺 = d.F店铺账号
                ,
                    发运表客户 = f.F客户名
                ,
                    发运表店铺 = f.F店铺名称
                }).ToPageList(pageIndex2, pageSize2, ref totalCount2);

                dbHelper.db.Storageable(newList2).ExecuteCommand();
                pageIndex2++;

            } while (totalCount2 > pageIndex2 * pageSize2);
        }

        private void NewMethod1()
        {
            var pageIndex = 0;
            var pageSize = 50000;
            var totalCount = 0;
            do
            {
                var baseList = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
                        .LeftJoin<CRM店铺业务关系>((j, g) => g.F店铺名称 == j.F店铺账号)
                        .LeftJoin<CRM平台店铺账号>((j, g, d) => d.F店铺账号 == j.F店铺账号)
                        .Where((j, g, d) =>
                        j.F计算状态 == 1
                          && !string.IsNullOrEmpty(d.F店铺账号) // 有店铺
                          && d.F是否共享店铺 == false // 真实店铺
                          && string.IsNullOrEmpty(g.F店铺名称) // 没报价
                        );
                //var total = 0;
                //var list = baseList.Select(j => j.F运单编号).ToPageList(0, 10, ref total);
                var newList = baseList.Select((j, g, d) => new Bill10()
                {
                    运单编号 = j.F运单编号,
                    Remark = "有真实店铺 ，真实店铺没有设置报价关系"
               ,
                    计算表店铺 = j.F店铺账号,
                    店铺账号表店铺 = d.F店铺账号
                }).ToPageList(pageIndex, pageSize, ref totalCount);

                dbHelper.db.Storageable(newList).ExecuteCommand();

                pageIndex++;

            } while (totalCount > pageIndex * pageSize);

        }

        /// <summary>
        /// 生成表Bill10
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            dbHelper.db.CodeFirst.InitTables(typeof(Bill10));
            dbHelper.db.Deleteable<Bill10>().ExecuteCommand();
            logHelper.Logger.Information($"表Bill10生成完成");
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
        /// 多个用户使用了同一个店铺账号 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            this.NewMethod6();
        }
        /// <summary>
        /// 查询计算状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var groupList = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>()
            .GroupBy(s => s.F计算状态).Select(s => new
            {
                计算状态 = s.F计算状态,
                数量 = SqlFunc.AggregateCount(s.F运单编号)
            })
            .ToList();

            foreach (var item in groupList)
            {
                leftBox.Text += $"运单计算数据=>计算状态:{item.计算状态};数量{item.数量}\n";
            }

            var list2All = dbHelper.db.Queryable<FIN快递出港账单_结算对象价格>().Select(s => s.F运单编号).Count();
            var list2 = dbHelper.db.Queryable<FIN快递出港账单_结算对象价格>().Select(s => s.F运单编号).Distinct().Count();

            leftBox2.Text = $"结算对象价格=>全部数据量:{list2All}\n";
            leftBox2.Text += $"结算对象价格=>不重复数据量:{list2}\n";




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
