using Infrastructure.Extensions;
using Infrastructure.Helper;
using Mapster;
using Masuit.Tools;
using Masuit.Tools.Database;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Logging;
using Microsoft.VisualBasic.ApplicationServices;
using MiniExcelLibs;
using RasterEdge.Imaging.Basic;
using RasterEdge.XDoc.Word;
using Serilog.Events;
using SharpCompress.Common;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SqlSugar;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ZR.Common;
using ZR.Common.ExcelHelper;
using ZR.Infrastructure.Images;
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
        /// <summary>
        /// Select Word
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            // 1. 加载 Word 文档
            string filePath = @"";
            //string filePath = @"C:\Users\ms363\Desktop\合同审批\石家庄-合同\早鸟申通高新合同 .docx";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog1.FileName;
                lbWordInputPath.Text = filePath;
            }

        }
        /// <summary>
        /// Word To Image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

            DOCXDocument doc = new DOCXDocument(lbWordInputPath.Text);
            var outputDir = Path.Combine(Path.GetDirectoryName(lbWordInputPath.Text), "output");
            doc.ConvertToImages(ImageType.PNG, outputDir, "page");
            MessageBox.Show("转换完成");
        }
        /// <summary>
        /// Merge Image To 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void button4_ClickAsync(object sender, EventArgs e)
        {
            var btn = sender as Button;
            btn.Enabled = false;
            btn.Text = "处理中...";
            // 使用示例
            string folderPath = @"C:\output\";
            string outputPath = @"C:\output\merged\long_image.png";

            await ImageMerger.MergeImagesVerticallyAsync(folderPath, outputPath);

            btn.Enabled = true;
            btn.Text = "Merge Image To 1";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            lbWordInputPath.Text = folderBrowserDialog1.InitialDirectory;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void excelToolStripMenuItem_Click(object sender, EventArgs e)
        {
             
        }

        private void tempToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
