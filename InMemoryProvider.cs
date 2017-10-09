using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace MyBucks.Core.DataProvider
{
    public class InMemoryProvider : ContextBase
    {
        private string _databaseName = "unknown";

        public InMemoryProvider(string databaseName) : base()
        {
            _databaseName = databaseName;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Please see https://github.com/aspnet/EntityFramework/issues/5106 for UseRowNumberPaging. This is only required for Sql Server 2008.
            //"User ID=postgres;Host=localhost;Port=5432;Database=application_service;Pooling = true;"
            optionsBuilder.UseInMemoryDatabase(_databaseName);
            //optionsBuilder.UseSqlServer(_connectionString, builder => builder.UseRowNumberForPaging());
        }
    }
}
