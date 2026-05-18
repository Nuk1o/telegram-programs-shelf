using dotenv.net;
using Npgsql;

namespace Service;

public class Bootstrap : BackgroundService
{
    private readonly ILogger<Bootstrap> _logger;
    private CancellationToken _cancellationToken;

    public Bootstrap(ILogger<Bootstrap> logger)
    {
        _logger = logger;
        Start();
    }

    private async void Start()
    {
        DotEnv.Load();
        var secrets = DotEnv.Read();
        var connectionString = secrets["BD_CONNECTION_STRING"];
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        var dataSource = dataSourceBuilder.Build();
        var conn = await dataSource.OpenConnectionAsync(_cancellationToken);
        _logger.Log(LogLevel.Information,$"BD CONNECTION {conn.Host}");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("BD>>>Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}