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
using Telegram.Bot.Types.ReplyMarkups;
using static HW9._4_BOT_Advansed.Loger;

namespace HW9._4_BOT_Advansed
{
    internal class Telega
    {
        //https://telegrambots.github.io/book/1/quickstart.html
        //https://github.com/TelegramBots/Telegram.Bot
        TelegramBotClient botClient;
        private static string token;
        int updateOffset=0;
        public static string fileResivedPatch;
        Dictionary<long, User> MyUsers = new Dictionary<long, User>();    //<ChatID,User> Пользователи

        public Telega(string tokenFileLocalPath,string fileResivedPatch_)
        {
            fileResivedPatch = fileResivedPatch_;
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
            
            Message message;
            while (true)
            {
                //Log("=====get updates===");
                Update[] updates = await botClient.GetUpdatesAsync(offset: updateOffset, limit: 10, timeout: 3);
                foreach (var update in updates)
                {
                    //Обработка апдейтов
                    Log($"+++ Input update");
                    updateOffset = update.Id + 1; //подтверждение приёма последнего апдейта (в любом случае по циклу пройдут все апдейты, последний ID+1 потом передаётся со следующим запросом.)
                    //Фильтр АПДЕЙТОВ
                    if (update.CallbackQuery != null)               //Обработка нажатий на кнопки Inline (превращает нажатия на кнопки inline в текстовое сообщение)
                    {
                        message = update.CallbackQuery.Message;
                        message.Text = update.CallbackQuery.Data;
                        Log($"CallbackQuery Chat.Id:{message.Chat.Id} MessageText:{message.Text}");
                        //SendMessage(message.Chat.Id, $"Вы выбрали опцию {message.Text}");
                    } else if (update.Message != null)              //Обычные сообщения (текст или документ) или нажата клавиатурная кнопка (по сути то-же сообщение)
                    {
                        message = update.Message;
                        Log($"Message Chat.Id:{message.Chat.Id} MessageText:{message.Text}");
                    } else if (update.MyChatMember != null)         //Что-то с чат мембером (например вышел из чата, забанился)
                    {
                        long ChatId = update.MyChatMember.Chat.Id;
                        long UserId = update.MyChatMember.From.Id;
                        string FirstName = update.MyChatMember.From.FirstName;
                        ChatMemberStatus chatMemberStatus = update.MyChatMember.NewChatMember.Status;
                        string Status = chatMemberStatus.ToString();
                        //сделать обработку изменяющихся статусов!!!
                        Log($"MyChatMember FirstName:{FirstName} Chat.Id:{ChatId} UserId:{UserId} NewStatus:{Status}");
                        continue;
                    } else //Не понятное действие
                    {
                        Log($"ERROR Update Filter else branch, unknow UPDATE TYPE");
                        continue;
                    }
                    //ОБРАБОТКА ОБЫЧНЫХ АПДЕЙТОВ
                     

                    Log($"Message Type:{message.Type.ToString()}  Text:{message.Text}. ChatID:{message.Chat.Id} UserID:{message.From.Id}, FirstName:{message.From.FirstName} updateOffset:{updateOffset} ");

                    switch (message.Type)
                    {
                        case MessageType.Text: //Обработка ТЕКСТОВЫХ сообщений
                            Commands(message);
                            break;
                        case MessageType.Photo: //Обработка входящих ФОТОГРАФИЙ
                        case MessageType.Document: //Обработка входящих документов
                        case MessageType.Voice: //Обработка входящих документов
                            ResiveFiles(message);
                            break;
                        default:
                            Log($"ERROR default branch in switch (message.Type) Message Type:{message.Type.ToString()}");
                            SendMessage(message.Chat.Id, "Не понял юмара!");
                            break;
                    }
                }
                Thread.Sleep(2000);
            }
        }
        /// <summary>
        /// Обработка команд от пользователя
        /// </summary>
        /// <param name="message">входящее сообщение текущего апдейта со всей инф-й</param>
        private void Commands(Message message) 
        {
            string s;
            switch (message.Text.ToUpper())
            {
                case "/START":
                        Log($"start");
                        SendMessageMainMenuButtons(message.Chat.Id, "старт", keyboardMainMenuButtons);
                    break;
                case "/СПИСОК ФАЙЛОВ":
                    Log($"СПИСОК");
                    SendMessageInlineKeyboard(message.Chat.Id, "Выберете тип файлов для просмотра списка или загрузки эскизов!", fileChooseButtons);
                    break;
                case "/ПОГОДА":
                    Log($"ПОГОДА");
                    SendMessage(message.Chat.Id, "погода");
                    break;
                case "/HELP":
                    Log($"HELP");
                    SendMessage(message.Chat.Id, "HELP");
                    break;
                case "/НОМЕР РЕГИОН":
                    Log($"НОМЕР РЕГИОН");
                    SendMessage(message.Chat.Id, "НОМЕР РЕГИОН");
                    break;
                case "/РАЗВЛЕЧЕНИЯ":
                    Log($"РАЗВЛЕЧЕНИЯ");
                    SendMessage(message.Chat.Id, "РАЗВЛЕЧЕНИЯ");
                    break;
                case "/КАРТИНКИСПИСОК":
                    Log($"КАРТИНКИ СПИСОК");
                    s = FileList("ResivedFiles", 0,RequestFromInlineBtn.EtypeOfFileFilter.JPG);
                    SendMessageInlineKeyboard(message.Chat.Id, $"Список картинок:\n{s}\nВыберете картинку!", fileListButtons);
                    break;
                case "/КАРТИНКИ":
                    Log($"КАРТИНКИ");
                    SendMessage(message.Chat.Id, "КАРТИНКИ");
                    break;
                case "/ГОЛОСОВЫЕ":
                    Log($"ГОЛОСОВЫЕ");
                    s = FileList("ResivedFiles", 0, RequestFromInlineBtn.EtypeOfFileFilter.OGG);
                    SendMessageInlineKeyboard(message.Chat.Id, $"Список файлов:\n{s}\nВыберете голосовое сообщение!", fileListButtons);
                    break;
                case "/ДОКУМЕНТЫ":
                    Log($"ДОКУМЕНТЫ");
                    SendMessage(message.Chat.Id, "ДОКУМЕНТЫ");
                    break;
                case "/ВСЕ":
                    Log($"ВСЕ");
                    s = FileList("ResivedFiles", 0, RequestFromInlineBtn.EtypeOfFileFilter.ALL);
                    SendMessageInlineKeyboard(message.Chat.Id, $"Список файлов:\n{s}\nВыберете файл!", fileListButtons);
                    break;
                default:
                break;
            }

            
        }


        /// <summary>
        /// Получаем данные о файлах из сообщения Update
        /// </summary>
        /// <param name="update">Полученный и обрабатываемый апдейт</param>
        private void ResiveFiles(Message message)
        {
            int FileSize, Duration;
            string FileId;
            string FileName;
            string Caption;
            string dataAndTime;
            Caption = message.Caption;
            dataAndTime = message.Date.ToLocalTime().ToString().Replace(':', '.');

            switch (message.Type)
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
                    SendMessage(message.Chat.Id, $"Это фотография! Имя: {FileName}, Текст под ней: {Caption}");
                    PhotoSize photoSmall = message.Photo[0];
                    DownloadFile(photoSmall.FileId,$@"{fileResivedPatch}\preview\{FileName}");
                    PhotoSize photoLage;
                    photoLage = message.Photo[message.Photo.Count() - 1];
                    DownloadFile(photoLage.FileId, $@"{fileResivedPatch}\{FileName}");
                    //SendFile(message.Chat.Id, $@"{fileResivedPatch}\{FileName}");  //СДЕЛАТЬ ДЛЯ ВСЕХ ПОЛЬЗОВАТЕЛЕЙ РАССЫЛКУ
                    ReSendFilePhoto(message.Chat.Id, photoLage.FileId, $"Пользователь сохранил файл.\n{FileName}");
                    break;
                case MessageType.Document: //Обработка документов
                    FileSize = (int)message.Document.FileSize;
                    FileId = message.Document.FileId;
                    FileName = message.Document.FileName;
                    SendMessage(message.Chat.Id, $"Это документ! Имя: {FileName}, Текст под ней: {Caption}");
                    Log($"FileName:{FileName}, FileSize:{FileSize}, FileId:{FileId}");
                    DownloadFile(FileId, $@"{fileResivedPatch}\{FileName}");

                    break;
                case MessageType.Voice: //Обработка голосовых
                    FileSize = (int)message.Voice.FileSize;
                    FileId = message.Voice.FileId;
                    Duration = message.Voice.Duration;
                    FileName = "Voice_" + dataAndTime + ".ogg";
                    SendMessage(message.Chat.Id, $"Это голосовое! Имя: {FileName}, Текст под ней: {Caption}");
                    Log($"FileName:{FileName}, FileSize:{FileSize}, Duration:{Duration}, FileId:{FileId}");
                    DownloadFile(FileId, $@"{fileResivedPatch}\{FileName}");
                    break;
                default:
                    SendMessage(message.Chat.Id, "Не понял юмара! 2");
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
                Log($"ОШИБКА Файл уже имеется: {fileName}  fileSize:{fileSize}  FileId:{FileId}");
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

        private async void SendMessageMainMenuButtons(long chatId, string msg, KeyboardButton[][] kb)
        {
           ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(kb) { ResizeKeyboard = true };

           Log($"==> SendMessageMainMenuButtons: {msg}\n \t for chatID: {chatId}");
           await botClient.SendTextMessageAsync(chatId, msg, replyMarkup: replyKeyboardMarkup);
        }

        private async void SendMessageInlineKeyboard(long chatId, string msg, InlineKeyboardButton[][] inlineKeyboards)
        {

                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(inlineKeyboards);
                Log($"==> SendMessageInlineKeyboard: {msg}\n \t for chatID: {chatId}");
                await botClient.SendTextMessageAsync(chatId, msg, replyMarkup: inlineKeyboard);
        }

        /// <summary>
        /// Отправка файла с коротким именем в подписи
        /// </summary>
        /// <param name="chatId">берётся например из полученного ранее update.Message.Chat.Id</param>
        /// <param name="fileName">полное имя файла от каталога запуска в данном случае</param>
        private async void SendFilePhoto(long chatId, string fileName)
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
            //await botClient.SendVoiceAsync(chatId, inputOnlineFile, shortFileName);
            fileStream.Close();
        }

        private async void ReSendFilePhoto(long chatId, string FileId, string msg)
        {
            Thread.Sleep(1000);
            Log($"==> ReSend File FileId: {FileId}\n \t for chatID: {chatId}");
            //InputTelegramFile inputTelegramFile = new InputTelegramFile(FileId);
            //Log($" FileName:{inputTelegramFile.FileName} FileType:{inputTelegramFile.FileType} FileId:{inputTelegramFile.FileId} ToString:{inputTelegramFile}");
            await botClient.SendPhotoAsync(chatId, FileId, msg);
        }

        private async void SendPhotoPreviews(long chatId)
        {
            throw new Exception("Не доделал");
            string filesPath = $@"{fileResivedPatch}\preview\";
            Log($"==> Send Photo Previews from: {filesPath}\n \t for chatID: {chatId}");
            //Photo 25.04.2022 21.01.03.jpg
            //Photo Кокш 25.04.2022 21.21.19.jpg
            //Photo_25.04.2022 19.59.04.jpg
            //InputMedia media = new InputMedia();
            //InputMediaPhoto photo = new InputMediaPhoto();
            IAlbumInputMedia[] albumInputMedias = new IAlbumInputMedia[]
                {
                    new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/06/20/19/22/fuchs-2424369_640.jpg"){ Caption = "ОДЫН"},
                    new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/04/11/21/34/giraffe-2222908_640.jpg"){ Caption = "ДУА"},
                    new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/06/20/19/22/fuchs-2424369_640.jpg"){ Caption = "3 3 3 3 3"},
                    new InputMediaPhoto("https://cdn.pixabay.com/photo/2017/04/11/21/34/giraffe-2222908_640.jpg"){ Caption = "4 4 4 4 4"},
                };

            await botClient.SendMediaGroupAsync(chatId, albumInputMedias);
        }
    }
}
