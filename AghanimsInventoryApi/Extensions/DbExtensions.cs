using AghanimsInventoryApi.Data;
using Microsoft.EntityFrameworkCore;

namespace AghanimsInventoryApi.Extensions;

public static class DbExtensions
{
    public static IServiceCollection AddDatabaseSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AghanimsInventoryDbContext>(options =>
        {
            options.UseSqlServer(configuration
                .GetConnectionString("DbConnection"), options =>
                {
                    options.EnableRetryOnFailure(3);
                });
        });

        return services;
    }
}