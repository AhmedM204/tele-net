using telegram_bot.Concrete;
using telegram_bot.Extensions;
using Telegram.Bot.Types;

namespace tele_net.Services;

public class TelegramActions(Bot bot)
{
    public void Register()
    {
        bot.AddAction("/start", Test);
    }

    private async Task Test(Update update)
    {
        await bot.SendMessageAsync(update.GetChatId(), "Hey");
    }
}