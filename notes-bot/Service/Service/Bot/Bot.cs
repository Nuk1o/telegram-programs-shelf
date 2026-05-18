using dotenv.net;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Service.Bot;

public class Bot : BackgroundService
{
    private readonly ILogger<Bot> _logger;
    private TelegramBotClient _bot;
    private long _adminChat;

    public Bot(ILogger<Bot> logger)
    {
        _logger = logger;
        using var cts = new CancellationTokenSource();
        var secrets = DotEnv.Read();
        var botToken = secrets["TELEGRAM_BOT_TOKEN"];
        _adminChat = Convert.ToInt64(secrets["ADMIN_CHAT"]);
        Start(botToken,cts);
    }

    private async void Start(string botToken, CancellationTokenSource cts)
    {
        _bot = new TelegramBotClient(botToken,cancellationToken: cts.Token);
        var me = await _bot.GetMe();
        _bot.OnMessage += OnMessage;
        _logger.Log(LogLevel.Information, $"Info bot: {me}");
    }

    private async Task OnMessage(Message message, UpdateType type)
    {
        if (message.Text is null)
            return;
        _logger.Log(LogLevel.Information,$"User send message {message.Text}");
        if (message.Text == "ping")
        {
            BotSendMessage(message.Text, message.Chat.Id,message.From);
            BotSendMessage($"infoUser",message.Chat.Id,message.From);
        }
    }

    private void BotSendMessage(string message, long chatId, User? user)
    {
        switch (message)
        {
            case "ping":
                _bot.SendMessage(chatId, "pong");
                break;
            case "infoUser":
                if (user is null)
                    return;
                _bot.SendMessage(_adminChat,$"Ping user\n"
                                            +$"Id: {user.Id}\n" +
                                            $"User: {user.Username}\n" +
                                            $"IsBot: {user.IsBot}\n" +
                                            $"CanManageBots: {user.CanManageBots}\n" +
                                            $"CanJoinGroups: {user.CanJoinGroups}\n" +
                                            $"FirstName: {user.FirstName}\n" +
                                            $"IsPremium: {user.IsPremium}\n");
                _logger.Log(LogLevel.Information,$"Send new add\n {_bot.GetChat(new ChatId(_adminChat)).Result}");
                SaveInfoUser(user);
                break;
        }
    }

    private void SaveInfoUser(User user)
    {
        _logger.Log(LogLevel.Information,$"Id: {user.Id}\n" +
                                         $"User: {user.Username}\n" +
                                         $"IsBot: {user.IsBot}\n" +
                                         $"CanManageBots: {user.CanManageBots}\n" +
                                         $"CanJoinGroups: {user.CanJoinGroups}\n" +
                                         $"FirstName: {user.FirstName}\n" +
                                         $"IsPremium: {user.IsPremium}\n");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("BOT>>>Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}