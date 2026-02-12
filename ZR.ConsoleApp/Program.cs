using Infrastructure.Extensions;
using Microsoft.Playwright;
using MiniExcelLibs;
using Models;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using SqlSugar;
using ZR.Common;
using ZR.Common.ExcelHelper;
using ZR.ConsoleApp;
using ZR.Infrastructure.Extensions;

/// 1.编辑excel文件，删除指定日期之前的数据
/// 2.查询最新的公式计算结果
/// 3.批量导入excel文件，生成对应的数据库表
//ExcelImportDemo.Run();
//await RulesEngineDemo.Run();
//await MusicDemo.Run();
await PlayWrightDemo.Run();