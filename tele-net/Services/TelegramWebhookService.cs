using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Options;
using telegram_bot;
using Telegram.Bot;

namespace tele_net.Services;

public class TelegramWebhookService(
    ITelegramBotClient botClient,
    IOptions<TelegramSettings> settings,
    IWebHostEnvironment env,
    ILogger<TelegramWebhookService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var baseUrl = settings.Value.WebhookUrl;
        var secret = settings.Value.WebhookSecret;

        if (string.IsNullOrEmpty(baseUrl) && env.IsDevelopment())
        {
            logger.LogInformation("No Webhook URL found in config. Auto-starting ngrok...");
            baseUrl = await GetOrStartNgrokAsync();
        }

        if (string.IsNullOrEmpty(baseUrl))
        {
            logger.LogWarning("Could not get an ngrok URL. Webhook registration skipped.");
            return;
        }

        var webhookUrl = $"{baseUrl.TrimEnd('/')}/api/telegram/webhook";
        logger.LogInformation("Setting up Telegram Webhook: {Url}", webhookUrl);

        await botClient.DeleteWebhook(true, cancellationToken);

        await botClient.SetWebhook(
            webhookUrl,
            secretToken: secret,
            dropPendingUpdates: true,
            cancellationToken: cancellationToken
        );

        logger.LogInformation("Telegram Webhook successfully registered!");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing Telegram Webhook...");
        await botClient.DeleteWebhook(cancellationToken: cancellationToken);
    }

    private async Task<string?> GetOrStartNgrokAsync()
    {
        using var httpClient = new HttpClient();

        try
        {
            var url = await FetchNgrokUrlAsync(httpClient);
            if (url != null) return url;
        }
        catch
        {
            // ignored
        }

        try
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "ngrok",
                    Arguments = "http 5236", 
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true
                }
            };
            process.Start();
            logger.LogInformation("Started ngrok in the background. Waiting for it to connect...");

            // 3. Smart Retry Loop (Try every 1 second, up to 5 times)
            for (var i = 0; i < 5; i++)
            {
                await Task.Delay(1000);

                if (process.HasExited)
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    logger.LogError("Ngrok crashed immediately! Error: {Error}", error);
                    logger.LogError("Did you run 'ngrok config add-auth-token YOUR_TOKEN' in the terminal?");
                    return null;
                }

                try
                {
                    var url = await FetchNgrokUrlAsync(httpClient);
                    if (url != null) return url;
                }
                catch
                {
                    /* Still waiting for ngrok to open port 4040 */
                }
            }

            logger.LogError("Ngrok started, but timed out waiting for a URL.");
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError("Failed to start ngrok process. Error: {Msg}", ex.Message);
            return null;
        }
    }

    private static async Task<string?> FetchNgrokUrlAsync(HttpClient httpClient)
    {
        var response = await httpClient.GetStringAsync("http://127.0.0.1:4040/api/tunnels");
        using var document = JsonDocument.Parse(response);

        var tunnels = document.RootElement.GetProperty("tunnels");
        return tunnels
            .EnumerateArray()
            .Select(tunnel => tunnel.GetProperty("public_url").GetString())
            .OfType<string>()
            .FirstOrDefault(publicUrl => publicUrl.StartsWith("https"));
    }
}