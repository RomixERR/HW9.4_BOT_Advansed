using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
//using System.Text.Json;

namespace HW9._4_BOT_Advansed
{
    internal class Pogoda
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }

        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }

        public class Main
        {
            public double temp { get; set; }
            public double feels_like { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
        }

        public class Wind
        {
            public double speed { get; set; }
            public int deg { get; set; }
        }

        public class Clouds
        {
            public int all { get; set; }
        }

        public class Sys
        {
            public int type { get; set; }
            public int id { get; set; }
            public double message { get; set; }
            public string country { get; set; }
            public int sunrise { get; set; }
            public int sunset { get; set; }
        }

        public class Root
        {
            public Coord coord { get; set; }
            public List<Weather> weather { get; set; }
            public string @base { get; set; }
            public Main main { get; set; }
            public int visibility { get; set; }
            public Wind wind { get; set; }
            public Clouds clouds { get; set; }
            public int dt { get; set; }
            public Sys sys { get; set; }
            public int timezone { get; set; }
            public int id { get; set; }
            public string name { get; set; }
            public int cod { get; set; }
        }

        
            static string sURL;
            static string RomixERRAPIKey;
            static WebRequest wrGETURL;

        public Pogoda(string APIKeypath)
        {
            RomixERRAPIKey = System.IO.File.ReadAllText(APIKeypath);
        }


        


        public static string GetPogoda(string NameOfCity, Message message)
        {
            //sURL = "http://api.openweathermap.org/data/2.5/weather?id=472757&appid=" + RomixERRAPIKey + "&lang=ru&units=metric";
            sURL = $"http://api.openweathermap.org/data/2.5/weather?q={NameOfCity}&appid={RomixERRAPIKey}&lang=ru&units=metric";
            //Console.WriteLine(sURL);
            //Console.ReadKey();
            Stream stream;
            wrGETURL = WebRequest.Create(sURL);
            string s;
            try
            {
                stream = wrGETURL.GetResponse().GetResponseStream();
                StreamReader streamReader = new StreamReader(stream);
                JsonSerializer jsonSerializer= JsonSerializer.Create();
                Root myDeserializedClass = (Root)jsonSerializer.Deserialize(streamReader, typeof(Root));
                if (myDeserializedClass == null) {
                    AuxiliaryClass.Log($"Не удалось десериализовать данные о погоде. myDeserializedClass == null");
                    return "Не удалось десериализовать данные о погоде."; 
                }
                s = $"🌆 ВЫБРАН ГОРОД: {myDeserializedClass.name}\n" +
                    $"🌅 ТЕМПЕРАТУРА: {myDeserializedClass.main.temp.ToString()}C\n" +
                    $"💧 ВЛАЖНОСТЬ: {myDeserializedClass.main.humidity.ToString()}%\n" +
                    $"⚡️ ДАВЛЕНИЕ: {(myDeserializedClass.main.pressure * 100 / 133).ToString()} мм. рт. ст.\n" +
                    $"⭐️ ОПИСАНИЕ: {myDeserializedClass.weather[0].description}\n\n" +
                    $"Данные с сервиса openweathermap.org";
            }
            catch (System.Net.WebException e)
            {
                AuxiliaryClass.Log($"Не удалось десериализовать данные о погоде. Ошибка: {e.Message}");
                return $"Не удалось десериализовать данные о погоде.\n🦨{e.Message}";
            }
            UserManager.SetMenuPosition(message, UserManager.EMenuPosition.MainMenu);
            return s;
        }






    }
}
