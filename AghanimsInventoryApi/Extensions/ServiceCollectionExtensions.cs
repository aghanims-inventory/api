using AghanimsInventoryApi.Constants;
using FluentValidation;

namespace AghanimsInventoryApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCorsSettings(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(AppConstants.DefaultCorsPolicy,
                builder =>
                {
                    builder.WithOrigins(
                            "http://localhost:3000",
                            "https://localhost:3000",
                            "http://localhost:5173",
                            "https://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        return services;
    }

    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();

        return services;
    }
}
