using FixIT.Infrastracture.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FixIT.Tests.IntegrationsTests
{
    public class CustomWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        // startar API i testerna med sqlite i minnet iställt för sql servern 
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("ApiKey", "test-nyckel"); // nyckel bara för testerna
            builder.UseSetting("ConnectionStrings:DefaultConnection", "används-inte"); // så API kan starta

            builder.ConfigureServices(services =>
            {
                // Ta bort SQL Server-inställningarna
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<FixITDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                var configDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<FixITDbContext>));
                if (configDescriptor != null)
                    services.Remove(configDescriptor);

                // Ta bort eventuell gammal SqliteConnection
                var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(SqliteConnection));
                if (dbConnectionDescriptor != null)
                    services.Remove(dbConnectionDescriptor);

                // Bara en öppen anslutning = en databas i minnet
                services.AddSingleton<SqliteConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();
                    return connection;
                });

                // FixITDbContext använder den anslutningen
                services.AddDbContext<FixITDbContext>((container, options) =>
                {
                    var connection = container.GetService<SqliteConnection>();
                    options.UseSqlite(connection);
                });
            });
        }

        // skapar tabellerna innan första testet
        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder);

            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FixITDbContext>();
            db.Database.EnsureCreated();

            return host;
        }
    }    
}
