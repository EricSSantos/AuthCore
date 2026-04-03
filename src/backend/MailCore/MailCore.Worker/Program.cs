using MailCore.Service;
using MailCore.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();

if (builder.Environment.IsDevelopment())
    builder.Logging.AddSimpleConsole();

builder.Services.AddMailCore(builder.Configuration);
builder.Services.AddHostedService<Worker>();

builder.Build().Run();
