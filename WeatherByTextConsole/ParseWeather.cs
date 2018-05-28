using HtmlAgilityPack;
using OpenJobScheduler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherByTextConsole
{
    class ParseWeather
    {
        public static void RunParser(bool strtOrstp)
        {           
            JobScheduler jobScheduler = new JobScheduler(TimeSpan.FromMinutes(20), new Action(() =>
            {                               
                ParseAndSave();               
            }));

            if (strtOrstp == true)
            {
                jobScheduler.Start();
            }
            else if (strtOrstp == false)
            {
                jobScheduler.Stop();
            }
        }

        public static void ParseAndSave()
        {           
            try
            {
                string url = "https://weather.com/weather/5day/l/11219:4:US";
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);

                Dictionary<string, string>[] day = new Dictionary<string, string>[6];
                StringBuilder[] weatherOfDay = new StringBuilder[6];

                var weatherByWeek = doc.DocumentNode.Descendants("tr").Where(node => node.GetAttributeValue("class", "").Equals("clickable closed")).ToList();

                for (int i = 0; i < weatherByWeek.Count; i++)
                {
                    StreamWriter saveWeatherInFile = new StreamWriter("C:/Users/UserName/source/repos/WeatherByTextConsole/WeatherByTextConsole/Resurces/Day" + i + ".txt");
                    day[i] = new Dictionary<string, string>();
                    weatherOfDay[i] = new StringBuilder();

                    var discription = weatherByWeek[0].SelectNodes("td").Last().Attributes["title"].Value; // Not using it to long for a text message
                    var dayOfWeek = weatherByWeek[i].Descendants("span").Where(n => n.GetAttributeValue("class", "").Equals("date-time")).FirstOrDefault().InnerText;
                    var dayOfMonth = weatherByWeek[i].Descendants("span").Where(n => n.GetAttributeValue("class", "").Equals("day-detail clearfix")).FirstOrDefault().InnerText;
                    var sunOrClouds = weatherByWeek[i].Descendants("td").Where(n => n.GetAttributeValue("class", "").Equals("description")).FirstOrDefault().InnerText;
                    var temp = weatherByWeek[i].Descendants("td").Where(n => n.GetAttributeValue("class", "").Equals("temp")).FirstOrDefault().InnerText;
                    var wind = weatherByWeek[i].Descendants("td").Where(n => n.GetAttributeValue("class", "").Equals("wind")).FirstOrDefault().InnerText;
                    var chanceOfRain = weatherByWeek[i].Descendants("td").Where(n => n.GetAttributeValue("class", "").Equals("precip")).FirstOrDefault().InnerText;
                    var humidity = weatherByWeek[i].Descendants("td").Where(n => n.GetAttributeValue("class", "").Equals("humidity")).FirstOrDefault().InnerText;

                    temp = temp.Replace("\u00B0", " ");  // this is to replace the ° symbol since basic phones replace it with a ?

                    day[i].Add("dayOfWeek", dayOfWeek);
                    day[i].Add("dayOfMonth", dayOfMonth);
                    day[i].Add("sunOrClouds", sunOrClouds);
                    day[i].Add("temp", temp);
                    day[i].Add("wind", wind);
                    day[i].Add("chanceOfRain", chanceOfRain);
                    day[i].Add("humidity", humidity);

                    foreach (KeyValuePair<string, string> kvp in day[i])
                    {
                        if (kvp.Key == "temp")
                        {
                            weatherOfDay[i].AppendLine("Temp: " + kvp.Value);
                        }
                        else if (kvp.Key == "wind")
                        {
                            weatherOfDay[i].AppendLine("Wind: " + kvp.Value);
                        }
                        else if (kvp.Key == "chanceOfRain")
                        {
                            weatherOfDay[i].AppendLine("RainPossibility: " + kvp.Value);
                        }
                        else if (kvp.Key == "humidity")
                        {
                            weatherOfDay[i].AppendLine("Humidity: " + kvp.Value);
                        }
                        else
                        {
                            weatherOfDay[i].AppendLine(kvp.Value);
                        }
                    }
                    saveWeatherInFile.WriteLine(weatherOfDay[i]);
                    saveWeatherInFile.Close();
                }

                Program.WriteLog(" Now parsed the weather... ");

                url = null;
                web = null;
                doc = null;
                day = null;
                weatherOfDay = null;
                weatherByWeek = null;
            }
            catch (Exception e)
            {
                Program.WriteLog("Couldn't parse the weather. error: " + e.Message);
                try
                {
                    Program.GetGmailInbox();
                }
                catch (Exception)
                {
                    RunParser(false);
                    Program.OnFailure();
                    throw;
                }
                throw;               
            }
        }
    }
}
