using Infrastructure.Extensions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZR.Infrastructure.Extensions;

namespace ZR.Common
{
    public enum EnumDbHelperCreateTableModel
    {
        CreateNew = 1,
        CreateIfNotExists = 2,
    }
    public class DbHelper
    {

        public SqlSugarClient db;

        public LogHelper LogHelper;

        public DbHelper()
        {
            InitDb2();
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
        private void InitDb2()
        {
            db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = "Data Source=localhost;Database=demo;User ID=root;Password=123456;pooling=true;port=3306;sslmode=none;CharSet=utf8;Convert Zero Datetime=True;Allow Zero Datetime=True;AllowLoadLocalInfile=true;"
                ,
                DbType = SqlSugar.DbType.MySql,
                IsAutoCloseConnection = true
            }, configAction: db =>
            {
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql);
                };
            });

        }


        public void CreateTable(string tableName, string[] tableColumns, EnumDbHelperCreateTableModel enumCreateTableModel = EnumDbHelperCreateTableModel.CreateIfNotExists)
        {
            // 检查表是否已存在
    
            if (db.DbMaintenance.IsAnyTable(tableName))
            {
                if (enumCreateTableModel == EnumDbHelperCreateTableModel.CreateIfNotExists)
                {
                    ///跳过表
                    Console.WriteLine($"表 {tableName} 已存在，跳过创建。");
                    return;
                }
                else if (enumCreateTableModel == EnumDbHelperCreateTableModel.CreateNew)
                {
                    ///删除表
                    db.DbMaintenance.DropTable(tableName);
                    Console.WriteLine($"表 {tableName} 已删除。");
                }

            }

       

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


        public void InsertToTable(string tableName, string[] tableColumns, DataTable dataTable)
        {

            //var sql = db.Insertable(dynamics).ToSqlString();

            // 在 BulkCopy 前设置批量大小
            db.Fastest<dynamic>()
                .AS(tableName)
                .PageSize(50000).BulkCopy(dataTable);

            LogHelper.Logger.Information($"表 {tableName} 数据更新成功！");
                
        }

    }

}
