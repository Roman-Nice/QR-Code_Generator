using MessagingToolkit.QRCode.Codec;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QRcodeGenerator.Data
{
    public class QRgeneratorService
    {
        
        public QRgeneratorService(string url, int scale = 250, Encoding charset = null)
        {
            Url = url;
            Scale = scale;
            Charset ??= Encoding.UTF8;
        }

        public string Url { get; init; }
        public int Scale { get; init; }
        public Encoding Charset { get; init; }

        public string ImageBase64 { get; set; }

        public string Generate()
        {
            QRCodeEncoder encoder = new QRCodeEncoder();
            encoder.QRCodeScale = Scale;

            Bitmap btmp = encoder.Encode(Url, Charset);

            ImageBase64 = ConvertToBase64(btmp);

            return ImageBase64;
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
