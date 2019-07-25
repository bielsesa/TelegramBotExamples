using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    class Program
    {
        static ITelegramBotClient botClient;
        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("953705394:AAFtLctlHehkU9qdI06-QLYPr-cGHel0tQY");
            var me = botClient.GetMeAsync().Result;

            // event handler for when the bot receives a message
            botClient.OnMessage += BotClient_OnMessage;

            // bot starts listening for messages
            botClient.StartReceiving();

            Console.WriteLine($"Bot number {me.Id} with name {me.FirstName} started. Press any key to close.");
            Console.ReadLine();
        }

        // async OnMessage event task for the bot
        // it has to be async so the bot can be always listening
        private async static void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            // Normal text message. Checks wether the message does have text.
            // in case it does, the bot answers.
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received text message in chat {e.Message.Chat.Id}.");

                // the bot sends asynchronously a message to the chat
                await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat.Id,
                        text: "You said:\n" + e.Message.Text
                    );
            }

            // Normal text message, with all the properties 
            // a text message can have.
            Message message = await botClient.SendTextMessageAsync(
              chatId: e.Message.Chat, // or a chat id: 123456789
              text: "Trying *all the parameters* of `sendMessage` method",
              parseMode: ParseMode.Markdown,
              disableNotification: true,
              replyToMessageId: e.Message.MessageId,
              replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithUrl(
                "Check sendMessage method",
                "https://core.telegram.org/bots/api#sendmessage"
              ))
            );

            // Almost all of the methods for sending messages return you 
            // the message you just sent.
            Console.WriteLine(
              $"{message.From.FirstName} sent message {message.MessageId} " +
              $"to chat {message.Chat.Id} at {message.Date.ToLocalTime()}. " + // if you don't use ToLocalTime(), the bot will show the UTC time.
              $"It is a reply to message {message.ReplyToMessage.MessageId} " +
              $"and has {message.Entities.Length} message entities."
            );
        }
    }
}
