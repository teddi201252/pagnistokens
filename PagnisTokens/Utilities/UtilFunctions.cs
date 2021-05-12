using System;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;

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

        public static Color ReverseColor(Color color)
		{
            return new Color(255 - color.R, 255 - color.G, 255 - color.B);
		}
    }
}
