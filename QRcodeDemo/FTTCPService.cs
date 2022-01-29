using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace QRcodeDemo
{
    public class FTTCPService
    {
        public string Url { get; set; }
        public string IpAdress { get; set; }
        public string Port { get; set; }
        private Process Server { get; set; }

        public FTTCPService()
        {
            IpAdress = GetIp();
            StartServer();
        }

        private async Task StartServer()
        {
            string strCmdText;
            strCmdText = "cmd.exe";

            Server = new Process();
            Server.StartInfo.FileName = "./node_modules\\.bin\\http-server.cmd";
            //Server.StartInfo.Arguments = @"--headless";
            Server.StartInfo.CreateNoWindow = true;
            Server.StartInfo.UseShellExecute = false;
            Server.StartInfo.RedirectStandardOutput = true;

            //Server.OutputDataReceived += CaptureOutput;

            Server.Start();

            while (!Server.StandardOutput.EndOfStream)
            {
                string line = Server.StandardOutput.ReadLine();
                if (line == "Hit CTRL-C to stop the server")
                    break;
                Output.Append(line);
                
            }

            string output = Output.ToString();
            Port = GetPortNumber(output);
            Url = $"http://{IpAdress}:{Port}/";

            // ProcessOutputSynchronizationContext();

            //return $"http://{IpAdress}:{Port}/";
        }
        public StringBuilder Output { get; set; } = new StringBuilder();

        private string GetPortNumber(string processOutput)
        //(https?://.*):(\d*)\/?(.*)
        {
            var adressMatches = Regex.Match(processOutput, @"(https?://.*):(\d*)\/?(.*)");
            var adress = adressMatches.Captures[adressMatches.Captures.Count - 1];
            string[] splitAddress = adress.Value.Split(":");

            return splitAddress[splitAddress.Length - 1];
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
