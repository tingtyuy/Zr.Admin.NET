using Infrastructure.Extensions;
using Infrastructure.Helper;
using Mapster;
using Masuit.Tools;
using Masuit.Tools.Database;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.IdentityModel.Logging;
using Microsoft.VisualBasic.ApplicationServices;
using MiniExcelLibs;
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

       
    }
}
