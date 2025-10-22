using Microsoft.Identity.Client;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Common.ExcelHelper
{
    public class ExcelHelper
    {
        public readonly string ExcelFilePath;

        public readonly IWorkbook Workbook;

        public ExcelHelper(string _excelFilePath)
        {
            ExcelFilePath = _excelFilePath;
            try
            {
                #region 初始化 Workbook
                if (!File.Exists(ExcelFilePath))
                {
                    Console.WriteLine("文件不存在！");
                    return;
                }
                using (FileStream fs = new FileStream(ExcelFilePath, FileMode.Open, FileAccess.ReadWrite))
                {
                    if (Path.GetExtension(ExcelFilePath).ToLower() == ".xls")
                        Workbook = new HSSFWorkbook(fs);
                    else
                        Workbook = new XSSFWorkbook(fs);
                }
                #endregion
            }
            catch (Exception)
            {

            }

        }

        /// <summary>
        /// 遍历 Sheet的每一
        /// </summary>
        /// <param name="sheet">要遍历的 Sheet</param>
        /// <param name="action">对每一行执行的操作（参数是当前行 IRow）</param>
        /// <param name="isForward">是不是正向遍历（false的时候可避免操作引起的索引问题）</param>
        public void ForeachRow(ISheet sheet, Action<IRow> action, bool isForward = false)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            if (isForward)
            {
                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row != null)
                    {
                        action.Invoke(row);
                    }
                }

            }
            else
            {
                // 从下往上遍历（避免删除行时索引变化问题）
                for (int rowIndex = sheet.LastRowNum; rowIndex >= 0; rowIndex--)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    if (row != null)
                    {
                        action.Invoke(row);
                    }
                }

            }

        }

        public ISheet GetSheet(string[] sheetNames)
        {
            ISheet sheet = null;
            foreach (var name in sheetNames)
            {
                sheet = Workbook.GetSheet(name);
                if (sheet != null)
                {
                    return sheet;
                }
            }
            return sheet;

        }

        public  string[] GetFirstRowAsStringArray(ISheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "工作表不能为 null！");

            IRow firstRow = sheet.GetRow(0);
            if (firstRow == null)
                return Array.Empty<string>(); // 返回空数组而非 null

            return firstRow.Cells
                .Select(cell => cell.ToString()?.Trim()) // 处理可能的 null 值并去除空格
                .ToArray();
        }
    }
}
