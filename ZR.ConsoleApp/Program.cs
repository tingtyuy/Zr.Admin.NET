using Infrastructure.Extensions;
using MiniExcelLibs;
//using Models;
using NPOI.SS.UserModel;
using SqlSugar;
using ZR.Common;
using ZR.Common.ExcelHelper;
using ZR.Infrastructure.Extensions;

Test();
void Test()
{


    #region 引入

    var dbHelper = new DbHelper();
    var logHelper = new LogHelper();

    #endregion

    #region 创建实体类
    //dbHelper.db.DbFirst.IsCreateAttribute().CreateClassFile(@"D:\123456789\kai\Zr.Admin.NET\ZR.ConsoleApp\models");

    //return; 
    #endregion

    //db.Deleteable<TempUserOrderData>().ExecuteCommand();

    //var targetDir = @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单1021";

    var fileDir = @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单";
    var fileDir2 = @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\仓里账单";




    TableCreateAndInsert(dbHelper, logHelper, fileDir, "out");
    //TableCreateAndInsert(dbHelper, logHelper, fileDir2, "in");

}


void TableCreateAndInsert(DbHelper dbHelper, LogHelper logHelper, string fileDir, string tableNamePre)
{
    var filePaths = new List<string>();
    GetAllFiles(fileDir, filePaths);



    foreach (var filePath in filePaths)
    {
        var npoiExcelHelper = new ExcelHelper(filePath);

        #region 导出客户、总单量、总金额
        //ExportTatalStatistic(filePath); 
        #endregion

        #region 删除指定日期之前的数据
        //bool flowControl = MakeDateData(logHelper, targetDir, filePath);
        //if (!flowControl)
        //{
        //    continue;
        //} 
        #endregion

        #region 导入所有
        // 1. 确定sheet
        var sheet = npoiExcelHelper.GetSheet(new string[] { "快递费", "账单明细", "账单明细总", "申通" });
        if (sheet is null)
        {
            logHelper.Logger.Error($"未找到指定工作表：{filePath}");
            continue;
        }
        // 2. 拿到表头 和 表名

        var tableColumns = npoiExcelHelper.GetFirstRowAsStringArray(sheet).Select(s => s.Value).ToArray();
        var tableName = $"{tableNamePre}{Path.GetFileNameWithoutExtension(filePath).FilterSpecial()}";

        // 3. 创建表
        try
        {

            dbHelper.CreateTable(tableName, tableColumns, EnumDbHelperCreateTableModel.CreateNew);
        }
        catch (Exception ex)
        {

            logHelper.Logger.Error($"创建表失败：{filePath}=》{ex.Message}");
        }


        // 5. 读取数据

        //var tableData = MiniExcel.QueryAsDataTable(filePath,useHeaderRow:true, sheetName: sheet.SheetName);

        var tableData = new System.Data.DataTable();

        try
        {

            tableData = npoiExcelHelper.GetTableData(sheet);

            logHelper.Logger.Error($"读取到成功：{filePath}=》{tableData.Rows.Count}");
        }
        catch (Exception ex)
        {

            logHelper.Logger.Error($"读取数据失败：{filePath}=》{ex.Message}");
        }


        // 4. 插入数据

        //dbHelper.db.Insertable(tableData).AS(tableName).ExecuteCommand();

        try
        {

            dbHelper.InsertToTable(tableName, tableColumns, tableData);
        }
        catch (Exception ex)
        {

            logHelper.Logger.Error($"插入数据失败：{filePath}=》{ex.Message}");
        }

        #endregion
    }
}





void ReadSheetByIndex(string filePath, int sheetIndex, out IEnumerable<dynamic>? list)
{
    list = null;
    try
    {
        // 1. 获取所有工作表名称
        var sheetNames = MiniExcel.GetSheetNames(filePath);

        // 2. 检查索引是否有效
        if (sheetIndex < 0 || sheetIndex >= sheetNames.Count)
        {
            Console.WriteLine($"索引 {sheetIndex} 超出范围。可用索引范围：0 到 {sheetNames.Count - 1}");
            return;
        }

        // 3. 通过索引获取目标工作表名称
        string targetSheetName = sheetNames[sheetIndex];
        Console.WriteLine($"正在读取工作表：{targetSheetName}");

        // 4. 读取目标工作表数据
        list = MiniExcel.Query(filePath, sheetName: targetSheetName);

    }

    catch (Exception ex)
    {
        Console.WriteLine($"读取数据时出错：{ex.Message}");
    }
}
//递归遍历文件夹
void GetAllFiles(string path, List<string> files)
{
    try
    {
        foreach (string file in Directory.GetFiles(path))
        {
            if (file.Contains(".xlsx"))
            {
                files.Add(file);
            }
        }
        foreach (string directory in Directory.GetDirectories(path))
        {
            GetAllFiles(directory, files);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error accessing path {path}: {ex.Message}");
    }
}



static bool MakeDateData(LogHelper logHelper, string targetDir, string filePath)
{
    var npoiExcelHelper = new ExcelHelper(filePath);
    if (npoiExcelHelper.Workbook is null)
    {

        logHelper.Logger.Error($"文件无法打开：{filePath}");
        return false;
    }
    var sheet = npoiExcelHelper.Workbook.GetSheetAt(1);
    DateTime thresholdDate = new DateTime(2025, 9, 5);
    npoiExcelHelper.ForeachRow(sheet, row =>
    {

        ICell cell = row.GetCell(0);
        if (cell == null)
        {
            logHelper.Logger.Error($"第一列不是业务日期：{filePath}");
            return;
        }
        if (DateTime.TryParse(cell.ToString(), out DateTime cellDate))
        {
            if (cellDate < thresholdDate)
            {
                sheet.RemoveRow(row);
            }
        }


    });

    // 保存修改
    using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
    {
        npoiExcelHelper.Workbook.Write(fs);
    }
    var targetFilePath = Path.Combine(targetDir, Path.GetFileName(filePath));
    File.Move(filePath, targetFilePath);
    logHelper.Logger.Information($"修改完成：{filePath}");
    return true;
}

static void ExportTatalStatistic(string filePath)
{
    var rows = MiniExcel.Query(filePath).ToList();
    // 获取 A1 单元格的值（第0行第0列）
    if (rows.Count > 6)
    {
        var userName = rows[2].B;
        var orderCount = rows[5].D;
        var orderMoney = rows[5].F;
        //db.Insertable(new TempUserOrderData
        //{
        //    UserName = userName,
        //    OrderCount = orderCount,
        //    OrderMoney =orderMoney
        //}).ExecuteCommand();
        //logHelper.Logger.Information($"{filePath}=>userName:{userName},orderCount:{orderCount},orderMoney:{orderMoney}");
        Console.WriteLine($"userName:{userName},orderCount:{orderCount},orderMoney:{orderMoney}");
    }
}