# Tele-Net

A small, lightweight, and flexible Telegram bot layer for .NET.

Tele-Net provides an **abstraction layer above [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot)**. It handles Telegram updates and actions while keeping your application logic separate.

The goal is to make Telegram integration simple, clean, and easy to extend.

## Features

* Receives Telegram updates through webhooks.
* Supports normal Telegram actions and commands.
* Supports regex-based actions.
* Supports a default action.
* Provides common Telegram operations such as:

  * Sending messages
  * Sending photos
  * Sending documents
* Works with Dependency Injection.
* Supports async actions.
* Lightweight and easy to extend.
* Keeps Telegram code separate from application logic.

## Installation

Add the Tele-Net projects to your solution and register the Telegram services using Dependency Injection.

You also need a Telegram Bot Token.

### 1. Create a Telegram Bot

Create a bot using **BotFather** on Telegram and get your bot token.

### 2. Add the Bot Token

Add your bot token to your `appsettings.json`:

```json
{
  "Telegram": {
    "Token": "YOUR_BOT_TOKEN"
  }
}
```

**Never commit your real bot token to a public repository.**

For local development, use `User Secrets` or environment variables if possible.

## Project Structure

The main Telegram abstraction is located in:

```text
telegram-bot/
```

Your application-specific Telegram events should be registered in:

```text
tele-net/Services/TelegramActions.cs
```

For example:

```csharp
public void Register()
{
    bot.AddAction("/test", Test);
    bot.AddAction("/something", Something);
}
```

Each action contains the logic for its Telegram event.

For example:

```csharp
private async Task Test(Update update)
{
    var chatId = update.GetChatId();

    await bot.SendMessageAsync(
        chatId,
        "Test successful."
    );
}
```

## Keep Business Logic in the Core Layer

Telegram actions should usually stay small.

If an action needs more complex logic, move that logic to your `Core` layer and call it from `TelegramActions`.

```text
Telegram
   |
   v
TelegramActions
   |
   v
Core Services
   |
   v
Application Logic
```

For example:

```csharp
private async Task GenerateReport(Update update)
{
    var report = await reportService.GenerateAsync();

    await bot.SendDocumentAsync(
        update.GetChatId(),
        report
    );
}
```

This keeps Telegram-specific code simple and makes the application easier to maintain and test.

## Local Development

Telegram webhooks require a **public HTTPS URL**.

Your local computer is normally not reachable from Telegram, so Tele-Net uses **ngrok** during development.

### Install ngrok

Download ngrok from the official website:

https://ngrok.com/download

After installation, check that it is available:

```bash
ngrok version
```

You also need to configure your ngrok account and authentication token.

During Development, the application can start ngrok automatically and use its public HTTPS URL for the Telegram webhook.

You do **not** need ngrok in Production if your application already has a public HTTPS domain.

## Production

In Production, your application should use a public HTTPS domain instead of ngrok.

Set the webhook URL in your Production configuration:

```json
{
  "Telegram": {
    "Token": "YOUR_BOT_TOKEN",
    "WebhookUrl": "https://api.example.com/"
  }
}
```

The webhook URL must be publicly accessible over ****HTTPS****.

For example:

```text
https://api.example.com/api/telegram/webhook
```

Make sure your server, reverse proxy, firewall, and HTTPS certificate allow Telegram to reach this endpoint.

## Configuration

A typical configuration looks like:

```json
{
  "Telegram": {
    "Token": "YOUR_BOT_TOKEN",
    "WebhookSecret": "MySuperSecretToken123",
    "WebhookUrl": "https://api.example.com/"
  }
}
```

### Development

```text
BotToken
    +
ngrok
    ↓
Temporary public HTTPS URL
    ↓
Telegram Webhook
```

### Production

```text
BotToken
    +
Public HTTPS Domain
    ↓
Telegram Webhook
```

## Security

Never commit sensitive information to a public repository.

This includes:

* Telegram Bot Token
* API keys
* Passwords
* Database connection strings
* Webhook secrets
* Private keys

For local development, use:

* .NET User Secrets
* Environment variables

For Production, use:

* Environment variables
* Your hosting provider's secret manager
* Another secure secret management system

Do not put real secrets directly in `appsettings.json` if the file is tracked by Git.

## Why Tele-Net?

Tele-Net is designed to be:

* **Simple**
* **Lightweight**
* **Flexible**
* **Async-friendly**
* **Easy to extend**
* **Easy to integrate**
* **Separate from application logic**

You can keep small Telegram actions directly inside `TelegramActions`, or connect them to your own `Core` services when the application grows.

The library does not force a specific application architecture. You can organize your business logic based on your project's needs.

## Basic Flow

```text
Telegram
    |
    v
Webhook
    |
    v
Tele-Net
    |
    v
TelegramActions
    |
    v
Core Services
    |
    v
Application Logic
```

This separation lets Tele-Net handle Telegram-specific work while your Core layer handles the actual application logic.
