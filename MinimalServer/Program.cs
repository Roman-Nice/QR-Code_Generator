using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using PeanutButter.SimpleHTTPServer;
using PeanutButter.SimpleTcpServer;
using System.Net.Sockets;
using System.Threading;

namespace MinimalServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            HttpServer server = new HttpServer(true, (s) => Console.WriteLine(s));

            server.AddDocumentHandler(HandleFile);

            


            while (true) ;
        }

        private static string HandleFile(HttpProcessor p, Stream str)
        {
            //var stream = File.Open(@"C:\Users\RomanNice\source\repos\QRcodeDemo\MinimalServer\bin\Debug\netcoreapp3.1/index.html", FileMode.Open);
            //byte[] b = new byte[stream.Length];
            //stream.Read(b, 0, int.Parse(stream.Length.ToString()));

            string[] fileContent = File.ReadAllLines(@"C:\Users\RomanNice\source\repos\QRcodeDemo\MinimalServer\bin\Debug\netcoreapp3.1/shabbes.jpg");
            //stream.Close();

            string s = "";
            foreach (var line in fileContent)
            {
                s += line;
            }

            return s;
        }
    }
}

