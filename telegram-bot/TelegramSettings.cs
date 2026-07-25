namespace telegram_bot;

public class TelegramSettings
{
    public string Token { get; set; } = string.Empty;
    public string AdminGroupId { get; set; } = string.Empty;

    public string? WebhookUrl { get; set; }
    public string? WebhookSecret { get; set; }
}