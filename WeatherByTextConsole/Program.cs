using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherByTextConsole
{
    /*
     * To run the code without poping up the console window every time refare to this threat 
     * https://stackoverflow.com/a/2686476/9329277
     * 
     * Project > Properties > Application tab > change Output type to "Windows application".
     * 
     * this is what i did for this application
     */

    class Program
    {
        static string[] Scopes = { GmailService.Scope.GmailModify };
        static string ApplicationName = "Gmail API .NET Quickstart";

        static void Main(string[] args)
        {
            ParseWeather.ParseAndSave(); // just start the program with fresh results
            ParseWeather.RunParser(true); // then start parsing every 20 minuts
            GetGmailInbox();
        }

        public static void GetGmailInbox()
        {
            int milliseconds = 1000;
            System.Threading.Thread.Sleep(milliseconds);  // just put a second between runs.

            try
            {
                GmailService service = InitGmailConnection();

                var re = service.Users.Messages.List("me");
                re.LabelIds = "INBOX";
                re.Q = "is:unread"; //only get unread;

                var res = re.Execute();

                if (res != null && res.Messages != null)
                {
                    foreach (var email in res.Messages)
                    {
                        var emailInfoReq = service.Users.Messages.Get("me", email.Id);
                        var emailInfoResponse = emailInfoReq.Execute();

                        if (emailInfoResponse != null)
                        {
                            String from = "";
                            String date = "";
                            String subject = "";
                            String body = "";
                            //loop through the headers and get the fields we need...
                            foreach (var mParts in emailInfoResponse.Payload.Headers)
                            {
                                if (mParts.Name == "Date")
                                {
                                    date = mParts.Value;
                                }
                                else if (mParts.Name == "From")
                                {
                                    from = mParts.Value;
                                }
                                else if (mParts.Name == "Subject")
                                {
                                    subject = mParts.Value;
                                }

                                if (date != "" && from != "")
                                {
                                    if (emailInfoResponse.Payload.Parts == null && emailInfoResponse.Payload.Body != null)
                                        body = DecodeBase64String(emailInfoResponse.Payload.Body.Data);
                                    else
                                        body = GetNestedBodyParts(emailInfoResponse.Payload.Parts, "");
                                }

                            }
                            var days = 0;
                            if (body.Contains("1\r\n"))
                            {
                                days = 1;
                                GetWeather.Parse(days, from, email.Id);
                            }
                            else if (body.Contains("2\r\n"))
                            {
                                days = 2;
                                GetWeather.Parse(days, from, email.Id);
                            }
                            else if (body.Contains("3\r\n"))
                            {
                                days = 3;
                                GetWeather.Parse(days, from, email.Id);
                            }
                        }
                    }
                }
                service = null;
                re = null;
                res = null;
                GC.Collect();
                  
                GetGmailInbox();                
            }
            catch (Exception e)
            {
                WriteLog("Can't get Gmail Inbox. error: " + e.Message);

                try  // just one more chance
                {                   
                    GetGmailInbox();
                }
                catch (Exception)
                {
                    ParseWeather.RunParser(false);
                    OnFailure();
                    throw;
                }
                throw;
            }            
        }

        public static bool MarkAsRead(string emailId)
        {
            GmailService service = InitGmailConnection();
            var markAsRead = new ModifyMessageRequest { RemoveLabelIds = new[] { "UNREAD" } };
            service.Users.Messages.Modify(markAsRead, "me", emailId).Execute();
            return true;
        }

        static String DecodeBase64String(string s)
        {
            var ts = s.Replace("-", "+");
            ts = ts.Replace("_", "/");
            var bc = Convert.FromBase64String(ts);
            var tts = Encoding.UTF8.GetString(bc);

            return tts;
        }

        static String GetNestedBodyParts(IList<MessagePart> part, string curr)
        {
            string str = curr;
            if (part == null)
            {
                return str;
            }
            else
            {
                foreach (var parts in part)
                {
                    if (parts.Parts == null)
                    {
                        if (parts.Body != null && parts.Body.Data != null)
                        {
                            var ts = DecodeBase64String(parts.Body.Data);
                            str += ts;
                        }
                    }
                    else
                    {
                        return GetNestedBodyParts(parts.Parts, str);
                    }
                }

                return str;
            }
        }

        static GmailService InitGmailConnection()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal);
                credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                //Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Gmail API service.
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            return service;
        }

        public static void WriteLog(string message)
        {
            StreamWriter writeLog = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true);
            writeLog.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " " + message);
            writeLog.Close();
        }

        public static void OnFailure()
        {
            var failProcess = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StartProcessOnFailier.exe");
            Process.Start(failProcess);
            WriteLog("fail process was called...");           
        }
    }
}
