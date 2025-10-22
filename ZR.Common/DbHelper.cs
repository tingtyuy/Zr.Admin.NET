using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZR.Common
{
    public class DbHelper
    {
        public SqlSugarClient db;

        public LogHelper LogHelper;

        public DbHelper()
        {
            InitDb();
            LogHelper = new LogHelper();
        }
        private void InitDb()
        {
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=47.105.65.51;Initial Catalog=mdtwo_dev;Encrypt=True;TrustServerCertificate=True;User ID=mdtwo_dbadmin;Password=Mdtwo2025;Connection Timeout=1200"
                ,
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = true
            }, configAction: db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql);
                };
            });

        }

        public void CreateTable(string tableName, string[] tableColumns)
        {
            // 表名过滤掉空和特殊符号字符
            tableName = Regex.Replace(tableName, @"[^\u4e00-\u9fa5a-zA-Z0-9_]", "");
            // 检查表是否已存在
            if (db.DbMaintenance.IsAnyTable(tableName))
            {
                Console.WriteLine($"表 {tableName} 已存在，跳过创建。");
                return;
            }
   
            // 列名过滤掉空和特殊符号字符
            tableColumns = tableColumns.Where(column => !string.IsNullOrEmpty(column))
                .Select(s => Regex.Replace(s, @"[^\u4e00-\u9fa5a-zA-Z0-9_]", ""))  // 过滤特殊符号
                .ToArray();

            // 构建列定义
            var columns = new List<DbColumnInfo>();
            foreach (string tableColumn in tableColumns)
            {
                // 默认类型为NVARCHAR(255)，可根据需要调整
                columns.Add(new DbColumnInfo
                {
                    DbColumnName = tableColumn,
                    DataType = "nvarchar",
                    Length = 255,
                    IsNullable = true
                });
            }

            // 动态创建表
            db.DbMaintenance.CreateTable(tableName, columns);
            LogHelper.Logger.Information($"表 {tableName} 创建成功！");
        }


        public void InsertToTable(string tableName, string[] tableColumns)
        {
            // 表名过滤掉空和特殊符号字符
            tableName = Regex.Replace(tableName, @"[^\u4e00-\u9fa5a-zA-Z0-9_]", "");
            // 检查表是否已存在
            if (db.DbMaintenance.IsAnyTable(tableName))
            {
                Console.WriteLine($"表 {tableName} 已存在，跳过创建。");
                return;
            }

            // 列名过滤掉空和特殊符号字符
            tableColumns = tableColumns.Where(column => !string.IsNullOrEmpty(column))
                .Select(s => Regex.Replace(s, @"[^\u4e00-\u9fa5a-zA-Z0-9_]", ""))  // 过滤特殊符号
                .ToArray();

            // 构建列定义
            var columns = new List<DbColumnInfo>();
            foreach (string tableColumn in tableColumns)
            {
                // 默认类型为NVARCHAR(255)，可根据需要调整
                columns.Add(new DbColumnInfo
                {
                    DbColumnName = tableColumn,
                    DataType = "nvarchar",
                    Length = 255,
                    IsNullable = true
                });
            }

            // 动态创建表
            db.DbMaintenance.CreateTable(tableName, columns);
            LogHelper.Logger.Information($"表 {tableName} 创建成功！");
        }
        public void BulkInsertData(SqlSugarClient db, DataTable dataTable, string tableName)
        {
            //// 将DataTable转换为动态对象列表
            //var entities = new List<ExpandoObject>();
            //foreach (DataRow row in dataTable.Rows)
            //{
            //    dynamic entity = new ExpandoObject();
            //    var dict = (IDictionary<string, object>)entity;
            //    foreach (DataColumn col in dataTable.Columns)
            //    {
            //        dict[col.ColumnName] = row[col];
            //    }
            //    entities.Add(entity);
            //}

            //// 批量插入（使用SqlSugar的动态建表能力）
            //db.Fastest<dynamic>()
            //    .AS(tableName)
            //    .BulkCopy(entities);

            //Console.WriteLine($"成功插入 {entities.Count} 条数据到表 {tableName}！");
        }

     
    }

}
