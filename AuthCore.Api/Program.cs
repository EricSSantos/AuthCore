using AuthCore.Api.Configurations.Extensions;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.AddDatabases();
builder.AddDependencyInjections();
builder.AddAuthentication();
builder.AddSwaggerService();
builder.AddCorsPolicies();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

#region Middleware

app.UseSwaggerDoc();
app.UseExceptionsHandling();
app.UseAuthentication();
app.UseAuthorization();
app.UseCorsAndHttps();
app.MapControllers();

#endregion

app.Run();