using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Common.ExcelHelper
{
    public class EPPlusService : IExcelService
    {
        public EPPlusService()
        {
            SetLicenseContext();
        }
        public void SetLicenseContext()
        {
            //ExcelPackage.LicenseContext = LicenseContext.NonCommercial; //7.0 以上就过时了
            ExcelPackage.License.SetNonCommercialPersonal("mengkai"); //要用这个
        }
        public void ClearExcelStyle(string excelPath)
        {
            var cleanedTemplatePath = Path.Combine(Path.GetFullPath(excelPath), $"{Path.GetFileNameWithoutExtension(excelPath)}_cleaned.xlsx");

            using (var templatePkg = new ExcelPackage(new FileInfo(excelPath)))
            {
   
                // 3. 简化样式（可选：减少样式数量）
                // MiniExcel 对复杂样式支持有限，建议减少模板中的样式种类
                var worksheet = templatePkg.Workbook.Worksheets[0];
                worksheet.Cells.StyleID = 0; // 重置所有单元格样式为默认

                // 4. 保存清理后的模板
                templatePkg.SaveAs(new FileInfo(cleanedTemplatePath));
            }
        }
    }
}
