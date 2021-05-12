using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using QRCoder;
using SkiaSharp;
using Xamarin.Forms;

namespace PagnisTokens.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(Application.Current.Properties["walletid"].ToString(), QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeBytes = qrCode.GetGraphic(20, "#0902a8", "#7977a3");
            
            ImageSource QrCodeImageSource = ImageSource.FromStream(() => new MemoryStream(qrCodeBytes));
            QrCodeImage.Source = QrCodeImageSource;
        }
    }
}
