using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;
using System.Drawing;

namespace PagnisTokens.Utilities
{
    public static class UtilFunctions
    {
        public static string GetHashedText(string source)
        {
            SHA1 sha1Hash = SHA1.Create();
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            return hash;
        }

        public static Xamarin.Forms.Color ReverseColor(Xamarin.Forms.Color color)
		{
            return new Xamarin.Forms.Color(1 - color.R, 1 - color.G, 1 - color.B);
		}

        public static string FormatBalance(double balance)
		{
            string result;
            string interi = Math.Floor(balance).ToString();
            double decimali = balance - int.Parse(interi);
            decimali *= 1000000;
            decimali = Math.Round(decimali);

            string tempInteri = interi;
			for (int i = 0; i < interi.Length; i++)
			{
				if (i%3 == 0 && i != 0)
				{
                    tempInteri = tempInteri.Insert(interi.Length - i, ".");
				}
			}
            interi = tempInteri;

            result = interi + "," + decimali.ToString();
            return result;
		}

        public static byte[] BitmapToByte(System.Drawing.Bitmap img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
