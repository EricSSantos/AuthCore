using AuthCore.Api.Configurations.Extensions;
using AuthCore.Infrastructure.Persistence.PostgreSQL.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.Logging.ClearProviders();
builder.Host.UseNLog();

builder.AddDependencyInjections();
builder.AddAuthentication();
builder.AddSwaggerService();
builder.AddCorsPolicies();
builder.AddRateLimiting();

builder.Services.AddHealthCheckServices();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

#endregion

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EFCoreDbContext>();
    await dbContext.Database.MigrateAsync();
}

#region Middlewares

app.UseHttpsRedirection();
app.UseRouting();
app.UseCorsPolicy();
app.UseStaticFiles();
app.UseSwaggerDoc();
app.UseRateLimiter();
app.UseExceptionsHandling();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

#endregion

app.Run();
