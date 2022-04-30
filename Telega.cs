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
using static HW9._4_BOT_Advansed.AuxiliaryClass;


namespace HW9._4_BOT_Advansed
{
    internal class Telega
    {
        //https://telegrambots.github.io/book/1/quickstart.html
        //https://github.com/TelegramBots/Telegram.Bot
        private TelegramBotClient botClient;
        private static string token;
        private int updateOffset =0;
        public static string fileResivedPatch;

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
                        UserManager.UnRegisterUser(UserId);
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
            if  (ExtractRequestFromInlineBtn(message.Text,out RequestFromInlineBtn req)) //пробуем обработать запрос типа /REQB:SF:10:JPG
            {
                if(req.typeOfReq == RequestFromInlineBtn.EtypeOfReq.SendFile) //это запрос файла
                {
                    SendMessage(message.Chat.Id, "Пожалуйста подождите, я обрабатываю ваш запрос!");
                    string fileNameToSend = GetFullFileName(fileResivedPatch, req.numberOfFile-1, req.typeOfFileFilter);
                    if (string.IsNullOrEmpty( fileNameToSend)) {Log($"ОШИБКА Имя файла не найдено") ; return; };
                    SendFileToUser(message.Chat.Id, fileNameToSend, req.typeOfFileFilter);
                }else if(req.typeOfReq == RequestFromInlineBtn.EtypeOfReq.ShowListFiles) //это запрос списка файлов (кнопки << >>)
                {
                    s = FileList(fileResivedPatch, req.numberOfFile, req.typeOfFileFilter);
                    SendMessageInlineKeyboard(message.Chat.Id, $"Список файлов:\n{s}\nВыберете файл!", fileListButtons);
                }else if (req.typeOfReq == RequestFromInlineBtn.EtypeOfReq.ShowPreviewsPhotos) //это запрос миниатюр фоток (кнопки << >>)
                {
                    SendPhotoPreviews(message.Chat.Id, req.numberOfFile, RequestFromInlineBtn.EtypeOfFileFilter.JPG);
                    SendMessageInlineKeyboard(message.Chat.Id, $"Выберете картинку (цифру)!", fileListButtons);
                }
            }
            UserManager.RegisterUser(message);
            switch (UserManager.GetMenuPosition(message)) //Если пользователь в подменю
            {
                case UserManager.EMenuPosition.RegionMenu: //Ищем по номеру региона название региона
                     SendMessage(message.Chat.Id, NomerRehiona.GetRegionNumber(message));
                    return;
                case UserManager.EMenuPosition.PogodaMenu: //Запрашиваем погоду у сервиса
                    UserManager.SetPogodaCity(message);
                    SendMessage(message.Chat.Id, Pogoda.GetPogoda(UserManager.GetPogodaCity(message),message));
                    return;
                default:
                    break;
            }
            switch (message.Text.ToUpper())
            {
                case "/START":
                        UserManager.SetMenuPosition(message, UserManager.EMenuPosition.MainMenu);
                        SendMessageMainMenuButtons(message.Chat.Id, START(), keyboardMainMenuButtons);
                    break;
                case "/СПИСОК ФАЙЛОВ":
                    SendMessageInlineKeyboard(message.Chat.Id, "Выберете тип файлов для просмотра списка или загрузки эскизов!", fileChooseButtons);
                    break;
                case "/ПОГОДА":
                    SendMessageInlineKeyboard(message.Chat.Id, Pogoda.GetPogoda(UserManager.GetPogodaCity(message),message), keyboardPogodaMenuButtons);
                    break;
                case "/ГОРОД":
                    UserManager.SetMenuPosition(message, UserManager.EMenuPosition.PogodaMenu);
                    SendMessage(message.Chat.Id, "Введите название города, для поиска:");
                    break;
                case "/HELP":
                    UserManager.SetMenuPosition(message, UserManager.EMenuPosition.MainMenu);
                    SendMessage(message.Chat.Id, HELP());
                    break;
                case "/НОМЕР РЕГИОН":
                    UserManager.SetMenuPosition(message, UserManager.EMenuPosition.RegionMenu);
                    SendMessage(message.Chat.Id, "Введите номер региона, для поиска:");
                    break;
                case "/РАЗВЛЕЧЕНИЯ":
                    Log($"РАЗВЛЕЧЕНИЯ");
                    SendMessage(message.Chat.Id, "Развлеки себя сам. А данный раздел WIP (в разработке!)");
                    break;
                case "/КАРТИНКИСПИСОК":
                    s = FileList(fileResivedPatch, 0,RequestFromInlineBtn.EtypeOfFileFilter.JPG);
                    SendMessageInlineKeyboard(message.Chat.Id, $"Список картинок:\n{s}\nВыберете картинку!", fileListButtons);
                    break;
                case "/КАРТИНКИ":
                    SendPhotoPreviews(message.Chat.Id, 0,RequestFromInlineBtn.EtypeOfFileFilter.JPG);
                    SendMessageInlineKeyboard(message.Chat.Id, $"Выберете картинку (цифру)!", fileListButtons);
                    break;
                case "/ГОЛОСОВЫЕ":
                    s = FileList(fileResivedPatch, 0, RequestFromInlineBtn.EtypeOfFileFilter.OGG);
                    SendMessageInlineKeyboard(message.Chat.Id, $"Список файлов:\n{s}\nВыберете голосовое сообщение!", fileListButtons);
                    break;
                case "/ВСЕ":
                    s = FileList(fileResivedPatch, 0, RequestFromInlineBtn.EtypeOfFileFilter.ALL);
                    SendMessageInlineKeyboard(message.Chat.Id, $"Список файлов:\n{s}\nВыберете файл, файл будет отправлен как документ, в независимости от типа!", fileListButtons);
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
                Thread.Sleep(1000);
                InlineKeyboardMarkup inlineKeyboard = new InlineKeyboardMarkup(inlineKeyboards);
                Log($"==> SendMessageInlineKeyboard: {msg}\n \t for chatID: {chatId}");
                await botClient.SendTextMessageAsync(chatId, msg, replyMarkup: inlineKeyboard);
        }

        /// <summary>
        /// Отправка файла с коротким именем в подписи
        /// </summary>
        /// <param name="chatId">берётся например из полученного ранее update.Message.Chat.Id</param>
        /// <param name="fileName">полное имя файла от каталога запуска в данном случае</param>
        private async void SendFileToUser(long chatId, string fileName, RequestFromInlineBtn.EtypeOfFileFilter typeOfFileFilter)
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
            inputOnlineFile.FileName = shortFileName;

            switch (typeOfFileFilter)
            {
                case RequestFromInlineBtn.EtypeOfFileFilter.JPG:
                    await botClient.SendPhotoAsync(chatId, inputOnlineFile, caption: shortFileName);
                    break;
                case RequestFromInlineBtn.EtypeOfFileFilter.OGG:
                    await botClient.SendVoiceAsync(chatId, inputOnlineFile, caption: shortFileName);
                    break;
                case RequestFromInlineBtn.EtypeOfFileFilter.ALL:
                    await botClient.SendDocumentAsync(chatId, inputOnlineFile, caption:shortFileName);
                    break;
            }
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

        private async void SendPhotoPreviews(long chatId, int stratOffset, RequestFromInlineBtn.EtypeOfFileFilter typeOfFileFilter)
        {
            
            string filesPath = $@"{fileResivedPatch}\preview\";
            string[] filesList;
            filesList = FileListArray(filesPath, stratOffset, typeOfFileFilter);
            if (filesList == null) return;
            System.IO.FileInfo fileInfo;
            string shortFileName;
            System.IO.FileStream fileStream;
            //InputMedia[] media = new InputMedia[filesList.Length];
            InputMedia media;
            InputMediaPhoto[] inputMediaPhotos = new InputMediaPhoto[filesList.Length];
            IAlbumInputMedia[] albumInputMedias = new IAlbumInputMedia[filesList.Length];
            int count = 0;

            if (filesList.Length == 0) return;
            foreach (string fullFileName in filesList)
            {
                Log("Имена файлов - миниатюр подготовленные на отправку:\n" + fullFileName);
                fileInfo = new System.IO.FileInfo(fullFileName); //получаем инфо о файле
                shortFileName = System.IO.Path.GetFileName(fullFileName); //получаем короткое имя файла (без пути)
                fileStream = new System.IO.FileStream(fullFileName, System.IO.FileMode.Open);
                media = new InputMedia(fileStream, shortFileName);
                inputMediaPhotos[count] = new InputMediaPhoto(media);
                inputMediaPhotos[count].Caption = $"📒 {stratOffset+1+ count} 📒\n📝{shortFileName}";
                albumInputMedias[count] = inputMediaPhotos[count];
                count++;
                //fileStream.Close();
            }

            Log($"==> Send Photo Previews from: {filesPath}\n \t for chatID: {chatId}");
            await botClient.SendMediaGroupAsync(chatId, albumInputMedias);
        }
    }
}
