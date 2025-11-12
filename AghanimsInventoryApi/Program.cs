using AghanimsInventoryApi.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddEndpointsApiExplorer()
    .AddApiVersioningSettings()
    .AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Aghanim's Inventory API Documentation")
            .WithTheme(ScalarTheme.Default)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
            .AddPreferredSecuritySchemes("Bearer")
            .AddHttpAuthentication("Bearer", bearer =>
            {
                bearer.Token = "your-bearer-token";
            })
            .ShowSidebar = true;
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
