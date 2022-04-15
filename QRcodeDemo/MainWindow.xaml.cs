using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessagingToolkit.QRCode.Codec;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Interop;
using System.IO;
using System.Globalization;
using Microsoft.Win32;
using System.Diagnostics;
using static QRcodeDemo.LoggingExtension;

namespace QRcodeDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FTHTTPService FileService { get; set; }

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            FileService = new FTHTTPService();
        }

        private void DataWindow_Closing(object sender, CancelEventArgs e)
        {
            FileService.Close();
        }

        private async void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];

                bool pathIsDirectory = Directory.Exists(files[0]);
                if (files.Length == 1 && !pathIsDirectory)
                {
                    HandlePathInputed(files[0]);
                    return;
                }

                if (pathIsDirectory)
                    files = Directory.GetFiles(files[0], "*");
                HandlePathsInputed(files);
            }
        }

        public Bitmap RawBitmap { get; set; }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string inputUrl = fe_urlTB.Text;

            HandlePathInputed(inputUrl);


            fe_save.Visibility = Visibility.Visible;
        }

        private async Task HandlePathInputed(string path)
        {
            FileService.DeleteHostedFile();
            try
            {
                if (!path.Contains("https://") || !path.Contains("http://"))
                {
                    path = await FileService.HostFile(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Generate(path);
            DisplayBitmap();

            fe_urlTB.Text = path;
        }

        private async Task HandlePathsInputed(string[] files)
        {
            string path = "";
            FileService.DeleteHostedFile();
            try
            {
                path = await FileService.HostFolder(files);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Generate(path);
            DisplayBitmap();

            fe_urlTB.Text = path;
        }

        private void Generate(string url)
        {
            QRCodeEncoder encoder = new QRCodeEncoder();

            Bitmap btmp = null;
            try
            {
                btmp = encoder.Encode(url, Encoding.UTF8);
            }
            catch (IndexOutOfRangeException ex)
            {
                Task.Run(delegate { MessageBox.Show("Link was too long"); });
                fe_urlTB.Text = "👀";
            }
            RawBitmap = btmp;

            fe_Image.Source = (ImageSource) ConvertToImageSource(btmp);
        }

        public object ConvertToImageSource(object value)
        {
            Bitmap bmp = value as Bitmap;
            if (bmp == null)
                return null;

            using(Stream str = new MemoryStream())
            {
                bmp.Save(str, System.Drawing.Imaging.ImageFormat.Bmp);
                str.Seek(0, SeekOrigin.Begin);
                BitmapDecoder bdc = new BmpBitmapDecoder(str, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad);
                return bdc.Frames[0];
            }
        }

        public void DisplayBitmap()
        {
            fe_display1.Text = ConvertToBase64(RawBitmap);
        }

        private string ConvertToBase64(Bitmap bitmap)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
            byte[] imageBytes = stream.ToArray();

            return Convert.ToBase64String(imageBytes);
        }

        private void fe_save_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(fe_urlTB.Text);
            WriteLine("Copied to clipboard!");
        }

        private void fe_urlTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string inputUrl = fe_urlTB.Text;
            Generate(inputUrl);
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            Generate("https://github.com/Roman-Nice");
        }

        private void fe_copy_Click(object sender, RoutedEventArgs e)
        {
            (string, string) crashInfo = ("undefined", "undefined");
            try
            {
                string displayContent = fe_display1.Text;
                string htmlTag = $"<img src=\"data: image / png; base64,{displayContent}\"/>";
                crashInfo = (displayContent, htmlTag);
                Clipboard.SetText(htmlTag);
            }
            catch( Exception ex) 
            {
                WriteLine("failed: " + ex.Message);
                WriteLine("Target: ");

                if(crashInfo.Item2 == "undefined")
                    WriteLine(crashInfo.Item1);
                else
                    WriteLine(crashInfo.Item2);
            }

            WriteLine("Copied!");
        }

        private void fe_open_Click(object sender, RoutedEventArgs e)
        {
            LoggingExtension.WriteLine("opening: "+ fe_urlTB.Text);
            try
            {
                var psi = new ProcessStartInfo()
                {
                    UseShellExecute = true,
                    FileName = fe_urlTB.Text
                };
                Process.Start(psi);
            }
            catch(Exception ex)
            {
                LoggingExtension.WriteLine("failed: "+ex.Message);
            }
        }
    }
}
