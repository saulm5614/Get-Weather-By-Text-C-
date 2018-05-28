using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StartProcessOnFailier
{
    class Program
    {
        static void Main(string[] args)
        {
            int milliseconds = 5000;
            System.Threading.Thread.Sleep(milliseconds);

            var weatherByText = Process.GetProcessesByName("WeatherByTextConsole");

            if (weatherByText.Length == 0)
            {
                StreamWriter writeLog = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt"), true);
                writeLog.WriteLine(DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " progra was started by StartProcessOnFailier...");
                writeLog.Close();

                Process process = Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WeatherByTextConsole.exe"));
            }
        }
    }
}
