using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Common.ExcelHelper
{
    public class OpenXmlService : IExcelService
    {
        public void ClearExcelStyle(string excelPath)
        {
            var cleanedPath = Path.Combine(Path.GetFullPath(excelPath), $"{Path.GetFileNameWithoutExtension(excelPath)}_cleaned.xlsx");
            // 1. 复制模板文件（避免修改原文件）
            File.Copy(excelPath, cleanedPath, true);

            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(cleanedPath, true))
            {
                // 2. 删除所有样式表（styles.xml）
                var stylePart = doc.WorkbookPart.GetPartsOfType<WorkbookStylesPart>();
                if (stylePart != null)
                {
                    doc.WorkbookPart.DeletePart((OpenXmlPart)stylePart);
                    doc.WorkbookPart.AddNewPart<WorkbookStylesPart>(); // 可选：重新创建空样式表
                }

                // 3. 删除计算链（calcChain.xml）
                var calcChainPart = doc.WorkbookPart.GetPartById("rId3"); // 可能需要根据实际情况调整
                if (calcChainPart != null)
                {
                    doc.WorkbookPart.DeletePart(calcChainPart);
                }

            }
        }
    }
}
