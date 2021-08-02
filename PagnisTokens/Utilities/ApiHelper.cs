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
		public string GET_USER_PAGE = $"/pagnistokens/users/{{id}}";
		public string GET_USERNAME_PAGE = $"/pagnistokens/users/@{{username}}";
		public string GET_WALLET_PAGE = $"/pagnistokens/wallets/{{id}}";
		public string GET_USER_BY_WALLET_PAGE = $"/pagnistokens/users/walletid/{{walletId}}";
		public string PAY_PAGE = "/pagnistokens/wallets";
		public string NOTIFICATIONS_PAGE = "/pagnistokens/notifications";
		public string FRIENDSHIP_PAGE = "/pagnistokens/friends";
		public string GET_FRIENDSHIP_PAGE = $"/pagnistokens/friends/{{id}}";

		public static HttpClient client;

		public ApiHelper()
		{
			client = new HttpClient();
		}

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
		}

		public async void sendNotification(string idUser, string title, string message)
		{
			Uri uri = new Uri(NOTIFICATIONS_PAGE);
			string json =   "{" +
							"'idUser': '" + idUser + "'," +
							"'title': '" + title + "'," +
							"'message': '" + message + "'," +
							"}";
			StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
			HttpResponseMessage response = await client.PostAsync(uri, content);
		}

		public async Task<UserModel> getUserByWalletId(string walletId)
		{
			Uri uri = new Uri(string.Format(GET_USER_BY_WALLET_PAGE, walletId));
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

		public async void getNotificationsOfUser(string userId) //Task<List<NotificationModel>>
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
			}
		}
	}
}
