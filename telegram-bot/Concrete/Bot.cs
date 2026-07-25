using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace telegram_bot.Concrete;

public partial class Bot(ITelegramBotClient client)
{
    private readonly Dictionary<string, Func<Update, Task>> _actions = new();

    private readonly List<(Regex Pattern, Func<Update, Task> Action)> _regexActions
        = [];

    private Func<Update, Task>? _defaultAction;


    public void AddAction(string key, Func<Update, Task> action)
    {
        _actions[key] = action;
    }

    public void AddRegexAction(Regex pattern, Func<Update, Task> action)
    {
        _regexActions.Add((pattern, action));
    }


    public async Task HandleWebhookUpdateAsync(Update update)
    {
        if (_defaultAction != null) await _defaultAction.Invoke(update);
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
            await InvokeTextMessages(update);
        else if (update.Type == UpdateType.CallbackQuery && update.CallbackQuery?.Data != null)
            await InvokeCallbackQuery(update);
    }

    private async Task InvokeTextMessages(Update update)
    {
        if (update.Type != UpdateType.Message || update.Message?.Text == null)
            return;
        var messageText = update.Message.Text.Trim();
        if (_actions.TryGetValue(messageText, out var action))
            await action.Invoke(update);
        else
            foreach (var (pattern, regexAction) in _regexActions)
                if (pattern.IsMatch(messageText))
                {
                    await regexAction.Invoke(update);
                    break;
                }
    }

    private async Task InvokeCallbackQuery(Update update)
    {
        if (update.Type != UpdateType.CallbackQuery || update.CallbackQuery?.Data == null)
            return;
        var callbackData = update.CallbackQuery.Data.Trim();
        if (_actions.TryGetValue(callbackData, out var action))
            await action.Invoke(update);
        else
            foreach (var (pattern, regexAction) in _regexActions)
            {
                if (!pattern.IsMatch(callbackData)) continue;
                await regexAction.Invoke(update);
                break;
            }

        await client.AnswerCallbackQuery(update.CallbackQuery.Id);
    }
}