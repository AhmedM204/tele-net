using Scalar.AspNetCore;
using tele_net.Services;
using telegram_bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddTelegramServices(builder.Configuration);
builder.Services.AddSingleton<TelegramActions>();
builder.Services.AddHostedService<TelegramWebhookService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var telegramActions = scope.ServiceProvider
        .GetRequiredService<TelegramActions>();

    telegramActions.Register();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", context =>
    {
        context.Response.Redirect("/scalar/v1");
        return Task.CompletedTask;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();