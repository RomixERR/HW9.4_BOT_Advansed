using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Telegram.Bot.Types.ReplyMarkups;

namespace HW9._4_BOT_Advansed
{
    internal class Loger
    {
        public static InlineKeyboardButton[][] fileChooseButtons; //Кнопки которые появляются в прямо в тексте, выбор какой именно список показать пользователю
        public static InlineKeyboardButton[][] fileListButtons; //Кнопки которые появляются в прямо в тексте, выбор какой именно файл показать пользователю
        public static KeyboardButton[][] keyboardMainMenuButtons; //Кнопки главного меню снизу
        private static string filePatch;
        public Loger(string filePatch_)
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
            File.AppendAllText(filePatch, msg + "\n");
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
                    InlineKeyboardButton.WithCallbackData(text: "ДОКУМЕНТЫ", callbackData: "/ДОКУМЕНТЫ"),
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



        public static string FileListAll(string fileResivedPatch, int startOffset, string searchPattern)
        {
            string S="";
            int endItem;
            int maxCount=10;

            if (!Directory.Exists(fileResivedPatch))
            {
                Log($"Директория {fileResivedPatch} не обнаружена");
                return S;
            }

            DirectoryInfo directoryInfo = new DirectoryInfo(fileResivedPatch);
            IEnumerable<FileInfo> fileInfo = directoryInfo.EnumerateFiles(searchPattern);

            if (fileInfo.Count() == 0)
            {
                Log($"Директория {fileResivedPatch} пустая");
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
                    S += $"#{i+1}: {fi[i]} \n";
                }
            CreateInlineButtonsForFileList(endItem- startOffset+1, startOffset, maxCount);
            return S;

        }

        private static void CreateInlineButtonsForFileList(int amount,int offset, int maxCount)
        {
            int Ywhole = amount / 6; //Сколько целых строк
            int Xpart = amount % 6;  //Сколько остаётся на дополнительную не целую строку
            int Yrows;
            int count=1;
            InlineKeyboardButton[] tempX;
            tempX = new InlineKeyboardButton[6];
            if (Xpart == 0) { Yrows = Ywhole; } else { Yrows = Ywhole + 1; } //Всего строк
            fileListButtons = new InlineKeyboardButton[Yrows+1][]; //новый массив массивов (строки целые и не полные) +1 - для доп. кнопки
            for (int i = 0; i < Ywhole; i++) //проходим целые строки если есть
            {
                tempX = new InlineKeyboardButton[6];
                for (int j = 0; j < 6; j++) //заполняем строку по X
                {
                    tempX[j] = InlineKeyboardButton.WithCallbackData(text: $"{count+ offset}", callbackData: $"/NF:{count + offset}");
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
                    tempX[j] = InlineKeyboardButton.WithCallbackData(text: $"{count + offset}", callbackData: $"/NF:{count + offset}");
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
            tempX = new InlineKeyboardButton[2];
            tempX[0] = InlineKeyboardButton.WithCallbackData(text: $"<< ПРЕДЫДУЩИЕ", callbackData: $"/OFFSET:{pr}");
            tempX[1] = InlineKeyboardButton.WithCallbackData(text: $"СЛЕДУЮЩИЕ >>", callbackData: $"/OFFSET:{nx}");
            fileListButtons[Yrows] = tempX;
        }

    }

    public class User
    {
        public enum EFileType
        {
            jpg,
            ogg,
            all
        }
        public long UserId;
        public string FirstName;
        public int OffsetFileList;
        public EFileType FileType = EFileType.all;
        public string GetFilePattern()
        {
            string s;
            switch (FileType)
            {
                case EFileType.jpg:
                    s = "*.jpg";
                    break;
                case EFileType.ogg:
                    s = "*.ogg";
                    break;
                case EFileType.all:
                    s = "*.*";
                    break;
                default:
                    s = "*.*";
                    break;
            }
            return s;
        }
    }
}
