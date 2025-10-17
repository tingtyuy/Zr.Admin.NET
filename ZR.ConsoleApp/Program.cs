using MiniExcelLibs;
Test();
void Test()
{
    var fileDir = @"D:\\123456789\\md\\运单-账单计算\\MD-2025-09-账单数据";
    var files = new List<string>();
    GetAllFiles(fileDir, files);

    foreach (var file in files)
    {
        var rows = MiniExcel.Query(file).ToList();
        // 获取 A1 单元格的值（第0行第0列）
        if (rows.Count > 6)
        {
            var userName = rows[2].B;
            var orderCount = rows[5].D;
            var orderMoney = rows[5].F;
            Console.WriteLine($"userName:{userName},orderCount:{orderCount},orderMoney:{orderMoney}");
        }
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