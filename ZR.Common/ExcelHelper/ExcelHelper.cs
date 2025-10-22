using Infrastructure.Extensions;
using Microsoft.Identity.Client;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZR.Infrastructure.Extensions;
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

        public Dictionary<int, string> GetFirstRowAsStringArray(ISheet sheet)
        {
            var result = new Dictionary<int, string>();
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet), "工作表不能为 null！");

            IRow firstRow = sheet.GetRow(0);
            if (firstRow == null)
                return result;

            // 使用 Dictionary 记录已出现的 Value，避免重复
            var seenValues = new HashSet<string>();

            return firstRow.Cells
                  .Select(cell => new
                  {
                      Value = cell?.ToString().FilterSpecial(), // 处理 null 并去除空格
                      ColumnIndex = cell.ColumnIndex
                  })
                  .Where(x => !string.IsNullOrEmpty(x.Value)) // 可选：过滤掉值为 null 的列
                   .Where(x => seenValues.Add(x.Value)) // HashSet.Add() 返回 bool，false 表示已存在
                  .ToDictionary(x => x.ColumnIndex, x => x.Value);
        }

        public DataTable GetTableData(ISheet sheet)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet));

            var firstRow = GetFirstRowAsStringArray(sheet);

            var dataTable = new System.Data.DataTable();
            try
            {

                dataTable.Columns.AddRange(firstRow.Values.Select(s => new System.Data.DataColumn(s)).ToArray());

                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row is null)
                    {
                        continue;
                    }

                    var cells = sheet.GetRow(rowIndex).Cells.Where(s => firstRow.Keys.Contains(s.ColumnIndex)).Select(s => GetCellValue(s)).ToArray();

                    DataRow newRow = dataTable.NewRow();
                    for (int i = 0; i < cells.Length; i++)
                    {
                        if (i < dataTable.Columns.Count)
                        {
                            newRow[i] = string.IsNullOrEmpty(cells[i].ToString()) ? DBNull.Value : (object)cells[i];
                        }
                    }

                    dataTable.Rows.Add(newRow);
                }
            }
            catch (Exception)
            {

                throw;
            }


            return dataTable;



        }


        private object GetCellValue(ICell cell)
        {
            if (cell == null)
                return DBNull.Value;

            return cell.CellType switch
            {
                CellType.String => cell.StringCellValue,
                CellType.Numeric => cell.NumericCellValue,
                CellType.Boolean => cell.BooleanCellValue,
                CellType.Formula => GetFormulaCellValue(cell), // 处理公式
                CellType.Blank => DBNull.Value,
                _ => cell.ToString() // 其他类型（如日期、错误）转为字符串
            };
        }

        // 处理公式单元格
        private object GetFormulaCellValue(ICell cell)
        {
            switch (cell.CachedFormulaResultType)
            {
                case CellType.String: return cell.StringCellValue;
                case CellType.Numeric: return cell.NumericCellValue;
                case CellType.Boolean: return cell.BooleanCellValue;
                default: return cell.ToString();
            }
        }
    }
}
