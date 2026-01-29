using Infrastructure.Extensions;
using Infrastructure.Helper;
using Mapster;
using Masuit.Tools;
using Masuit.Tools.Database;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing.Template;
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
        /// 导出太仓日报
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            var timeStamp = .GetTimeStamp(DateTime.Now, true);
            // 获取当前应用程序的 bin/Debug 或 bin/Release 目录
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            // 设置输出文件的路径
            string path = Path.Combine(basePath, "out", "太仓申通网点质控日报.xlsx");

            // 如果模板文件在项目根目录的 Templates 文件夹中
            // 需要先复制到 bin 目录，或者使用相对路径
            string templatePath = Path.Combine(basePath, "res","太仓申通网点质控日报-模板.xlsx");

            // 检查模板文件是否存在
            if (!File.Exists(templatePath))
            {
                // 如果模板文件不存在，提示或创建
                MessageBox.Show($"模板文件不存在: {templatePath}");
                return;
            }

            var value = new
            {
                Name = "Jack",
                CreateDate = new DateTime(2021, 01, 01),
                VIP = true,
                Points = 123
            };

            try
            {
                MiniExcel.SaveAsByTemplate(path, templatePath, value);
                MessageBox.Show($"文件保存成功: {path}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存文件时出错: {ex.Message}");
            }
        }
    }
}
