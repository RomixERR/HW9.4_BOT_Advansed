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
        private static string filePatch;
        public Loger(string filePatch_)
        {
            filePatch = filePatch_;
        }
        public static void Log(string msg)
        {
            Console.WriteLine(msg);
            CreateSupportingDirectory(filePatch);
            File.AppendAllText(filePatch, msg+"\n");
        }

        public static void CreateSupportingDirectory(string fileName)
        {
            string dir = System.IO.Path.GetDirectoryName(fileName);
            System.IO.Directory.CreateDirectory(dir);
        }



    }
}
