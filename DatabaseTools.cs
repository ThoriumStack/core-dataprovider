using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.SqlServer.Server;

namespace Thorium.Core.DataProvider
{
    public class DatabaseTools
    {
  


        public static DataTable ConvertToDataTable<T>(IEnumerable<T> data, string dataModelNameSpace)
        {
            var properties =
               TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
            {
                if (prop.PropertyType.FullName != null && (string.IsNullOrEmpty(dataModelNameSpace) || !prop.PropertyType.FullName.Contains(dataModelNameSpace)))
                    table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    if (prop.PropertyType.FullName != null && (string.IsNullOrEmpty(dataModelNameSpace) || !prop.PropertyType.FullName.Contains(dataModelNameSpace)))
                        row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                }
                table.Rows.Add(row);
            }
            return table;

        }

        public static List<SqlDataRecord> GetSQLDataRecordsFromList(List<long> data, string column = "RefId")
        {
            var table = new List<SqlDataRecord>();
            for (int i = 0; i < data.Count; i++)
            {
                var sqlRow = new SqlDataRecord(
                    new SqlMetaData(column, SqlDbType.BigInt)
               );
                sqlRow.SetInt64(0, data[i]);
                table.Add(sqlRow);
            }

            return table;
        }

        public static List<T> ToObjectList<T>(DataTable dt)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            var objectProperties = typeof(T).GetProperties(flags);
            
            var targetList = dt.Rows.Cast<T>().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();

                foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name)))
                {
                    var objectProps = dataRow.GetType().GetProperty(properties.Name);
                    object value = objectProps.GetValue(dataRow);
                    properties.SetValue(instanceOfT, value, null);
                }
                return instanceOfT;
            }).ToList();

            return targetList;
        }

    }
}
