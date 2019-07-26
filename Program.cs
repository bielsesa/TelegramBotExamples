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
            #region Text Message
            if (e.Message.Text != null)
            {
                Console.WriteLine($"Received text message in chat {e.Message.Chat.Id}.");

                // the bot sends asynchronously a message to the chat
                await botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat.Id,
                        text: "You said:\n" + e.Message.Text
                    );
            }
            #endregion

            // Normal text message, with all the properties 
            // a text message can have.
            #region Text message with properties
            Message textMessage = await botClient.SendTextMessageAsync(
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
            #endregion

            // Almost all of the methods for sending messages return you 
            // the message you just sent.
            #region Messages return you the message you sent (as an object)
            Console.WriteLine(
              $"{textMessage.From.FirstName} sent message {textMessage.MessageId} " +
              $"to chat {textMessage.Chat.Id} at {textMessage.Date.ToLocalTime()}. " + // if you don't use ToLocalTime(), the bot will show the UTC time.
              $"It is a reply to message {textMessage.ReplyToMessage.MessageId} " +
              $"and has {textMessage.Entities.Length} message entities."
            );
            #endregion

            // Photo message. It gets the image from a URL on the internet.
            // All images can have captions. You can use HTML or Markdown on them.
            #region Photo Message
            Message photoMessage = await botClient.SendPhotoAsync(
              chatId: e.Message.Chat,
              photo: "https://github.com/TelegramBots/book/raw/master/src/docs/photo-ara.jpg",
              caption: "<b>Ara bird</b>. <i>Source</i>: <a href=\"https://pixabay.com\">Pixabay</a>",
              parseMode: ParseMode.Html
            );
            #endregion

            // Sticker message. Sticker files should be in WebP format.
            // Sends two messages. The first one gets the sticker by passing HTTP URL
            // to WebP sticker file, and the second by reusing the file_id of the
            // first sticker (kinda like a cache?)
            #region Sticker Messages
            Message stickerMessage1 = await botClient.SendStickerAsync(
              chatId: e.Message.Chat,
              sticker: "https://github.com/TelegramBots/book/raw/master/src/docs/sticker-fred.webp"
            );

            Message stickerMessage2 = await botClient.SendStickerAsync(
              chatId: e.Message.Chat,
              sticker: stickerMessage1.Sticker.FileId
            );
            #endregion

            // Audio message. Opens it in Media Player. MP3 Format.
            // Telegram can read metadata from the MP3 file (that's why
            // there are commented properties; the bot gets the same
            // properties, but from the file metadata!)
            #region Audio Message
            Message audioMessage = await botClient.SendAudioAsync(
                e.Message.Chat,
                "https://github.com/TelegramBots/book/raw/master/src/docs/audio-guitar.mp3"
                /* ,
                performer: "Joel Thomas Hunger",
                title: "Fun Guitar and Ukulele",
                duration: 91 // in seconds
                */
            );
            #endregion

            // Voice message. Not opened in Media Player (played directly
            // on the chat). OGG Format.
            // In this case, the OGG file is on our disk!
            #region Voice Message
            Message voiceMessage;
            using (var stream = System.IO.File.OpenRead("C:/Users/Biel/source/repos/TelegramBot/voice-nfl_commentary.ogg"))
            {
                voiceMessage = await botClient.SendVoiceAsync(
                  chatId: e.Message.Chat,
                  voice: stream,
                  duration: 36
                );
            }
            #endregion

            // You can send MP4 files as a regular video or as a video note.
            // Other video formats must be sent as a file.

            // Video message. They can have caption, reply, and reply markup
            // (like other multimedia messages)
            // You can optionally specify the duration and the resolution
            // of the video. In this case the video is streamed,
            // meaning the user can partly watch the video on stream, without
            // having to download it completely.
            Message videoMessage = await botClient.SendVideoAsync(
                    chatId: e.Message.Chat,
                    video: "https://raw.githubusercontent.com/TelegramBots/book/master/src/docs/video-countdown.mp4",
                    thumb: "https://raw.githubusercontent.com/TelegramBots/book/master/src/2/docs/thumb-clock.jpg",
                    supportsStreaming: true
             );

            // Video note message. They are shown in circles to the user.
            // They are usually short (1 minute or less).
            // You can send a video note only by the video file or
            // reusing the file_id of another video note.
            // (sending it by its HTTP URL is not supported currently)
            Message videoNoteMessage;
            using (var stream = System.IO.File.OpenRead("C:/Users/step/Source/Repos/TelegramBotExamples/video-waves.mp4"))
            {
                videoNoteMessage = await botClient.SendVideoNoteAsync(
                        chatId: e.Message.Chat,
                        videoNote: stream,
                        duration: 47,
                        length: 360 // value of width/height
                    );
            }
        }
    }
}
