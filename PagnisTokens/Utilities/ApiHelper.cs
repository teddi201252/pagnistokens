using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PagnisTokens.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PagnisTokens.Utilities
{
	public class ApiHelper
	{
		public static string homeBase = "https://mikael-dev.com/pagnistokens";
		public string GET_USER_PAGE = homeBase + $"/users/"; //Id utente
		public string LOGIN_REGISTER_PAGE = homeBase + "/users/";
		public string GET_USERNAME_PAGE = homeBase + $"/users/@"; //Username
		public string GET_WALLET_PAGE = homeBase + $"/wallets/"; //Id 
		public string GET_USER_BY_WALLET_PAGE = homeBase + $"/users/walletid/"; //WalletId
		public string PAY_PAGE = homeBase + "/wallets";
		public string NOTIFICATIONS_PAGE = homeBase + "/notifications";
		public string FRIENDSHIP_PAGE = homeBase + "/friends";
		public string GET_FRIENDSHIP_PAGE = homeBase + $"/friends/"; //Id

		public static HttpClient client;

		public ApiHelper()
		{
			client = new HttpClient();
		}

		public async Task<UserModel> tryLogin(string username, string password)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, new Uri(LOGIN_REGISTER_PAGE));
			request.Headers.Add("username",username);
			request.Headers.Add("password",password);

			HttpResponseMessage response = await client.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				string responseJson = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<UserModel>(responseJson);
			}
			else
			{
				return null;
			}
		}

		public async Task<bool> tryRegister(string username, string password)
		{
			client = new HttpClient();
			StringContent bodyJson = new StringContent("{" +
													"\"username\": \"" + username + "\", " +
													"\"password\": \"" + password + "\"" +
													"}",
													Encoding.UTF8, "application/json");
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(LOGIN_REGISTER_PAGE),
				Method = HttpMethod.Post
			};
			request.Content = bodyJson;

			HttpResponseMessage response = await client.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Ritorna tutte le informazioni di un utente tramite id
		/// </summary>
		/// <param name="idUser"></param>
		/// <returns></returns>
		public async Task<UserModel> getUserById(string idUser)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(string.Format(GET_USER_PAGE + idUser)),
				Method = HttpMethod.Get
			};
			HttpResponseMessage response = await client.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				string responseJson = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<UserModel>(responseJson);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Funzione per inviare soldi da qualcuno a qualcuno
		/// </summary>
		/// <param name="amount"></param>
		/// <param name="sender"></param>
		/// <param name="receiver"></param>
		public async void paySomeone(double amount, string sender, string receiver)
		{
			Uri uri = new Uri(PAY_PAGE);
			string json =	"{" +
							"'amount': '" + amount + "'," +
							"'sender': '" + sender + "'," +
							"'receiver': '" + receiver + "'," +
							"}";
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PutAsync(uri, content);

			UserModel idUserSender = await App.apiHelper.getUserByWalletId(sender);
			UserModel idUserReceiver = await App.apiHelper.getUserByWalletId(receiver);

			App.apiHelper.sendNotification(idUserSender.id, "Transazione avvenuta", "Hai inviato " + amount + " a " + idUserReceiver.username);
			App.apiHelper.sendNotification(idUserReceiver.id, "Transazione avvenuta", "Hai ricevuto " + amount + " da " + idUserSender.username);
		}

		/// <summary>
		/// Ritorna una lista di utenti con il nome simile a quello passato
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public async Task<List<UserModel>> searchUsersByUsername(string username)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(string.Format(GET_USERNAME_PAGE + username)),
				Method = HttpMethod.Get
			};
			HttpResponseMessage response = await client.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				string responseJson = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<UserModel>>(responseJson);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Ritorna l'id utente del proprietario del wallet inserito
		/// </summary>
		/// <returns></returns>
		public async Task<UserModel> getUserByWalletId(string walletId)
		{
			Uri uri = new Uri(string.Format(GET_USER_BY_WALLET_PAGE + walletId));
			HttpResponseMessage response = await client.GetAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				string result = await response.Content.ReadAsStringAsync();
				JObject resultParsed = JObject.Parse(result);
				UserModel returnedUser = new UserModel
				{
					id = resultParsed.Value<string>("id"),
					username = resultParsed.Value<string>("username"),
					walletid = resultParsed.Value<string>("walletid")
				};
				return returnedUser;
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Ritorna tutte le notifiche per l'utente corrente
		/// </summary>
		/// <returns></returns>
		public async Task<List<NotificationModel>> getNotificationsOfUser(string userId)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(NOTIFICATIONS_PAGE),
				Method = HttpMethod.Get
			};
			request.Headers.Add("id", userId);
			HttpResponseMessage response = await client.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				string json = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<List<NotificationModel>>(json);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Aggiungi una notifica all'utente nel databse
		/// </summary>
		/// <param name="idUser"></param>
		/// <param name="title"></param>
		/// <param name="msg"></param>
		public async void sendNotification(string idUser, string title, string message)
		{
			Uri uri = new Uri(NOTIFICATIONS_PAGE);
			string json = "{" +
							"'idUser': '" + idUser + "'," +
							"'title': '" + title + "'," +
							"'message': '" + message + "'" +
							"}";
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync(uri, content);
		}

		/// <summary>
		/// Rimuove dal database la notifica con l'id passato
		/// </summary>
		/// <param name="id"></param>
		public async void deleteNotificationById(int id)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(NOTIFICATIONS_PAGE),
				Method = HttpMethod.Delete
			};
			request.Headers.Add("id", id.ToString());

			HttpResponseMessage response = await client.SendAsync(request);
		}

		public async void updateNotificationById(int id)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(NOTIFICATIONS_PAGE),
				Method = HttpMethod.Put
			};
			request.Headers.Add("id", id.ToString());

			HttpResponseMessage response = await client.SendAsync(request);
		}

		public async Task<WalletModel> getWallet(string walletId)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(string.Format(GET_WALLET_PAGE + walletId)),
				Method = HttpMethod.Get
			};
			HttpResponseMessage response = await client.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				string json = await response.Content.ReadAsStringAsync();
				return JsonConvert.DeserializeObject<WalletModel>(json);
			}
			else
			{
				return null;
			}
		}

		public async Task<string> getBalanceFromWallet(string walletId)
		{
			WalletModel currentWallet = await getWallet(walletId);
			if (currentWallet != null)
			{
				return UtilFunctions.FormatBalance(currentWallet.balance);
			}
			else
			{
				return null;
			}
		}

		public async Task<bool> createNewFriendship(string requestId)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(string.Format(FRIENDSHIP_PAGE)),
				Method = HttpMethod.Post
			};
			request.Content = new StringContent("{" +
				"'id1': '" + App.Current.Properties["id"] + "'," +
				"'id2': '" + requestId + "', " +
				"}");
			HttpResponseMessage response = await client.SendAsync(request);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> refuseFriendshipByIds(string idSender, string idReceiver)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(string.Format(FRIENDSHIP_PAGE)),
				Method = HttpMethod.Delete
			};
			request.Content = new StringContent("{" +
				"'id1': '" + idSender + "'," +
				"'id2': '" + idReceiver + "', " +
				"}");
			HttpResponseMessage response = await client.SendAsync(request);
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> acceptFriendshipByIds(string idSender, string idReceiver)
		{
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(string.Format(FRIENDSHIP_PAGE)),
				Method = HttpMethod.Put
			};
			request.Content = new StringContent("{" +
				"'id1': '" + idSender + "'," +
				"'id2': '" + idReceiver + "', " +
				"}");
			HttpResponseMessage response = await client.SendAsync(request);
			return response.IsSuccessStatusCode;
		}

		/// <summary>
		/// Ritorna una lista di utenti, che sono tutti amici dell'utente locale
		/// </summary>
		/// <returns></returns>
		public async Task<List<UserModel>> getAllFriendsOfCurrentUser()
		{
			List<FriendshipModel> result = new List<FriendshipModel>();
			client = new HttpClient();
			HttpRequestMessage request = new HttpRequestMessage()
			{
				RequestUri = new Uri(string.Format(GET_FRIENDSHIP_PAGE + App.Current.Properties["id"])),
				Method = HttpMethod.Get
			};
			HttpResponseMessage response = await client.SendAsync(request);
			if (response.IsSuccessStatusCode)
			{
				string json = await response.Content.ReadAsStringAsync();
				result = JsonConvert.DeserializeObject<List<FriendshipModel>>(json);
			}
			else
			{
				return null;
			}
			

			List<string> friendIds = new List<string>();
			List<string> accepted = new List<string>();
			foreach (var friendship in result)
			{
				if (friendship.id1 != App.Current.Properties["id"].ToString())
				{
					friendIds.Add(friendship.id1);
					accepted.Add(friendship.accepted);
				}
				else
				{
					friendIds.Add(friendship.id2);
					accepted.Add(null);
				}
			}

			List<UserModel> finalFriends = new List<UserModel>();
			for (int i = 0; i < friendIds.Count; i++)
			{
				finalFriends.Add(await getUserById(friendIds[i]));
				if (accepted[i] == "True")
					finalFriends[i].friendStatusWithCurrent = "accepted";
				else if (accepted[i] == "False")
					finalFriends[i].friendStatusWithCurrent = "toAccept";
			}

			return finalFriends;
		}
	}
}
