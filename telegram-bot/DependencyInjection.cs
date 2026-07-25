using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using telegram_bot.Concrete;
using Telegram.Bot;

namespace telegram_bot;

public static class DependencyInjection
{
    public static IServiceCollection AddTelegramServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<TelegramSettings>(configuration.GetSection("Telegram"));

        services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<TelegramSettings>>().Value;
            return new TelegramBotClient(settings.Token);
        });
        services.AddSingleton<Bot>();


        return services;
    }
}