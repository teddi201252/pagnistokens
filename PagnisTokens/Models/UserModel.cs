using System;
namespace PagnisTokens.Models
{
    public class UserModel
    {
        public int id { set; get; }
        public string username { set; get; }
        public string walletid { set; get; }
        public string friendStatusWithCurrent { set; get; } = null; 
    }
}
