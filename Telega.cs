using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
//using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.Enums;
using static HW9._4_BOT_Advansed.Loger;

namespace HW9._4_BOT_Advansed
{
    internal class Telega
    {
        //https://telegrambots.github.io/book/1/quickstart.html
        //https://github.com/TelegramBots/Telegram.Bot
        TelegramBotClient botClient;
        string token;
        int updateOffset=0;
        string fileResivedPatch;
        

        public Telega(string tokenFileLocalPath,string fileResivedPatch)
        {
            this.fileResivedPatch = fileResivedPatch;
            token = System.IO.File.ReadAllText(tokenFileLocalPath);
            botClient = new TelegramBotClient(token);
            Log($"START BOT AT {DateTime.Now}");
            RunLoopUpdates();
            Log("any more");
            Console.ReadKey();
            Log($"STOP BOT AT {DateTime.Now}");

        }
        /// <summary>
        /// основной жизненный цикл бота
        /// </summary>
        private async void RunLoopUpdates()
        {
            while (true)
            {
                //Log("=====get updates===");
                Update[] updates = await botClient.GetUpdatesAsync(offset: updateOffset, limit: 10, timeout: 3);
                foreach (var update in updates)
                {
                    updateOffset = update.Id+1;
                    Log($"+++ Input update. ChatID:{update.Message.Chat.Id} UserID:{update.Message.From.Id}, FirstName:{update.Message.From.FirstName} updateOffset:{updateOffset} Type:{update.Message.Type.ToString()} Text:{update.Message.Text}"); //Обработка апдейтов
                    switch (update.Message.Type)
                    {
                        case MessageType.Text: //Обработка ТЕКСТОВЫХ сообщений
                            SendMessage(update.Message.Chat.Id, update.Message.Text);
                            break;
                        case MessageType.Photo: //Обработка ФОТОГРАФИЙ
                        case MessageType.Document: //Обработка документов
                        case MessageType.Voice: //Обработка документов
                            ResiveFiles(update);
                            break;
                        default:
                            SendMessage(update.Message.Chat.Id, "Не понял юмара!");
                            break;
                    }
                }
                Thread.Sleep(2000);
            }
        }


        /// <summary>
        /// Получаем данные о файлах из сообщения Update
        /// </summary>
        /// <param name="update">Полученный и обрабатываемый апдейт</param>
        private void ResiveFiles(Update update)
        {
            int FileSize, Duration;
            string FileId;
            string FileName;
            string Caption;
            string dataAndTime;
            Caption = update.Message.Caption;
            dataAndTime = update.Message.Date.ToLocalTime().ToString().Replace(':', '.');

            switch (update.Message.Type)
            {
                case MessageType.Photo: //Обработка ФОТОГРАФИЙ
                    string tempName;
                    if (string.IsNullOrEmpty(Caption))
                    {
                        tempName = dataAndTime;
                    }
                    else
                    {
                        tempName = Caption + " " + dataAndTime;
                    }
                    FileName = "Photo "+ tempName + ".jpg";
                    SendMessage(update.Message.Chat.Id, $"Это фотография! Имя: {FileName}, Текст под ней: {Caption}");
                    PhotoSize photoSmall = update.Message.Photo[0];
                    DownloadFile(photoSmall.FileId,$@"{fileResivedPatch}\preview\{FileName}");
                    PhotoSize photoLage;
                        photoLage = update.Message.Photo[update.Message.Photo.Count() - 1];
                        DownloadFile(photoLage.FileId, $@"{fileResivedPatch}\{FileName}");
                        SendFile(update.Message.Chat.Id, $@"{fileResivedPatch}\{FileName}");
                    break;
                case MessageType.Document: //Обработка документов
                    FileSize = (int)update.Message.Document.FileSize;
                    FileId = update.Message.Document.FileId;
                    FileName = update.Message.Document.FileName;
                    SendMessage(update.Message.Chat.Id, $"Это документ! Имя: {FileName}, Текст под ней: {Caption}");
                    Log($"FileName:{FileName}, FileSize:{FileSize}, FileId:{FileId}");
                    DownloadFile(FileId, $@"{fileResivedPatch}\{FileName}");

                    break;
                case MessageType.Voice: //Обработка голосовых
                    FileSize = (int)update.Message.Voice.FileSize;
                    FileId = update.Message.Voice.FileId;
                    Duration = update.Message.Voice.Duration;
                    FileName = "Voice_" + update.Message.Date.ToLocalTime().ToString().Replace(':', '.') + ".mp3";
                    SendMessage(update.Message.Chat.Id, $"Это голосовое! Имя: {FileName}, Текст под ней: {Caption}");
                    Log($"FileName:{FileName}, FileSize:{FileSize}, Duration:{Duration}, FileId:{FileId}");
                    DownloadFile(FileId, $@"{fileResivedPatch}\{FileName}");
                    break;
                default:
                    SendMessage(update.Message.Chat.Id, "Не понял юмара! 2");
                    break;
            }

            
        }
        /// <summary>
        /// Скачивает файлы некоторых типов на локальный диск
        /// </summary>
        /// <param name="FileId">берётся из полученного сообщения update.Message.|Photo[0]/Voice/Document|.FileId</param>
        /// <param name="fileName">полное имя файла от каталога запуска в данном случае</param>
        private async void DownloadFile(string FileId, string fileName)
        {

            CreateSupportingDirectory(fileName);

            File file = await botClient.GetFileAsync(FileId);

            int fileSize = (int)file.FileSize;

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            if (!fileInfo.Exists)
            {

                System.IO.FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create);
                await botClient.DownloadFileAsync(file.FilePath, fileStream);
                fileStream.Close();
                Log($"Скачивание файла на диск: {fileName}  fileSize:{fileSize}  FileId:{FileId}");
            }
            else
            {
                Log($"Файл уже имеется: {fileName}  fileSize:{fileSize}  FileId:{FileId}");
            }
        }

        /// <summary>
        /// Отправка сообщения юзеру чата
        /// </summary>
        /// <param name="chatId">берётся например из полученного ранее update.Message.Chat.Id</param>
        /// <param name="msg">Сообщение для пользователя чата</param>
        private async void SendMessage(long chatId, string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Log($"==> Send message: {msg}\n \t for chatID: {chatId}");
                await botClient.SendTextMessageAsync(chatId, msg);
            }
        }
        /// <summary>
        /// Отправка файла с коротким именем в подписи
        /// </summary>
        /// <param name="chatId">берётся например из полученного ранее update.Message.Chat.Id</param>
        /// <param name="fileName">полное имя файла от каталога запуска в данном случае</param>
        private async void SendFile(long chatId, string fileName)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);
            string shortFileName = System.IO.Path.GetFileName(fileName);
            int count = 0;
            while (!fileInfo.Exists)
            {
                Thread.Sleep(1000);
                fileInfo = new System.IO.FileInfo(fileName);
                count++;
                if (count>3)
                {
                    Log($"Файл на отправку не был найден. {fileName}");
                    return;
                }
            }
            Thread.Sleep(5000);
            Log($"==> Send File: {fileName}\n \t for chatID: {chatId}");
            System.IO.FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
            InputOnlineFile inputOnlineFile = new InputOnlineFile(fileStream);
            await botClient.SendPhotoAsync(chatId, inputOnlineFile, shortFileName);
            fileStream.Close();
        }



    }
}
