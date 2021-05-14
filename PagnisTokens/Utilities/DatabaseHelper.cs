using System;
using System.Collections.Generic;
using MySqlConnector;
using PagnisTokens.Models;

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

        /// <summary>
        /// Funzione per inviare soldi da qualcuno a qualcuno
        /// </summary>
        /// <param name="importo"></param>
        /// <param name="walletSend"></param>
        /// <param name="walletReceive"></param>
        public static void sendMoneyFromTo(double importo, string walletSend, string walletReceive)
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

            addNotificationToUser(getIdUserByWallet(walletSend), "Transazione avvenuta", "Hai inviato " + importo + " a " + getUsernameByWallet(walletReceive));
            addNotificationToUser(getIdUserByWallet(walletReceive), "Transazione avvenuta", "Hai ricevuto " + importo + " da " + getUsernameByWallet(walletSend));
        }

        public static string getUsernameByWallet(string walletId)
		{
            string result = "Qualcuno che non conosco";
            string sqlText = "SELECT * FROM Users WHERE walletid = @walletId";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@walletId", walletId);
            cmd.Prepare();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result = reader.GetString("username");
            }
            reader.Close();
            return result;
        }

        public static List<NotificationModel> getAllNotificationsIdForCurrentUser()
        {
            List<NotificationModel> result = new List<NotificationModel>();
            string sqlText = "SELECT * FROM Notifications WHERE idUser = @idUser";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@idUser", App.Current.Properties["id"]);
            cmd.Prepare();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new NotificationModel {
                    id = reader.GetInt32("id"),
                    idUser = reader.GetInt32("idUser"),
                    title = reader.GetString("title"),
                    message = reader.GetString("message"),
                    seen = reader.GetBoolean("seen")
                });
            }
            reader.Close();
            return result;
        }

        public static void removeNotificationById(int id)
        {
            string sqlText = "DELETE FROM Notifications WHERE id = @id";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

    }
}
