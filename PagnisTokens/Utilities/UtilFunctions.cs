using System;
using System.Security.Cryptography;
using System.Text;

namespace PagnisTokens.Utilities
{
    public static class UtilFunctions
    {
        public static string GetHashedText(string inputData)
        {
            byte[] tmpSource;
            byte[] tmpData;
            tmpSource = Encoding.ASCII.GetBytes(inputData);
            tmpData = new SHA1CryptoServiceProvider().ComputeHash(tmpSource);
            return Convert.ToBase64String(tmpData);
        }
    }
}
