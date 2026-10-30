using System.Diagnostics;
using Application;
using Application.Abstractions.Persistence.Seeders.Models;
using Infrastructure;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Extensions;
using Presentation.Server.Middlewares;
using Presentation.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration);

builder.Services.Configure<AdminUserSettings>(builder.Configuration.GetRequiredSection(AdminUserSettings.SectionName));

builder.Services.AddControllers();
builder.Services.AddSingleton<CustomerExcelService>();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions.TryAdd(
            "traceId",
            Activity.Current?.Id ?? context.HttpContext.TraceIdentifier);
    };
});
builder.Services.AddExceptionHandler<ExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

await app.Services.SeedDatabaseAsync();

app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseDefaultFiles();
app.MapStaticAssets();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
