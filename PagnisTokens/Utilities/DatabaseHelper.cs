using System;
using MySqlConnector;

namespace PagnisTokens.Utilities
{
    public static class DatabaseHelper
    {
        /// <summary>
        /// Ritorna l'id utente del proprietario del wallet inserito
        /// </summary>
        /// <returns></returns>
        public static int getIdUserByWallet(string walletId)
        {
            int result = -1;
            string sqlText = "SELECT * FROM Users WHERE walletid = @walletId";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@walletId", walletId);
            cmd.Prepare();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result = reader.GetInt32("id");
            }
            reader.Close();
            return result;
        }

        /// <summary>
        /// Aggiungi una notifica all'utente nel databse
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        public static void addNotificationToUser(int idUser, string title, string msg)
        {
            string sqlText = "INSERT INTO Notifications (idUser, title, message) VALUES (@idUser, @titolo, @msg)";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@idUser", idUser);
            cmd.Parameters.AddWithValue("@titolo", title);
            cmd.Parameters.AddWithValue("@msg", msg);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        internal static void sendMoneyFromTo(double importo, string walletSend, string walletReceive)
        {
            
            string sqlText = "UPDATE Wallets SET balance = balance - @importo WHERE id = @walletSend";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@importo", importo);
            cmd.Parameters.AddWithValue("@walletSend", walletSend);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            sqlText = "UPDATE Wallets SET balance = balance + @importo WHERE id = @walletReceive";
            MySqlCommand cmd2 = new MySqlCommand(sqlText, App.Connection);
            cmd2.Parameters.AddWithValue("@importo", importo);
            cmd2.Parameters.AddWithValue("@walletReceive", walletReceive);
            cmd2.Prepare();
            cmd2.ExecuteNonQuery();

            addNotificationToUser(getIdUserByWallet(walletSend), "Transazione avvenuta", "Hai inviato " + importo);
            addNotificationToUser(getIdUserByWallet(walletReceive), "Transazione avvenuta", "Hai ricevuto " + importo);
        }
    }
}
