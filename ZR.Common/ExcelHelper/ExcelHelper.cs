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
        /// 从下往上遍历 Sheet 的每一行（避免操作时索引变化问题）
        /// </summary>
        /// <param name="sheet">要遍历的 Sheet</param>
        /// <param name="action">对每一行执行的操作（参数是当前行 IRow）</param>
        public void ForeachRow(ISheet sheet, Action<IRow> action)
        {
            if (sheet == null)
                throw new ArgumentNullException(nameof(sheet));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            // 从下往上遍历（避免删除行时索引变化问题）
            for (int rowIndex = sheet.LastRowNum; rowIndex >= 0; rowIndex--)
            {
                IRow row = sheet.GetRow(rowIndex);
                if (row != null) // 跳过空行
                {
                    action.Invoke(row); // 执行传入的 Action
                }
            }
        }
    }
}
