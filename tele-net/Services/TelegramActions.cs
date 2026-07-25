using telegram_bot.Concrete;
using Telegram.Bot.Types;

namespace tele_net.Services;

public class TelegramActions(Bot bot)
{
    public void Register()
    {
        bot.AddAction("/test", Test);
        bot.AddAction("/something", Something);
    }

    private async Task Test(Update update)
    {
        var chatId = update.Message.Chat.Id;

        bot.SendMessageAsync(update.Message.Chat.Id.ToString(), update.Message.Text);
        // action
        await Task.CompletedTask;
    }

    private async Task Something(Update update)
    {
        // action
        await Task.CompletedTask;
    }
}