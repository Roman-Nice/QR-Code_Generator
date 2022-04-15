using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace QRcodeDemo
{
    public class FTHTTPService
    {
        public string Url { get; set; }
        public string IpAdress { get; set; }
        public string Port { get; set; }
        private Process Server { get; set; }

        public FTHTTPService()
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

            Server.Start();

            while (!Server.StandardOutput.EndOfStream)
            {
                string line = Server.StandardOutput.ReadLine();
                if (line == "Hit CTRL-C to stop the server")
                    break;
                Output.Append(line);
                
            }

            string output = Output.ToString();
            LoggingExtension.WriteLine(output);
            Port = GetPortNumber(output);
            Url = $"http://{IpAdress}:{Port}/";
        }

        internal async Task<string> HostFolder(string[] files)
        {
            string tmpDir = $"Server/_temp/{DateTime.Now.ToString("dd-hh-mm-ss-FFF")}";
            tmpDir = Directory.CreateDirectory(tmpDir).FullName;

            foreach (var file in files)
            {
                string tmpFile = Path.Combine(tmpDir)+ "\\" + Path.GetFileName(file);
                File.Copy(file, tmpFile);

            }

            string zipPath = Path.Combine(Directory.GetParent(tmpDir).FullName)+ "\\" + DateTime.Now.ToString("dd-hh-mm-ss-FFF") + ".zip";
            ZipFile.CreateFromDirectory(tmpDir, zipPath);
            Task.Run(()=> Directory.Delete(tmpDir, true));
            LocalFileName = zipPath;
            HostedFileName = Url + "Server/_temp/" + Path.GetFileName(zipPath);
            return HostedFileName;
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

        public async Task<string> HostFile(string originalUri)
        {
            LocalFileName = $"Server/_temp/_{DateTime.Now.ToString("dd-hh-mm-ss-FFF")}.{GetFileExtension(originalUri)}";
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
            LoggingExtension.WriteLine(Server.CloseMainWindow()); //efasefaefae
        }

        private string GetFileExtension(string uri)
        {
            var ar = uri.Split(".");
            return ar[ar.Length - 1];
        }
    }
}
