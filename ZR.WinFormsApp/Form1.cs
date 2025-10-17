using MiniExcelLibs;
namespace ZR.WinFormsApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Test();
        }

        public void Test()
        {
            var fileDir = @"D:\\123456789\\md\\运单-账单计算\\MD-2025-09-账单数据";
            var files = new List<string>();
            GetAllFiles(fileDir, files);

            foreach (var file in files)
            {
               var rows = MiniExcel.Query(file).ToList() ;
                // 获取 A1 单元格的值（第0行第0列）
                if (rows.Count > 0 && rows[0].Count > 0)
                {
                    var userName= rows[3][2];
                    var orderCount = rows[6][4];
                    var orderMoney = rows[6][4];
                    Console.WriteLine($"userName:{userName},orderCount:{orderCount},orderMoney:{orderMoney}");
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
                    files.Add(file);
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

    }
}
