using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegram_bot.Concrete;

public partial class Bot
{
    public async Task SendMessageAsync(string chatId, string message, ReplyMarkup? replyMarkup = null)
    {
        try
        {
            await client.SendMessage(
                chatId,
                message,
                ParseMode.Html,
                replyMarkup: replyMarkup
            );
        }
        catch
        {
            // ignored
        }
    }


    public void AddDefaultAction(Func<Update, Task> action)
    {
        _defaultAction = action;
    }


    public async Task EditMessageTextWithoutDelete(string chatId, int messageId, string newText,
        InlineKeyboardButton[][]? replyMarkup = null)
    {
        try
        {
            await client.EditMessageText(chatId, messageId, newText, replyMarkup: replyMarkup);
        }
        catch
        {
            await DeleteMessage(chatId, messageId);
            await SendMessageAsync(chatId, newText, replyMarkup);
        }
    }

    public async Task EditMessageTextWithDelete(string chatId, int messageId, string newText,
        InlineKeyboardButton[][]? replyMarkup = null)
    {
        try
        {
            await client.DeleteMessage(chatId, messageId);
            await SendMessageAsync(chatId, newText, replyMarkup);
        }
        catch
        {
            await SendMessageAsync(chatId, newText);
        }
    }

    public async Task MessageBox(Update? update, string message)
    {
        if (update == null) return;
        var chatId = update?.Message?.Chat.Id.ToString() ?? update?.CallbackQuery?.Message?.Chat.Id.ToString();
        if (chatId == null) return;
        var callbackId = update?.CallbackQuery?.Id;
        if (callbackId == null) return;

        await client.AnswerCallbackQuery(callbackId, message, true);
    }


    public async Task ClearUpdates()
    {
        await client.GetUpdates(-1);
    }

    public async Task SendPhotoAsync(string chatId, string photoUrl, string? caption = null,
        ReplyMarkup? replyMarkup = null)
    {
        try
        {
            await client.SendPhoto(chatId, photoUrl, caption, replyMarkup: replyMarkup);
        }
        catch
        {
            // ignored
        }
    }

    public async Task EditPhotoAsync(string chatId, int messageId, string photoUrl, string? caption = null,
        InlineKeyboardButton[][]? replyMarkup = null)
    {
        try
        {
            await client.EditMessageMedia(chatId, messageId, new InputMediaPhoto(photoUrl), replyMarkup);
            await client.EditMessageCaption(chatId, messageId, caption, ParseMode.Html);
            await client.EditMessageReplyMarkup(chatId, messageId, replyMarkup);
        }
        catch
        {
            await SendPhotoAsync(chatId, photoUrl, caption);
        }
    }

    public async Task DeleteMessage(string chatId, int messageId)
    {
        try
        {
            await client.DeleteMessage(chatId, messageId);
        }
        catch
        {
            // ignored
        }
    }


    public async Task<ChatMember[]> GetChatAdministratorsAsync(long chatId)
    {
        try
        {
            var admins = await client.GetChatAdministrators(chatId);
            return admins.ToArray();
        }
        catch
        {
            return Array.Empty<ChatMember>();
        }
    }

    public async Task SendDocumentAsync(string chatId, string htmlContent, string fileName = "document.html",
        string? caption = null, ReplyMarkup? replyMarkup = null)
    {
        try
        {
            var fileBytes = Encoding.UTF8.GetBytes(htmlContent);
            using var stream = new MemoryStream(fileBytes);

            var inputFile = new InputFileStream(stream, fileName);

            await client.SendDocument(
                chatId,
                inputFile,
                caption,
                ParseMode.Html,
                replyMarkup: replyMarkup
            );
        }
        catch
        {
            // ignored
        }
    }
}