using MiniExcelLibs;
using Models;
using SharpCompress.Common;
using ZR.Common;
using ZR.Common.ExcelHelper;
using ZR.Infrastructure.Extensions;
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
        /// 查询所有仓内的订单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            var result = new button4_Click_Result();
            result.RealTotalImportOrderInfos = dbHelper.db.Queryable<FIN快递出港账单_运单计算数据>().Select(s => s.F运单编号).ToList();


            var info = new Info();
            //dbHelper.db.Queryable<in2509三帆商贸账单明细>();
        }

        public class button4_Click_Result
        {
            public List<Info> Infos { get; set; }
            public int RealTotalImportOrderCount => RealTotalImportOrderInfos.Count;
            public List<string> RealTotalImportOrderInfos { get; set; }
            public int TotalMdOrderCount => Infos.Sum(i => i.MdOrderCount);
            public int TotalImportOrderCount => Infos.Sum(i => i.ImportOrderCount);
            public bool IsAllSame => TotalMdOrderCount == TotalImportOrderCount;

        }

        public class Info
        {
            public string UserName { get; set; }
            public int MdOrderCount { get; set; }
            public int ImportOrderCount { get; set; }
            public bool IsSame => MdOrderCount == ImportOrderCount;
            List<string> MdShortInfos { get; set; }
            List<string> ImportShortInfos { get; set; }
        }
        /// <summary>
        /// 导入账单数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
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

                    var npoiExcelHelper = new ExcelHelper(file.FullName);

                    // 1. 确定sheet
                    var sheet = npoiExcelHelper.GetSheet(new string[] { "快递费", "账单明细", "账单明细总", "申通" });
                    if (sheet is null)
                    {
                        logHelper.Logger.Error($"未找到指定工作表：{file.FullName}");
                        continue;
                    }
                    // 2. 拿到表头 和 表名

                    var tableColumns = npoiExcelHelper.GetFirstRowAsStringArray(sheet).Select(s => s.Value).ToArray();
                    var tableName = $"{file.DirectoryName}{Path.GetFileNameWithoutExtension(file.FullName).FilterSpecial()}";
                }

            }
        }
    }
}
