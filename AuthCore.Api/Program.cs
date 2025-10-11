using AuthCore.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabases();
builder.AddInjections();
builder.AddAuthentication();
builder.AddSwaggerService();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseHttpsRedirection();

}

app.UseSwaggerDocumentation();
app.UseExceptionHandling();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();