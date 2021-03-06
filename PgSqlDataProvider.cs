﻿using Microsoft.EntityFrameworkCore;

namespace Thorium.Core.DataProvider
{
    public class PgSqlDataProvider : ContextBase
    {
        public PgSqlDataProvider(string connectionString) : base(connectionString) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Please see https://github.com/aspnet/EntityFramework/issues/5106 for UseRowNumberPaging. This is only required for Sql Server 2008.
            //"User ID=postgres;Host=localhost;Port=5432;Database=application_service;Pooling = true;"
            optionsBuilder.UseNpgsql(ConnectionString);
            //optionsBuilder.UseSqlServer(_connectionString, builder => builder.UseRowNumberForPaging());
        }
    }
}
