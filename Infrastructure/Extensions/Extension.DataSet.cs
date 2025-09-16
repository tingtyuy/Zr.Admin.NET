using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Infrastructure.Extensions
{
    public static class DataSetExtensions
    {

        public static List<T> ToList<T>(this DataSet ds) where T : new()
        {
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                List<T> list = new List<T>();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    T model = new T();
                    var modelType = typeof(T);
                    var dataRowColumns = row.Table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList();

                    foreach (var property in modelType.GetProperties())
                    {
                        if (dataRowColumns.Contains(property.Name.ToUpper()))
                        {
                            var value = row[property.Name];
                            if (value != DBNull.Value)
                            {
                                property.SetValue(model, Convert.ChangeType(value, property.PropertyType), null);
                            }
                        }
                    }
                    list.Add(model);
                }
                return list;
            }
            else
                return null;

        }
    }
}
