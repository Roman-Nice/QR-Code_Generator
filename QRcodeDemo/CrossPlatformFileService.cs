using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QRcodeDemo
{
    public class CrossPlatformFileService
    {
        public string Url { get; set; }
        public string IpAdress { get; set; }
        private Process Server { get; set; }

        public CrossPlatformFileService()
        {
            IpAdress = GetIp();
            Url = StartServer();
        }

        private string StartServer()
        {
            string strCmdText;
            strCmdText = "/c npx http-server";
            Server = Process.Start("CMD.exe", strCmdText);

            return $"http://{IpAdress}:8080/";
        }

        private string GetIp()
        {
            var ip = Dns.GetHostAddresses(Dns.GetHostName());
            return ip[ip.Length -1].ToString();
        }

        private string HostedFileName { get; set; }
        public string LocalFileName { get; set; }

        Random rnd = new Random();
        public async Task<string> HostFile(string originalUri)
        {
            LocalFileName = $"Server/_temp/_{rnd.Next(10000, 99999)}.{GetFileExtension(originalUri)}";
            File.Copy(originalUri, LocalFileName);

            int c = LocalFileName.Split("/").Length;

            HostedFileName = Url + "Server/_temp/" + LocalFileName.Split("/")[c - 1];
            return HostedFileName;
        }

        public void DeleteHostedFile()
        {
            if (LocalFileName == null)
                return;

            File.Delete(LocalFileName);
        }

        public void Close()
        {
            if(LocalFileName != null)
                File.Delete(LocalFileName);
            Server.CloseMainWindow();
        }

        private string GetFileExtension(string uri)
        {
            var ar = uri.Split(".");
            return ar[ar.Length - 1];
        }
    }
}
