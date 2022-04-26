using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace HW9._4_BOT_Advansed
{
    internal class Loger
    {
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

        private static string filePatch;
        public Loger(string filePatch_)
        {
            filePatch = filePatch_;
            FillOptionsMainButton();
        }
        public static void Log(string msg)
        {
            Console.WriteLine(msg);
            CreateSupportingDirectory(filePatch);
            File.AppendAllText(filePatch, msg + "\n");
        }

        public static void CreateSupportingDirectory(string fileName)
        {
            string dir = System.IO.Path.GetDirectoryName(fileName);
            System.IO.Directory.CreateDirectory(dir);
        }

       
        
    }
}
