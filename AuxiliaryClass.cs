using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;

namespace HW9._4_BOT_Advansed
{
    internal class AuxiliaryClass
    {
        public static InlineKeyboardButton[][] fileChooseButtons; //Кнопки которые появляются в прямо в тексте, выбор какой именно список показать пользователю
        public static InlineKeyboardButton[][] fileListButtons; //Кнопки которые появляются в прямо в тексте, выбор какой именно файл показать пользователю
        public static KeyboardButton[][] keyboardMainMenuButtons; //Кнопки главного меню снизу
        private static string filePatch;
        public AuxiliaryClass(string filePatch_)
        {
            filePatch = filePatch_;
            FillOptionsMainButton();
            CreateKeyboardButtons();
            CreateInlineButtons();
        }
        public static void Log(string msg)
        {
            Console.WriteLine(msg);
            CreateSupportingDirectory(filePatch);
            System.IO.File.AppendAllText(filePatch, msg + "\n");
        }
        public enum forOptionsButton
        {
            spisok,
            pogoda,
            help,
            region_nomer,
            razvlecheniya
        }
        public static Dictionary<forOptionsButton, string> OptionsMainButton = new Dictionary<forOptionsButton, string>();

        private void FillOptionsMainButton()
        {
            OptionsMainButton.Add(forOptionsButton.spisok, "/СПИСОК ФАЙЛОВ");
            OptionsMainButton.Add(forOptionsButton.pogoda, "/ПОГОДА");
            OptionsMainButton.Add(forOptionsButton.help, "/HELP");
            OptionsMainButton.Add(forOptionsButton.region_nomer, "/НОМЕР РЕГИОН");
            OptionsMainButton.Add(forOptionsButton.razvlecheniya, "/РАЗВЛЕЧЕНИЯ");
        }
        private void CreateInlineButtons()
        {

            fileChooseButtons = new InlineKeyboardButton[][]
            {
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "КАРТИНКИ СПИСОК", callbackData: "/КАРТИНКИСПИСОК"),
                    InlineKeyboardButton.WithCallbackData(text: "КАРТИНКИ МИНИАТЮРЫ", callbackData: "/КАРТИНКИ"),
                },
                new InlineKeyboardButton[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "ГОЛОСОВЫЕ", callbackData: "/ГОЛОСОВЫЕ"),
                    InlineKeyboardButton.WithCallbackData(text: "ВСЕ СПИСОК", callbackData: "/ВСЕ"),
                }
            };
        }
        private void CreateKeyboardButtons()
        {
            keyboardMainMenuButtons = new KeyboardButton[][]
           {
                        new KeyboardButton[] { "/СПИСОК ФАЙЛОВ", "/ПОГОДА" },
                        new KeyboardButton[] { "/HELP", "/НОМЕР РЕГИОН", "/РАЗВЛЕЧЕНИЯ" },
           };
        }
        public static void CreateSupportingDirectory(string fileName)
        {
            string dir = System.IO.Path.GetDirectoryName(fileName);
            System.IO.Directory.CreateDirectory(dir);
        }
        public static string START()
        {
            string s = "☺️ Привет! Это СУПЕР БОТ от RomixERR©️.\n" +
                       "В низу у вас есть клавиатура со списком команд. Она включается и выключается так-же как и простая клавиатура, " +
                       "просто нажмите на значёк справа от поля для ввода сообщения.\n" +
                       "Попробуйте для начала команду 🔸/HELP\n" +
                       "Эта команда ознакомит вас с возможностями бота.";
            return s;

        }

        public static string HELP()
        {
            string s =  "☺️ Меня зовут RomixERR©️ и я создатель этого Telegram бота! " +
                        "Давайте этот раздел будет чем-то типа о программе! 👍🏻\n\n" +
                        "🎱 Данный бот разрабатывается в рамках ДЗ от SkillBox: 9.4 Практическая работа. Научиться создавать чат-ботов на языке C#.\n" +
                        "♥️Что оценивается♥️:\n" +
                        "🔸 Бот принимает текстовые сообщения.\n" +
                        "🔸 Бот реагирует на команду / start.\n" +
                        "🔸 Бот позволяет сохранять на диск изображения, аудио - и другие файлы.\n" +
                        "С помощью произвольной команды можно просмотреть список сохранённых файлов и скачать любой из них.\n\n" +
                        "🀄️Функции бота🀄️:\n" +
                        "🔸/START - начало работы с ботом. Инициализируется клавиатура с основным меню. В любой не понятной ситуации пиши /START\n" +
                        "🔸/СПИСОК ФАЙЛОВ - Список файлов хранящихся в локальной директории бота. Вы можете отправлять боту различные вложения (по одному за раз!) любые документы, фотографии или голосовые. " +
                        "Они будут храниться на диске в локальной директории бота.\n" +
                        "🔸/КАРТИНКИ 🔸/ГОЛОСОВЫЕ 🔸/ВСЕ - выбор, какие файлы вы хотите скачать или посмотреть их список.\n" +
                        "\n" +
                        "🔹/ПОГОДА " +
                        "🔹/НОМЕР РЕГИОН " +
                        "🔹/РАЗВЛЕЧЕНИЯ " +
                        "в процессе разработки!";
            return s;
        }


        public static string FileList(string fileResivedPatch, int startOffset, RequestFromInlineBtn.EtypeOfFileFilter searchFileType)
        {
            string searchPat = RequestFromInlineBtn.GetFilePattern(searchFileType);
            string S="";
            int endItem;
            int maxCount=10;

            if (!Directory.Exists(fileResivedPatch))
            {
                Log($"ОШИБКА Директория {fileResivedPatch} не обнаружена");
                return S;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(fileResivedPatch);
            IEnumerable<FileInfo> fileInfo = directoryInfo.EnumerateFiles(searchPat);

            if (fileInfo.Count() == 0)
            {
                Log($"ОШИБКА Директория {fileResivedPatch} пустая");
                return S;
            }

            FileInfo[] fi = fileInfo.ToArray();

            endItem = fileInfo.Count();
            if (endItem > (maxCount+ startOffset-1))
            {
                endItem = (maxCount+ startOffset-1);
            }
            if (endItem >= fileInfo.Count()) { endItem = fileInfo.Count() - 1; };

                for (int i = startOffset; i <= endItem; i++)
                {
                    S += $"✅{i+1}: {fi[i]} \n";
                }
            CreateInlineButtonsForFileList(endItem- startOffset+1, startOffset, maxCount, searchFileType, RequestFromInlineBtn.EtypeOfReq.ShowListFiles);
            return S;
        }

        public static string[] FileListArray(string fileResivedPatch, int startOffset, RequestFromInlineBtn.EtypeOfFileFilter searchFileType)
        {
            string searchPat = RequestFromInlineBtn.GetFilePattern(searchFileType);
            string[] S;
            int endItem;
            int maxCount = 10;
            int count = 0;

            if (!Directory.Exists(fileResivedPatch))
            {
                Log($"ОШИБКА Директория {fileResivedPatch} не обнаружена");
                return null;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(fileResivedPatch);
            IEnumerable<FileInfo> fileInfo = directoryInfo.EnumerateFiles(searchPat);

            if (fileInfo.Count() == 0)
            {
                Log($"ОШИБКА Директория {fileResivedPatch} пустая");
                return null;
            }

            FileInfo[] fi = fileInfo.ToArray();

            endItem = fileInfo.Count();
            if (endItem > (maxCount + startOffset - 1))
            {
                endItem = (maxCount + startOffset - 1);
            }
            if (endItem >= fileInfo.Count()) { endItem = fileInfo.Count() - 1; };

            S = new string[endItem - startOffset+1];

            for (int i = startOffset; i <= endItem; i++)
            {
                S[count] = fi[i].FullName;
                count++;
            }
            CreateInlineButtonsForFileList(endItem - startOffset + 1, startOffset, maxCount, searchFileType, RequestFromInlineBtn.EtypeOfReq.ShowPreviewsPhotos);
            return S;
        }



        public static string GetFullFileName(string fileResivedPatch, int numOfFileInList, RequestFromInlineBtn.EtypeOfFileFilter searchFileType)
        {
            string searchPat = RequestFromInlineBtn.GetFilePattern(searchFileType);
            string S = "";

            if (!Directory.Exists(fileResivedPatch))
            {
                Log($"ОШИБКА GetFullFileName Директория {fileResivedPatch} не обнаружена");
                return S;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(fileResivedPatch);
            IEnumerable<FileInfo> fileInfo = directoryInfo.EnumerateFiles(searchPat);

            if (fileInfo.Count() == 0)
            {
                Log($"ОШИБКА GetFullFileName Директория {fileResivedPatch} пустая");
                return S;
            }

            FileInfo[] fi = fileInfo.ToArray();
            if (fi.Length <= numOfFileInList) {Log($"ОШИБКА GetFullFileName numOfFileInList слишком большое значение:{numOfFileInList}"); return S; };
            S = fi[numOfFileInList].FullName;
            return S;
        }


        private static void CreateInlineButtonsForFileList(int amount,int offset, int maxCount, RequestFromInlineBtn.EtypeOfFileFilter searchFileType, RequestFromInlineBtn.EtypeOfReq typeOfReq)
        {
            int Ywhole = amount / 6; //Сколько целых строк
            int Xpart = amount % 6;  //Сколько остаётся на дополнительную не целую строку
            int Yrows;
            int count=1;
            string PatternRequest = RequestFromInlineBtn.patternRequest;
            InlineKeyboardButton[] tempX;
            tempX = new InlineKeyboardButton[6];
            if (Xpart == 0) { Yrows = Ywhole; } else { Yrows = Ywhole + 1; } //Всего строк
            fileListButtons = new InlineKeyboardButton[Yrows+1][]; //новый массив массивов (строки целые и не полные) +1 - для доп. кнопки
            for (int i = 0; i < Ywhole; i++) //проходим целые строки если есть
            {
                tempX = new InlineKeyboardButton[6];
                for (int j = 0; j < 6; j++) //заполняем строку по X
                {
                    tempX[j] = InlineKeyboardButton.WithCallbackData(text: $"{count+ offset}", callbackData: $"{PatternRequest}:SF:{count + offset}:{searchFileType.ToString()}");
                    count++;
                }
                fileListButtons[i] = tempX;
                //Array.Copy(tempX, fileListButtons[i], tempX.Length);
            }
            if (Xpart > 0) //Если осталась не полная строка
            {
                tempX = new InlineKeyboardButton[Xpart];
                for (int j = 0; j < Xpart; j++) //заполняем строку по X
                {
                    tempX[j] = InlineKeyboardButton.WithCallbackData(text: $"{count + offset}", callbackData: $"{PatternRequest}:SF:{count + offset}:{searchFileType.ToString()}");
                    count++;
                }
                fileListButtons[Yrows-1] = tempX;
                //Array.Copy(tempX, fileListButtons[Yrows - 1], tempX.Length);
            }
            //доп кнопка
            //int pr = offset - amount;
            int pr = offset + amount- maxCount;
            int nx = count + offset - 1;
            if (pr < 0) pr = 0; //Первый лист
            if (Ywhole==0) //нет целых строк, нельзя показать следующиеХ
            {
                nx = 0;
            }
            string P;
            if (typeOfReq == RequestFromInlineBtn.EtypeOfReq.ShowListFiles)
            {
                P = "SL";
            }
            else
            {
                P = "SP";
            }
            tempX = new InlineKeyboardButton[2];
            tempX[0] = InlineKeyboardButton.WithCallbackData(text: $"<< ПРЕДЫДУЩИЕ", callbackData: $"{PatternRequest}:{P}:{pr}:{searchFileType.ToString()}");
            tempX[1] = InlineKeyboardButton.WithCallbackData(text: $"СЛЕДУЮЩИЕ >>", callbackData: $"{PatternRequest}:{P}:{nx}:{searchFileType.ToString()}");
            fileListButtons[Yrows] = tempX;
        }

        public static bool ExtractRequestFromInlineBtn(string text, out RequestFromInlineBtn req)
        {
            //    /REQB:SF:10:JPG
            req = new RequestFromInlineBtn();
            string[] s;
            if (!text.Contains(RequestFromInlineBtn.patternRequest)) return false; //Это не это
            try
            {
                s = text.Split(':');
                if (!s[0].Equals(RequestFromInlineBtn.patternRequest)) {Log($"ExtractRequestFromInlineBtn - s[0] паттерн не ликвидный: {s[0]}"); return false; }; //s[0] не ликвидный
                switch (s[1])
                {
                    case "SF": req.typeOfReq = RequestFromInlineBtn.EtypeOfReq.SendFile; break;
                    case "SL": req.typeOfReq = RequestFromInlineBtn.EtypeOfReq.ShowListFiles; break;
                    case "SP": req.typeOfReq = RequestFromInlineBtn.EtypeOfReq.ShowPreviewsPhotos; break;
                    default: { Log($"ExtractRequestFromInlineBtn - s[1] паттерн не ликвидный: {s[1]}"); return false; };
                }
                req.numberOfFile = int.Parse(s[2]);
                req.typeOfFileFilter = (RequestFromInlineBtn.EtypeOfFileFilter)Enum.Parse(typeof(RequestFromInlineBtn.EtypeOfFileFilter), s[3]);
            }
            catch (Exception e)
            {
                Log($"ExtractRequestFromInlineBtn - Exception.message: {e.Message}");
                return false;
            }
            return true;
        }

    }

    
     public struct RequestFromInlineBtn
    {
        public static string patternRequest = @"/REQB";
        public enum EtypeOfReq
        {
            SendFile,
            ShowListFiles,
            ShowPreviewsPhotos
        }
        public enum EtypeOfFileFilter
        {
            JPG,
            OGG,
            ALL
        }
        public EtypeOfReq typeOfReq;
        public int numberOfFile;
        public EtypeOfFileFilter typeOfFileFilter;
        public static string GetFilePattern(EtypeOfFileFilter typeOfFileFilter)
        {
            string s;
            switch (typeOfFileFilter)
            {
                case EtypeOfFileFilter.JPG:
                    s = "*.jpg";
                    break;
                case EtypeOfFileFilter.OGG:
                    s = "*.ogg";
                    break;
                case EtypeOfFileFilter.ALL:
                    s = "*.*";
                    break;
                default:
                    s = "*.*";
                    break;
            }
            return s;
        }
    }

    public class UserManager
    {
        public class User
        {
            public long UserId;
            public string FirstName;
            public string PogodaCity;
            public int NumOfRegion;
            public EMenuPosition MenuPosition;
        }
        public enum EMenuPosition
        {
            MainMenu,
            RegionMenu
        }


        private static Dictionary<long, User> MyUsers = new Dictionary<long, User>();    //<ChatID,User> Пользователи

        public static void RegisterUser(Message message)
        {
            if (!MyUsers.ContainsKey(message.Chat.Id)) //Если пользователя нет то регистрируем его
            {
                User user = new User() {
                    FirstName = message.From.FirstName,
                    UserId = message.From.Id,
                    MenuPosition = EMenuPosition.MainMenu
                    };
                MyUsers.Add(message.Chat.Id, user); //регистрируем в оперативку
                AuxiliaryClass.Log($"Пользователь добавлен FirstName:{message.From.FirstName} ChatID:{message.Chat.Id}");
            }
        }
        public static void UnRegisterUser(Message message)
        {
            if (!MyUsers.ContainsKey(message.Chat.Id)) return; //Если пользователя нет
            MyUsers.Remove(message.Chat.Id); //Удаляем из оперативки
        }
        public static string GetPogodaCity(Message message)
        {
            string s="";
            if(MyUsers.TryGetValue(message.Chat.Id,out User user))
            {
                s = user.PogodaCity;
            }
            return s;
        }
        public static int GetNumOfRegion(Message message) 
        {
            int n = -1;
            if (MyUsers.TryGetValue(message.Chat.Id, out User user))
            {
                n = user.NumOfRegion;
            }
            return n;
        }
        public static void SetNumOfRegion(Message message, string numOfRegion)
        {
            if (MyUsers.TryGetValue(message.Chat.Id, out User user))
            {
                try
                {
                    user.NumOfRegion = int.Parse(numOfRegion);
                }
                catch (Exception e)
                {
                    AuxiliaryClass.Log($"ОШИБКА SetNumOfRegion Message:{e}");
                }
            }
        }
        public static void SetMenuPosition(Message message, EMenuPosition menuPosition)
        {
            if (MyUsers.TryGetValue(message.Chat.Id, out User user))
            {
                user.MenuPosition = menuPosition;
            }
        }
        public static EMenuPosition GetMenuPosition(Message message)
        {
            EMenuPosition menuPosition = EMenuPosition.MainMenu;
            if (MyUsers.TryGetValue(message.Chat.Id, out User user))
            {
                menuPosition = user.MenuPosition;
            }
            return menuPosition;
        }


    }

}
