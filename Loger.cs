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
        public static InlineKeyboardButton[][] fileListButtons; //Кнопки которые появляются в прямо в тексте, выбор какой именно список показать пользователю
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

            fileListButtons = new InlineKeyboardButton[][]
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
    }
}
