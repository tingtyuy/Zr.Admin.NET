using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Common.ExcelHelper
{
    public interface IExcelService
    {
        /// <summary>
        /// Removes all formatting and styles from the specified Excel file.
        /// </summary>
        /// <param name="excelPath">The full path to the Excel file from which to clear styles. Cannot be null or empty.</param>
        /// <returns>The path to the updated Excel file with all styles removed.</returns>
        public void ClearExcelStyle(string excelPath);
    }
}
