using AuthCore.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

#region Services

builder.AddDatabases();
builder.AddInjections();
builder.AddAuthentication();
builder.AddSwaggerService();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#endregion

var app = builder.Build();

#region Middleware

app.UseSwaggerDocumentation();
app.UseExceptionHandling();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

#endregion

app.Run();