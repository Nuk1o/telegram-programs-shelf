using Service;
using Service.Bot;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Bootstrap>();
builder.Services.AddHostedService<Bot>();

var host = builder.Build();
host.Run();