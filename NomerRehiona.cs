using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using Telegram.Bot.Types;

namespace HW9._4_BOT_Advansed
{
    internal class NomerRehiona
    {
        private static Dictionary<int, string> nomerRegiona = new Dictionary<int, string>();

        public NomerRehiona()
        {
            string s = Properties.Resources.NumRegions;
            nomerRegiona = GenerateRegionNumCollection(s);
            //foreach (var item in nomerRegiona)
            //{
            //    Console.WriteLine($"NUM: {item.Key}, STRING: {item.Value}");
            //}
        }

        private Dictionary<int, string> GenerateRegionNumCollection(string resourseNumRegionsFile)
        {
            Dictionary<int, string> nomerRegiona = new Dictionary<int, string>();
            string[] ssep = new string[1] { Environment.NewLine };
            string[] ss = resourseNumRegionsFile.Split(ssep, StringSplitOptions.RemoveEmptyEntries);
            string[] pairs, numbers;

            foreach (var line in ss)
            {
                //Console.WriteLine($"++ {line}");
                pairs = line.Split(';');
                //Console.WriteLine($" ===  {pairs[0]}  {pairs[1]}");
                pairs[0].Replace(" ","");
                numbers = pairs[0].Split(',');
                foreach (var num in numbers)
                {
                    nomerRegiona.Add(int.Parse(num), pairs[1]);
                }
            }
            return nomerRegiona;
        }

        public static string GetRegionNumber(Message message)
        {
            string region = message.Text;
            string s;
            if (string.IsNullOrEmpty(region)) return "Ошибка. Вы ничего не ввели";
            try
            {
                
                if (nomerRegiona.TryGetValue(int.Parse(region), out s)) //Ищем название региона
                {
                    UserManager.SetMenuPosition(message, UserManager.EMenuPosition.MainMenu);
                }
                else
                {
                    return "Регион с данным номером не найден!";
                }
                
            }
            catch(Exception e)
            {
                AuxiliaryClass.Log($"ОШИБКА GetRegionNumber {e.Message}");
                return "Необходимо ввести номер!";
            }
            return s;
        }


    }
}
