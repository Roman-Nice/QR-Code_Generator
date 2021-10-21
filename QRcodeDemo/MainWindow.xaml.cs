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

namespace QRcodeDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public Bitmap RawBitmap { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Generate();
            DisplayBitmap();
        }

        private void Generate()
        {
            QRCodeEncoder encoder = new QRCodeEncoder();
            string url = fe_urlTB.Text;

            Bitmap btmp = encoder.Encode(url, Encoding.UTF8);
            RawBitmap = btmp;


            fe_Image.Source = (ImageSource) ConvertToImageSource(btmp);
        }

        public object ConvertToImageSource(object value)
        {
            Bitmap bmp = value as Bitmap;
            if (bmp == null)
                return null;

            using (Stream str = new MemoryStream())
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
    }
}
