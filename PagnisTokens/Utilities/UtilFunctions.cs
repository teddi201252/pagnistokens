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

        public static string FormatBalance(double balance)
		{
            string result;
            string interi = Math.Floor(balance).ToString();
            double decimali = balance - int.Parse(interi);
            decimali *= 100;
            decimali = Math.Floor(decimali);

            string tempInteri = interi;
			for (int i = interi.Length; i > 0; i--)
			{
				if (i%3 == 0 && i != 0)
				{
                    tempInteri = tempInteri.Insert(i-1, ".");
				}
			}
            interi = tempInteri;

            result = interi + "," + decimali.ToString();
            return result;
		}
    }
}
