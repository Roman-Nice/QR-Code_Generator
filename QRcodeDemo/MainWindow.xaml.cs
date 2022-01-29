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

namespace QRcodeDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public FTTCPService FileService { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            FileService = new FTTCPService();
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

                HandlePathInputed(files[0]);
            }
        }

        public Bitmap RawBitmap { get; set; }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            string inputUrl = fe_urlTB.Text;

            HandlePathInputed(inputUrl);


            fe_save.Visibility = Visibility.Visible;
        }

        private async void HandlePathInputed(string path)
        {
            try
            {
                if (!path.Contains("https://") || !path.Contains("http://"))
                {
                    FileService.DeleteHostedFile();
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

        private void Generate(string url)
        {
            QRCodeEncoder encoder = new QRCodeEncoder();

            Bitmap btmp = encoder.Encode(url, Encoding.UTF8);
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
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Save file";
            
            fileDialog.ShowDialog();

           // File.Create()
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
    }
}
