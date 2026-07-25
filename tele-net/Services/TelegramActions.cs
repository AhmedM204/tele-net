using telegram_bot.Concrete;
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
        var chatId = update?.Message?.Chat.Id ?? -1;
        if (chatId == -1) return;
        await bot.SendMessageAsync(chatId, "Hey");
    }
}