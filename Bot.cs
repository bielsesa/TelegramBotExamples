using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Exceptions;
using System.Collections.Generic;
using System.Timers;

namespace TelegramBot
{
    class Bot
    {
        private static ITelegramBotClient botClient;
        private static Dictionary<long, string> registeredUsers = new Dictionary<long, string>();

        static void Main(string[] args)
        {
            botClient = new TelegramBotClient("953705394:AAFtLctlHehkU9qdI06-QLYPr-cGHel0tQY");
            var me = botClient.GetMeAsync().Result;

            // event handler for when the bot receives a message
            botClient.OnMessage += BotClient_OnMessage;
            botClient.OnReceiveError += BotClient_OnReceiveError;

            // bot starts listening for messages            
            try
            {
                botClient.StartReceiving();
            }
            catch (ApiRequestException)
            {
                Console.WriteLine("The Bot token is invalid.");
                Console.ReadLine();
                return;
            }

            // create timer to send the current time every 30 minutes
            Timer halfHourTimer = new Timer();
            halfHourTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            halfHourTimer.Interval = 1800000;
            halfHourTimer.Enabled = true;

            Console.WriteLine($"Bot number {me.Id} with name {me.FirstName} started. Press any key to close.");
            Console.ReadLine();
        }

        private async static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            foreach(KeyValuePair<long,string> pair in registeredUsers)
            {
                await botClient.SendTextMessageAsync(
                    chatId: pair.Key,
                    text: String.Format("Hola! La hora actual es: {0:HH:mm:ss}", DateTime.Now)
                );
            }            
        }

        // Keep alive method. In case the bot indicates a receive error, it starts it again.
        private static void BotClient_OnReceiveError(object sender, Telegram.Bot.Args.ReceiveErrorEventArgs e)
        {
            if (!botClient.IsReceiving)
            {
                botClient.StartReceiving();
            }
        }

        // async OnMessage event task for the bot
        // it has to be async so the bot can be always listening
        private async static void BotClient_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            long currentChatId = e.Message.Chat.Id;

            // Check whether the user has already been registered or not.
            // If not, save the UserId (actually, it's the chat id)
            if (!registeredUsers.ContainsKey(currentChatId))
            {
                registeredUsers.Add(currentChatId, "");

                Console.WriteLine($"User registered with ID {currentChatId}.");

                await botClient.SendTextMessageAsync
                (
                    chatId: currentChatId,
                    text: "You were registered!"
                );
            }

            if (e.Message.Text == "/showusers")
            {
                string currentRegistUsers = "These are the currently registered users:";

                foreach (KeyValuePair<long,string> pair in registeredUsers)
                {
                    currentRegistUsers += $"\n- *{pair.Key}*";
                }

                await botClient.SendTextMessageAsync
                (
                    chatId: currentChatId,
                    text: currentRegistUsers
                );
            }

            if (e.Message.Text == "/whattime")
            {
                string currentRegistUsers = "These are the currently registered users:";

                foreach (KeyValuePair<long,string> pair in registeredUsers)
                {
                    currentRegistUsers += $"\n- *{pair.Key}*";
                }

                await botClient.SendTextMessageAsync
                (
                    chatId: currentChatId,
                    text: currentRegistUsers
                );
            }
        }
    }
}
