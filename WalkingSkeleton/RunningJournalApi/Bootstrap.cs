﻿using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json.Serialization;
using RunningJournalApi.Properties;

namespace RunningJournalApi
{
    public class Bootstrap
    {
        public void Configure(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "API Default",
                routeTemplate: "{controller}/{id}",
                defaults: new
                {
                    controller = "Journal",
                    id = RouteParameter.Optional
                });

            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Services.Replace(
                typeof(IHttpControllerActivator),
                new CompositeRoot());
        }

        public void InstallDatabase()
        {
            var connStr = ConfigurationManager.ConnectionStrings["running-journal"].ConnectionString;
            var builder = new SqlConnectionStringBuilder(connStr);
            builder.InitialCatalog = "Master";
            Console.WriteLine("Create at " + builder.ConnectionString);
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;

                    var schemaSql = Resources.RunningDbSchema;
                    foreach (var sql in schemaSql.Split(new [] {"GO"}, StringSplitOptions.RemoveEmptyEntries))
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void UninstallDatabase()
        {
            var connStr = ConfigurationManager.ConnectionStrings["running-journal"].ConnectionString;
            var builder = new SqlConnectionStringBuilder(connStr);
            builder.InitialCatalog = "Master";
            Console.WriteLine("Drop at "+ builder.ConnectionString);
            using (var conn = new SqlConnection(builder.ConnectionString))
            {
                conn.Open();

                var dropCmd = @"IF EXISTS (SELECT name 
                                           FROM master.dbo.sysdatabases 
                                           WHERE name = N'RunningJournal')
                                    DROP DATABASE [RunningJournal];";
                using (var cmd = new SqlCommand(dropCmd, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

        }
    }
}