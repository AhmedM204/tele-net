# Tele-Net 
A small and flexible Telegram bot layer for .NET.

This project provides an **abstraction layer above
[Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)**. It keeps
Telegram update handling and common bot commands in one simple place,
while allowing your application logic to stay separate.

## What it does

-   Receives Telegram updates through webhooks.
-   Supports simple actions such as commands.
-   Supports regex-based actions.
-   Supports a default action for other messages.
-   Provides common Telegram operations such as sending messages,
    photos, and documents.
-   Works well with Dependency Injection.
-   Is lightweight and easy to extend.

## Project Structure

The main Telegram abstraction is in:

``` text
telegram-bot/
```

Your application-specific Telegram events should be registered in:

``` text
tele-net/Services/TelegramActions.cs
```

For example:

``` csharp
public void Register()
{
    bot.AddAction("/test", Test);
    bot.AddAction("/something", Something);
}
```

Put the logic for each Telegram event in its action method.

You can also keep the action small and call services from your `Core`
layer. This is recommended when the logic becomes larger.

Example:

``` text
TelegramActions
      |
      v
Core Services
      |
      v
Application Logic
```

This keeps the Telegram layer simple and makes the project easier to
maintain.

## Local Development

Telegram webhooks need a public HTTPS URL. Your local computer is not
normally reachable from Telegram, so this project uses **ngrok** during
Development.

### Install ngrok

Download ngrok from the official website:

https://ngrok.com/download

After installation, make sure this works:

``` bash
ngrok version
```

You also need to configure your ngrok account and authentication token.

When the application runs in Development, it can start ngrok
automatically and use its public HTTPS URL for the Telegram webhook.

You do not need ngrok in Production if your application already has a
public HTTPS domain.

## Production

In Production, set the webhook URL to your public API address:

``` json
{
  "Telegram": {
    "WebhookUrl": "https://api.example.com/api/telegram/webhook"
  }
}
```

The URL must be publicly reachable over HTTPS.

Keep sensitive values such as the Telegram bot token and webhook secret
outside the public repository. Use environment variables, user secrets,
or your hosting provider's secret management.

## Why use this?

The goal is to keep Telegram integration:

-   **Simple**
-   **Lightweight**
-   **Flexible**
-   **Easy to extend**
-   **Separate from application logic**

You can use the built-in Telegram actions for small tasks, or connect
them to your own `Core` services for larger features.
