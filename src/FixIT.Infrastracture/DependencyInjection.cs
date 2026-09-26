using FixIT.Domain.Interfaces;
using FixIT.Infrastracture.Data;
using FixIT.Infrastracture.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FixIT.Infrastracture
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


            services.AddDbContext<FixITDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IServiceRequestRepository, ServiceRequestRepository>();

            return services;
        }
    }
}