using Service.Bot;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Bot>();

var host = builder.Build();
host.Run();