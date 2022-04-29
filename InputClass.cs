using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HW9._4_BOT_Advansed
{
    internal class InputClass
    {
        /// <summary>
        /// Мой личный улучшенный метод пользовательского ввода
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static T Input<T>(string msg = "")
        {
            T res = default(T);
            bool flag = true;
            do
            {
                try
                {
                    Console.WriteLine(msg);
                    //Object.ReferenceEquals(typeof(T), typeof(int)))
                    res = (T)Convert.ChangeType(Console.ReadLine(), typeof(T));
                    flag = false;
                }
                catch
                {
                    Console.WriteLine("НЕ КОРРЕКТНЫЙ ВВОД!!!");
                }

            } while (flag);
            return res;
        }
        /// <summary>
        /// Мой личный улучшенный метод пользовательского ввода со значением по умолчанию
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="msg"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T Input<T>(string msg, T defaultValue)
        {
            T res = default(T);
            bool flag = true;
            string rl;
            do
            {
                try
                {
                    Console.WriteLine(msg);
                    Console.Write("Пустое поле заменится на -> ");
                    Console.WriteLine(defaultValue);
                    rl = Console.ReadLine();
                    if (rl == "")
                    {
                        res = defaultValue;
                    }
                    else
                    {
                        res = (T)Convert.ChangeType(rl, typeof(T));
                    }
                    flag = false;
                }
                catch
                {
                    Console.WriteLine("НЕ КОРРЕКТНЫЙ ВВОД!!!");
                }

            } while (flag);
            return res;
        }
        /// <summary>
        /// Мой личный улучшенный метод пользовательского ввода с проверкой на пустой ввод
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isAnNotEmptyLine"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static T Input<T>(out bool isAnEmptyLine, string msg = "")
        {
            T res = default(T);
            bool flag = true;
            isAnEmptyLine = true;
            string s;
            do
            {
                try
                {
                    Console.WriteLine(msg);
                    //Object.ReferenceEquals(typeof(T), typeof(int)))
                    s = Console.ReadLine();
                    if (s.Equals(""))
                    {
                        flag = false;
                    }
                    else
                    {
                        isAnEmptyLine = false;
                        res = (T)Convert.ChangeType(s, typeof(T));
                        flag = false;
                    }
                }
                catch
                {
                    Console.WriteLine("НЕ КОРРЕКТНЫЙ ВВОД!!!");
                }

            } while (flag);
            return res;
        }

    }
}
