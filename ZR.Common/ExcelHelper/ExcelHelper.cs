using Infrastructure.Extensions;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Logging;
using MiniExcelLibs;
using MiniExcelLibs.OpenXml;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZR.Infrastructure.Extensions;
namespace ZR.Common.ExcelHelper
{
    /// <summary>
    /// 选择的列
    /// </summary>
    public class SelectColumn
    {
        public string Name { get; set; }

        public string[] MaybeName { get; set; }
    }
    public class ExcelHelper
    {
        public readonly string ExcelFilePath;

        public readonly IWorkbook Workbook;
        public static Common.LogHelper logHelper= new Common.LogHelper(false);

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

        public DataTable GetTableData(ISheet sheet, List<SelectColumn> selectColumns = null)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet));

            var firstRow = GetFirstRowAsStringArray(sheet);

            if (selectColumns is not null)
            {
                firstRow = firstRow.Where(kv =>
                {
                    var match = selectColumns.Any(sc => sc.MaybeName.Contains(kv.Value));
                    return match;
                }
                ).ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            var dataTable = new System.Data.DataTable();
            try
            {


                if (selectColumns is not null)
                {
                    dataTable.Columns.AddRange(firstRow.Values.Select(s => new System.Data.DataColumn(
                        selectColumns.Where(w => w.MaybeName.Contains(s)).FirstOrDefault().Name
                        )).ToArray());
                }
                else
                {
                    dataTable.Columns.AddRange(firstRow.Values.Select(s => new System.Data.DataColumn(s)).ToArray());

                }

                // 预计算列索引，避免在循环中重复查询
                var validColumnIndices = firstRow.Keys.OrderBy(x => x).ToArray();

                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    // 1. 一次性获取行，避免重复调用 GetRow
                    var row = sheet.GetRow(rowIndex);
                    if (row == null) continue;

                    // 2. 直接读取目标单元格，避免 Where + Select
                    var cells = new object[validColumnIndices.Length];
                    for (int i = 0; i < validColumnIndices.Length; i++)
                    {
                        var cell = row.GetCell(validColumnIndices[i]);
                        cells[i] = cell == null ? DBNull.Value : GetCellValue(cell);
                    }

                    // 3. 批量填充 DataRow
                    DataRow newRow = dataTable.NewRow();
                    for (int i = 0; i < cells.Length && i < dataTable.Columns.Count; i++)
                    {
                        newRow[i] = cells[i] ?? DBNull.Value; // 直接赋值，避免 ToString()
                    }

                    dataTable.Rows.Add(newRow);
                }

                //for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                //{
                //    var row = sheet.GetRow(rowIndex);
                //    if (row is null)
                //    {
                //        continue;
                //    }

                //    var cells = sheet.GetRow(rowIndex).Cells.Where(s => firstRow.Keys.Contains(s.ColumnIndex)).Select(s => GetCellValue(s)).ToArray();

                //    DataRow newRow = dataTable.NewRow();
                //    for (int i = 0; i < cells.Length; i++)
                //    {
                //        if (i < dataTable.Columns.Count)
                //        {
                //            newRow[i] = string.IsNullOrEmpty(cells[i].ToString()) ? DBNull.Value : (object)cells[i];
                //        }
                //    }

                //    dataTable.Rows.Add(newRow);
                //}
            }
            catch (Exception)
            {

                throw;
            }


            return dataTable;



        }
        public static List<dynamic> GetDynamicData(
           string excelFilePath,
           List<string> selectMaybeSheetNames,
           List<SelectColumn> selectColumns = null)
        {
            // 1. 获取所有 Sheet 名称
            var sheetAllNames = MiniExcel.GetSheetNames(excelFilePath);

            // 2. 筛选出用户指定的 Sheet
            var selectSheetNames = sheetAllNames.Intersect(selectMaybeSheetNames);
            if (!selectSheetNames.Any())
            {
                logHelper.Logger.Error($"{excelFilePath}不包含指定的 Sheet 名称");
            }

            // 3. 处理每个选中的 Sheet

            var result = new List<dynamic>();
            foreach (var sheetName in selectSheetNames)
            {
                var excelAllColumnNames = MiniExcel.GetColumns(excelFilePath, useHeaderRow: true, sheetName: sheetName);
                if (excelAllColumnNames is null)
                {
                    logHelper.Logger.Error($"{excelFilePath}的{sheetName}工作表中,没有获取到任何列名");
                    continue;
                }

                // 4. 如果 selectColumns 为 null，则默认返回所有列（不重命名）
                if (selectColumns == null || !selectColumns.Any())
                {
                    return MiniExcel.Query(excelFilePath, useHeaderRow: true, sheetName: sheetName).ToList();
                }

                // 5. 构建原始列名 → 新列名（Name）的映射字典
                var columnMapping = excelAllColumnNames
                    .Where(originalCol => selectColumns.Any(sc => sc.MaybeName.Contains(originalCol.FilterSpecial())))
                    .ToDictionary(
                        originalCol => originalCol, // 原始列名
                        originalCol => selectColumns
                            .First(sc => sc.MaybeName.Contains(originalCol.FilterSpecial()))
                            .Name // 映射到 selectColumns 的 Name
                    );
                //没有拿到的列
                var shortColumns = selectColumns.Select(s => s.Name).Except(columnMapping.Select(s => s.Value));
                foreach (var shortCol in shortColumns)
                {
                    logHelper.Logger.Error($"{excelFilePath}的{sheetName}工作表中不包含必需的列，缺失列：{shortCol}");
                }

                // 6. 读取 Excel 数据并动态修改列名
                var excelData = MiniExcel.Query(excelFilePath, useHeaderRow: true, sheetName: sheetName);


                foreach (IDictionary<string, object> row in excelData)
                {
                    // 使用 ExpandoObject 动态构建新行
                    dynamic newRow = new ExpandoObject();
                    var newRowDict = (IDictionary<string, object>)newRow;

                    foreach (var mapping in columnMapping)
                    {
                        var originalCol = mapping.Key;
                        var newCol = mapping.Value;

                        if (row.TryGetValue(originalCol, out var value))
                        {
                            newRowDict[newCol] = value?.ToString() ?? ""; // 使用新列名
                        }
                        newRowDict["UserName"] = "";
                        newRowDict["UserGroup"] = "";

                    }

                    result.Add(newRow);
                }
            }





            return result;
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
