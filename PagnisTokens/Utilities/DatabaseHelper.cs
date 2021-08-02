﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using PagnisTokens.Models;
using Xamarin.Forms;

namespace PagnisTokens.Utilities
{
    public static class DatabaseHelper
    {
        public static string getBalanceFromWallet(string walletId)
        {
            string result = "";
            string sqlText = "SELECT balance FROM Wallets WHERE id = @walletid";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@walletid", walletId);
            cmd.Prepare();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result = UtilFunctions.FormatBalance(reader.GetDouble(0));
            }
            reader.Close();
            return result;
        }

        /// <summary>
        /// Ritorna l'id utente del proprietario del wallet inserito
        /// </summary>
        /// <returns></returns>
        public static async Task<UserModel> getUserByWallet(string walletId)
        {
            return await App.apiHelper.getUserByWalletId(walletId);
        }

        /// <summary>
        /// Aggiungi una notifica all'utente nel databse
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        public static void addNotificationToUser(string idUser, string title, string msg)
        {
            App.apiHelper.sendNotification(idUser, title, msg);
        }

        /// <summary>
        /// Funzione per inviare soldi da qualcuno a qualcuno
        /// </summary>
        /// <param name="importo"></param>
        /// <param name="walletSend"></param>
        /// <param name="walletReceive"></param>
        public static async void sendMoneyFromTo(double importo, string walletSend, string walletReceive)
        {
			App.apiHelper.paySomeone(importo, walletSend, walletReceive);

            UserModel idUserSender = await getUserByWallet(walletSend);
            UserModel idUserReceiver = await getUserByWallet(walletReceive);

            addNotificationToUser(idUserSender.id, "Transazione avvenuta", "Hai inviato " + importo + " a " + idUserReceiver.username);
            addNotificationToUser(idUserReceiver.id, "Transazione avvenuta", "Hai ricevuto " + importo + " da " + idUserSender.username);
        }

        /// <summary>
        /// Ritorna tutte le notifiche per l'utente corrente
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Rimuove dal database la notifica con l'id passato
        /// </summary>
        /// <param name="id"></param>
        public static void removeNotificationById(int id)
        {
            string sqlText = "DELETE FROM Notifications WHERE id = @id";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Ritorna una lista di utenti con il nome simile a quello passato
        /// </summary>
        /// <param name="namePart"></param>
        /// <returns></returns>
        public static List<UserModel> searchUsersByUsername(string namePart)
        {
            List<UserModel> result = new List<UserModel>();
            string sqlText = "SELECT * FROM Users WHERE username LIKE @namePart";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@namePart", "%" + namePart + "%");
            cmd.Prepare();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result.Add(new UserModel
                {
                    id = reader.GetInt32("id"),
                    username = reader.GetString("username"),
                    walletid = reader.GetString("walletid")
                });
            }
            reader.Close();
            return result;
        }

        /// <summary>
        /// Ritorna tutte le informazioni di un utente tramite id
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public static UserModel getUserById(int idUser)
		{
            UserModel result = null;
            string sqlText = "SELECT * FROM Users WHERE id = @idUser";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@idUser", idUser);
            cmd.Prepare();
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                result = new UserModel
                {
                    id = reader.GetInt32("id"),
                    username = reader.GetString("username"),
                    walletid = reader.GetString("walletid")
                };
            }
            reader.Close();
            return result;
        }

        /// <summary>
        /// Ritorna una lista di utenti, che sono tutti amici dell'utente locale
        /// </summary>
        /// <returns></returns>
        public static List<UserModel> getAllFriendsOfCurrentUser()
		{
            List<UserModel> result = new List<UserModel>();
            string sqlText = "SELECT * FROM FriendRelations WHERE id1 = @idUser OR id2 = @idUser";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@idUser", App.Current.Properties["id"]);
            cmd.Prepare();
            MySqlDataReader reader = cmd.ExecuteReader();
            List<int> friendIds = new List<int>();
            List<string> accepted = new List<string>();
            while (reader.Read())
            {
				if (reader.GetInt32("id1") != int.Parse(App.Current.Properties["id"].ToString()))
				{
                    friendIds.Add(reader.GetInt32("id1"));
                    accepted.Add(reader.GetBoolean("accepted").ToString());
                }
				else
				{
                    friendIds.Add(reader.GetInt32("id2"));
                    accepted.Add(null);
                }
            }
            reader.Close();

            for (int i = 0; i < friendIds.Count; i++)
            {
                result.Add(getUserById(friendIds[i]));
                if (accepted[i] == "True")
                    result[i].friendStatusWithCurrent = "accepted";
                else if (accepted[i] == "False")
                    result[i].friendStatusWithCurrent = "toAccept";
            }

            return result;
        }

        public static void createNewFriendship(int requestId)
        {
            string sqlText = "INSERT INTO FriendRelations (id1, id2) VALUES (@idUser, @requestId)";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@idUser", App.Current.Properties["id"]);
            cmd.Parameters.AddWithValue("@requestId", requestId);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public static void refuseFriendshipByIds(int idSender, int idReceiver)
        {
            string sqlText = "DELETE FROM FriendRelations WHERE id1 = @idSender AND id2 = @idReceiver";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@idSender", idSender);
            cmd.Parameters.AddWithValue("@idReceiver", idReceiver);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        public static void acceptFriendshipByIds(int idSender, int idReceiver)
        {
            string sqlText = "UPDATE FriendRelations SET accepted = true WHERE id1 = @idSender AND id2 = @idReceiver";
            MySqlCommand cmd = new MySqlCommand(sqlText, App.Connection);
            cmd.Parameters.AddWithValue("@idSender", idSender);
            cmd.Parameters.AddWithValue("@idReceiver", idReceiver);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }
    }
}
