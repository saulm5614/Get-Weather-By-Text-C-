using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace WeatherByTextConsole
{
    public class GetWeather
    {
        public static void Parse(int days, string toAddress, string emailId)
        {
            var send = "";
            for (int i = 0; i < days; i++)
            {
                StreamReader sr = new StreamReader("C:/Users/UserName/source/repos/WeatherByTextConsole/WeatherByTextConsole/Resurces/Day" + i + ".txt");
                StringBuilder sb = new StringBuilder();
                var wod = sr.ReadLine();
                while (wod != null)
                {
                    sb.AppendLine(wod);
                    wod = sr.ReadLine();
                }
                sr.Close();
                send = SendWeather(toAddress, sb.ToString());
            }

            if (send == "Success")
            {
                var success = Program.MarkAsRead(emailId);
                if (success == true)
                {
                    Program.WriteLog(" Send weather To: " + toAddress + " asked for " + days + " days ");
                    Program.GetGmailInbox();                  
                }
            }
        }

        public static string SendWeather(string toAddres, string weather)
        {
            try
            {
                var fromAddress = new MailAddress("YourGmailAddres", "Name");
                var toAddress = new MailAddress(toAddres, "Name");
                const string fromPassword = "YourPassword";
                const string subject = "";
                string body = weather;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }
                return "Success";
            }
            catch (Exception e)
            {
                Program.WriteLog("Can't send out text. error: " + e.Message);
                // just one more chance
                try
                {
                    Program.GetGmailInbox();
                }
                catch (Exception)
                {
                    ParseWeather.RunParser(false);
                    Program.OnFailure();
                    throw;
                }
                throw;
            }            
        }
    }
}
