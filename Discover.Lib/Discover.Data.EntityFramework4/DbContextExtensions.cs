using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;
using System.Text.RegularExpressions;

namespace Discover.Data.EntityFramework4
{
    public static class DbContextExtensions
    {
        public static void BulkInsert<T>(this DbContext context, IEnumerable<T> entities) where T : class
        {
            BulkInsert(context, entities, entities.Count());
        }

        public static void BulkInsert<T>(this DbContext context, IEnumerable<T> entities, int batchSize) where T : class
        {
            var closeConnectionWhenFinished = false;

            if (context.Database.Connection.State != ConnectionState.Open)
            {
                context.Database.Connection.Open();
                closeConnectionWhenFinished = true;
            }

            try
            {
                using (var bulkCopy = new SqlBulkCopy((SqlConnection)context.Database.Connection))
                {
                    bulkCopy.BatchSize = batchSize;
                    bulkCopy.DestinationTableName = context.GetTableName<T>();

                    var table = new DataTable();

                    //NB: currently only "simple" properties are supported (no relationships / foreign-key values, or mapped complex types)
                    var props = TypeDescriptor.GetProperties(typeof(T)).Cast<PropertyDescriptor>()
                                              .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                              .ToArray();

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];

                    foreach (var item in entities)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);

                        if (table.Rows.Count >= batchSize)
                        {
                            bulkCopy.WriteToServer(table);
                            table.Clear();
                        }
                    }

                    if (table.Rows.Count > 0)
                    {
                        bulkCopy.WriteToServer(table);
                    }
                }
            }
            finally
            {
                if (closeConnectionWhenFinished) context.Database.Connection.Close();
            }
        }

        public static string GetTableName<T>(this DbContext context) where T : class
        {
            return ((IObjectContextAdapter)context).ObjectContext.GetTableName<T>();
        }

        public static string GetTableName<T>(this ObjectContext context) where T : class
        {
            var m = Regex.Match(context.CreateObjectSet<T>().ToTraceString(), "FROM (?<table>.*) AS");
            return m != null ? m.Groups["table"].Value : null;
        }
    }
}
