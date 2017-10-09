using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using MyBucks.Core.DataProvider.Tools;

namespace MyBucks.Core.DataProvider.Extensions
{
    public static class EfCoreExtensions
    {
        public static IEnumerable<TReturn> ExecuteStoredProcedure<TReturn>(this DbContext dbConnection, string query, params SqlParameter[] parameters) 
        {
            AssertSqlServer(dbConnection);
            var results = new List<TReturn>();
            using (var command = dbConnection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                command.Parameters.AddRange(parameters);
                dbConnection.Database.OpenConnection();


                System.Data.Common.DbDataReader dr = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                return DatabaseTools.ToObjectList<TReturn>(dt); // dt.ToObjectList<TReturn>()
            }
        }

        private static void AssertSqlServer(DbContext dbConnection)
        {
            if (!dbConnection.Database.IsSqlServer())
            {
                throw new NotSupportedException("Stored procedures not supported by databases other than Sql Server.");
            }
        }

        public static void AddOrUpdate<T>(this DbSet<T> dbSet, T data) where T : class
        {
            var t = typeof(T);
            PropertyInfo keyField = null;
            foreach (var propt in t.GetProperties())
            {
                var keyAttr = propt.GetCustomAttribute<KeyAttribute>();
                if (keyAttr != null)
                {
                    keyField = propt;
                    break; // assume no composite keys
                }
            }
            if (keyField == null)
            {
                throw new Exception($"{t.FullName} does not have a KeyAttribute field. Unable to exec AddOrUpdate call.");
            }

            var keyVal = keyField.GetValue(data);

            var dbVal = dbSet.Find(keyVal);

            if (dbVal != null)
            {
                dbSet.Update(data);
                return;
            }
            dbSet.Add(data);
        }

        public static TReturn ExecuteStoredProcedureSingleResult<TReturn>(this DbContext dbConnection, string query, params SqlParameter[] parameters)
        {
            AssertSqlServer(dbConnection);
            var results = new List<TReturn>();
            using (var command = dbConnection.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = query;
                command.CommandType = CommandType.Text;
                command.Parameters.AddRange(parameters);
                dbConnection.Database.OpenConnection();


                System.Data.Common.DbDataReader dr = command.ExecuteReader();
                
                return dr.Read() ? dr.GetFieldValue<TReturn>(0) : default(TReturn);
            }
        }

    }
}
