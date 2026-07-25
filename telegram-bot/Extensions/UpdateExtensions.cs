using Telegram.Bot.Types;

namespace telegram_bot.Extensions;

public static class UpdateExtensions
{
    public static long GetChatId(this Update update)
    {
        ArgumentNullException.ThrowIfNull(update);

        return update.Message?.Chat.Id
               ?? update.CallbackQuery?.Message?.Chat.Id
               ?? throw new InvalidOperationException(
                   $"Update '{update.Id}' does not contain a chat.");
    }
}