using Microsoft.AspNetCore.Mvc;
using telegram_bot.Concrete;
using Telegram.Bot.Types;

namespace tele_net.Controllers;

[Route("api/telegram/webhook")]
[ApiController]
public class TelegramWebhookController(Bot bot, IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        var expectedSecret = configuration["Telegram:WebhookSecret"];
        var receivedSecret = Request.Headers["X-Telegram-Bot-Api-Secret-Token"].ToString();

        if (!string.IsNullOrEmpty(expectedSecret) && receivedSecret != expectedSecret) return Unauthorized();

        await bot.HandleWebhookUpdateAsync(update);
        return Ok();
    }
}