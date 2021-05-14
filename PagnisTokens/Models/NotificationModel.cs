using System;
namespace PagnisTokens.Models
{
    public class NotificationModel
    {
        public int id { set; get; }
        public int idUser { set; get; }
        public string title { set; get; }
        public string message { set; get; }
        public bool seen { set; get; }
    }
}
