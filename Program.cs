using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HW9._4_BOT_Advansed
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Loger loger = new Loger(@"LogFiles\Logs.txt");
            Telega telegram = new Telega(@"E:\C#\SKILLBOXC#\HW9\tokenTestBot.txt", @"ResivedFiles");

        }
    }
}
