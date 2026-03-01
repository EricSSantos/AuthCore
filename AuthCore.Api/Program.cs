using AuthCore.Api.Configurations.Extensions;
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

#endregion

var app = builder.Build();

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
app.UseCsrfProtection();
app.MapControllers();

#endregion

app.Run();
