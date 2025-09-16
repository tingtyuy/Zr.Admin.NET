using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Infrastructure.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// convert to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static DataSet ToDataSet<T>(List<T> list) where T : class
        {
            // 创建一个新的DataSet
            DataSet dataSet = new DataSet();

            // 获取T类型的所有公共属性
            PropertyInfo[] properties = typeof(T).GetProperties();

            // 创建一个DataTable来存储数据
            DataTable dataTable = new DataTable(typeof(T).Name);

            // 添加列
            foreach (PropertyInfo property in properties)
            {
                dataTable.Columns.Add(property.Name, property.PropertyType);
            }

            // 将List<T>转换为DataTable
            foreach (T item in list)
            {
                if (item != null)
                {
                    var values = properties.Select(p => p.GetValue(item)).ToArray();
                    dataTable.Rows.Add(values);
                }
            }

            // 将DataTable添加到DataSet
            dataSet.Tables.Add(dataTable);

            return dataSet;
        }
    }
}
