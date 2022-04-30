using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HW9._4_BOT_Advansed.InputClass;


namespace HW9._4_BOT_Advansed
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string LogPath = @"LogFiles\Logs.txt";
            string TokenFilePath = @"E:\C#\SKILLBOXC#\HW9\tokenTestBot.txt";
            string ResivedFilesPath = @"ResivedFiles";

            if (args.Length == 3)
            {
                LogPath = args[0];
                TokenFilePath = args[1];
                ResivedFilesPath = args[2];
            }
            else
            {
                Console.WriteLine("HW9.4_BOT_Advansed.exe [LogPath] [TokenFilePath] [ResivedFilesPath]");
                Console.WriteLine(@"For exemple HW9.4_BOT_Advansed.exe LogFiles\Logs.txt E:\C#\SKILLBOXC#\HW9\tokenTestBot.txt ResivedFiles");
                LogPath = Input<string>($"Enter LogPath:", LogPath);
                TokenFilePath = Input<string>($"Enter TokenFilePath:", TokenFilePath);
                ResivedFilesPath = Input<string>($"Enter ResivedFilesPath:", ResivedFilesPath);
            }
            Console.WriteLine($"LogPath: {LogPath}\nTokenFilePath: {TokenFilePath}\nResivedFilesPath: {ResivedFilesPath}\n");
            Console.WriteLine("Press any key for START");
            Console.ReadKey();
            Console.Clear();
            NomerRehiona nomerRehiona = new NomerRehiona();
            AuxiliaryClass loger = new AuxiliaryClass(LogPath);
            Telega telegram = new Telega(TokenFilePath, ResivedFilesPath);
        }
    }
}
