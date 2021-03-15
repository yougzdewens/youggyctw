using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace TwitterAPI
{
    public class TwitterAPI
    {
		string token;

        public void GetToken(string oAuthConsumerKey, string oAuthConsumerSecret)
        {
			var httpClient = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token ");
			var customerInfo = Convert.ToBase64String(new UTF8Encoding()
									  .GetBytes(oAuthConsumerKey + ":" + oAuthConsumerSecret));
			request.Headers.Add("Authorization", "Basic " + customerInfo);
			request.Content = new StringContent("grant_type=client_credentials",
													Encoding.UTF8, "application/x-www-form-urlencoded");

			HttpResponseMessage response = httpClient.SendAsync(request).GetAwaiter().GetResult();

			string json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

			dynamic item = JsonConvert.DeserializeObject<object>(json);

			token = item["access_token"];
		}

		public IEnumerable<string> GetTweets(string userName, int count, string accessToken = null)
		{
			var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get, string.Format("https://api.twitter.com/1.1/statuses/user_timeline.json?count={0}&screen_name={1}&trim_user=1&exclude_replies=1", count, userName));
			
			requestUserTimeline.Headers.Add("Authorization", "Bearer " + token);
			var httpClient = new HttpClient();
			HttpResponseMessage responseUserTimeLine = httpClient.SendAsync(requestUserTimeline).GetAwaiter().GetResult();

			dynamic item = JsonConvert.DeserializeObject<object>(responseUserTimeLine.Content.ReadAsStringAsync().GetAwaiter().GetResult());

			var enumerableTweets = (item as IEnumerable<dynamic>);

			if (enumerableTweets == null)
			{
				return null;
			}
			return enumerableTweets.Select(t => (string)(t["text"].ToString()));
		}

	}
}
