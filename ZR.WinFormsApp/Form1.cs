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
using MiniExcelLibs.OpenXml;
using NPOI.SS.UserModel;
using Serilog.Events;
using SharpCompress.Common;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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
            var timeStamp = DateTime.Now.ToString("yyyyMMddhhmmsss");
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.Combine(basePath, "out", $"太仓申通网点质控日报{timeStamp}.xlsx");
            string templatePath = Path.Combine(basePath, "res", "太仓申通网点质控日报-模板.xlsx");
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));
            }
            if (!File.Exists(templatePath))
            {
                MessageBox.Show($"模板文件不存在: {templatePath}");
                return;
            }

            var value = new
            {
                Name = "Jack",

            };

            try
            {
                var config = new OpenXmlConfiguration()
                {
                
                };
                MiniExcel.SaveAsByTemplate(path, templatePath, value, config);
                MessageBox.Show($"文件保存成功: {path}");
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存文件时出错: {ex.Message}");
            }
        }
    }
}
