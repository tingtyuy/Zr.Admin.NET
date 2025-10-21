using MiniExcelLibs;
using NPOI.SS.UserModel;
using SqlSugar;
using ZR.Common.ExcelHelper;

SqlSugarClient db;
Test();
void Test()
{
    InitDb();



    //db.DbFirst.IsCreateAttribute().CreateClassFile(@"D:\123456789\kai\Zr.Admin.NET\ZR.ConsoleApp\models");

    var fileDir = @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单";
    var targetDir = @"D:\123456789\md\运单-账单计算\MD-2025-09-账单数据\揽收账单1021";
    var filePaths = new List<string>();
    GetAllFiles(fileDir, filePaths);

    foreach (var filePath in filePaths)
    {
        //var rows = MiniExcel.Query(filePath).ToList();
        //// 获取 A1 单元格的值（第0行第0列）
        //if (rows.Count > 6)
        //{
        //    var userName = rows[2].B;
        //    var orderCount = rows[5].D;
        //    var orderMoney = rows[5].F;
        //    Console.WriteLine($"userName:{userName},orderCount:{orderCount},orderMoney:{orderMoney}");
        //}
        var npoiExcelHelper = new NpoiExcelHelper(filePath);
        if (npoiExcelHelper.Workbook is null)
        {

            Console.WriteLine($"文件无法打开：{filePath}");
            continue;
        }
        var sheet = npoiExcelHelper.Workbook.GetSheetAt(1);
        DateTime thresholdDate = new DateTime(2025, 9, 5);
        npoiExcelHelper.ForeachRow(sheet, row =>
        {

            ICell cell = row.GetCell(0);
            if (cell == null)
            {
                Console.WriteLine($"第一列不是业务日期：{filePath}");
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
            File.Move(filePath, targetDir);
            Console.WriteLine($"修改完成：{filePath}");
        }

    }

}
void SyncTable(string file)
{
    MiniExcel.Query(file).ToList();
    IEnumerable<dynamic>? list;
    ReadSheetByIndex(file, 1, out list);
    if (list != null)
    {
        //db.Insertable(list).ExecuteCommand();
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
            if (file.Contains("xlsx"))
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

void InitDb()
{
    db = new SqlSugarClient(new ConnectionConfig()
    {
        ConnectionString = "Data Source=47.105.65.51;Initial Catalog=mdsto_dev;Encrypt=True;TrustServerCertificate=True;User ID=mdsto_dbadmin;Password=Mdstodb2025;Connection Timeout=1200"
        ,
        DbType = DbType.SqlServer,
        IsAutoCloseConnection = true
    }, configAction: db =>
    {
        db.Aop.OnLogExecuting = (sql, pars) =>
        {
            Console.WriteLine(sql);
        };
    });

}