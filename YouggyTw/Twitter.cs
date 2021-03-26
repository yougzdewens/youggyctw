using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAuth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using YouggyTw.Model;
using YouggyTw.Tools;

namespace YouggyTw
{
    /// <summary>
    /// 
    /// </summary>
    public class Twitter
    {
        /// <summary>
        /// Gets or sets the bearer token.
        /// </summary>
        /// <value>
        /// The bearer token.
        /// </value>
        public string BearerToken { get; set; }

        /// <summary>
        /// The limit manager list
        /// </summary>
        private Dictionary<NameRequestTwitterAPI, LimitManager> limitManagerList;

        private OAuthTokens oAuthTokens = new OAuthTokens();

        /// <summary>
        /// The time out before request
        /// </summary>
        const int TimeOutBeforeRequest = 15;

        /// <summary>
        ///   <br />
        /// </summary>
        private enum NameRequestTwitterAPI
        {
            /// <summary>
            /// The verify credential
            /// </summary>
            VerifyCredential,

            /// <summary>
            /// The friend list
            /// </summary>
            FriendList,

            /// <summary>
            /// The get user timeline
            /// </summary>
            GetUserTimeline,

            /// <summary>
            /// The retweet or update
            /// </summary>
            RetweetOrUpdate,

            /// <summary>
            /// The delete friend
            /// </summary>
            DeleteFriend,

            MentionTimeline,

            SendDirectMessage,

            GetDirectMessage
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Twitter"/> class.
        /// </summary>
        /// <param name="ConsumerKey">The consumer key.</param>
        /// <param name="ConsumerSecret">The consumer secret.</param>
        public Twitter(string consumerKey, string consumerSecret, string accessToken, string accessTokenSecret)
        {
            oAuthTokens.AccessToken = accessToken;
            oAuthTokens.AccessTokenSecret = accessTokenSecret;
            oAuthTokens.ConsumerKey = consumerKey;
            oAuthTokens.ConsumerSecret = consumerSecret;

            limitManagerList = CreateLimitManagerList();
        }

        public void DeleteFriend(long userId)
        {
            string url = "https://api.twitter.com/1.1/friendships/destroy.json?user_id=" + userId;

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            //parameters.Add("user_id", user.Id);

            ////TODO FIND THE GOOD OBJECT
            //string returnTest = TwitterApiPostCall<string>(url, NameRequestTwitterAPI.DeleteFriend);

            //OAuthYouggy oay = new OAuthYouggy(consumerKey, consumerSecret, access_token, access_token_secret);
            string returnTest = TwitterApiCall<string>(url, NameRequestTwitterAPI.RetweetOrUpdate, HTTPVerb.POST, parameters);
        }

        public List<MentionTimeline> GetTweetMentioningMe()
        {
            string url = "https://api.twitter.com/1.1/statuses/mentions_timeline.json";

            return TwitterApiCall<MentionTimeline[]>(url, NameRequestTwitterAPI.MentionTimeline, HTTPVerb.GET, new Dictionary<string, object>()).ToList();
        }

        /// <summary>
        /// Gets the user timeline.
        /// </summary>
        /// <param name="user_id">The user identifier.</param>
        /// <param name="since_id">The since identifier.</param>
        /// <returns></returns>
        public List<Tweet> GetUserTimeline(string user_id, string since_id)
        {
            string url = "https://api.twitter.com/1.1/statuses/user_timeline.json?user_id=" + user_id + "&tweet_mode=extended";

            if (!string.IsNullOrEmpty(since_id))
            {
                url = url + "&since_id=" + since_id;
            }

            return TwitterApiCall<Tweet[]>(url, NameRequestTwitterAPI.GetUserTimeline, HTTPVerb.GET, new Dictionary<string, object>()).ToList();
        }

        /// <summary>
        /// Verifies the credentials.
        /// </summary>
        /// <returns></returns>
        public Account VerifyCredentials()
        {
            string url = "https://api.twitter.com/1.1/account/verify_credentials.json";

            return TwitterApiCall<Account>(url, NameRequestTwitterAPI.VerifyCredential, HTTPVerb.GET, new Dictionary<string, object>());
        }

        /// <summary>
        /// Gets all friends.
        /// </summary>
        /// <returns></returns>
        public List<User> GetAllFriends()
        {
            Tuple<List<User>, long> usersAndCursor = GetFriends();
            List<User> users = new List<User>();

            users.AddRange(usersAndCursor.Item1);

            while (usersAndCursor.Item2 > 0)
            {
                usersAndCursor = GetFriends(usersAndCursor.Item2);
                users.AddRange(usersAndCursor.Item1);
            }

            return users;
        }

        public Tuple<List<User>, long> GetFriends(long nextCursor = 0)
        {
            string url = "https://api.twitter.com/1.1/friends/list.json" + (nextCursor > 0 ? "?cursor=" + nextCursor : string.Empty);

            Users friends = TwitterApiCall<Users>(url, NameRequestTwitterAPI.FriendList, HTTPVerb.GET, new Dictionary<string, object>());
            return new Tuple<List<User>, long>(friends.UserList, friends.NextCursor);       
        }

        public List<EventTwitter> GetAllDirectMessages(string lastDirectMessage)
        {
            Tuple<List<EventTwitter>, long> eventsAndCursor = GetDirectMessages();
            List<EventTwitter> events = new List<EventTwitter>();

            events.AddRange(eventsAndCursor.Item1);

            while (eventsAndCursor.Item2 > 0)
            {
                eventsAndCursor = GetDirectMessages(eventsAndCursor.Item2);
                events.AddRange(eventsAndCursor.Item1);
            }

            if (!string.IsNullOrEmpty(lastDirectMessage))
            {
                events = events.OrderBy(x => x.Id).Where(x => x.Id > long.Parse(lastDirectMessage)).ToList();
            }          

            return events;
        }

        public Tuple<List<EventTwitter>, long> GetDirectMessages(long nextCursor = 0)
        {
            string url = "https://api.twitter.com/1.1/direct_messages/events/list.json" + (nextCursor > 0 ? "?cursor=" + nextCursor : string.Empty);

            EventsTwitter eventsTwitter = TwitterApiCall<EventsTwitter>(url, NameRequestTwitterAPI.GetDirectMessage, HTTPVerb.GET, new Dictionary<string, object>());
            return new Tuple<List<EventTwitter>, long>(eventsTwitter.Events, eventsTwitter.NextCursor);
        }

        /// <summary>
        /// Retweets a tweet.
        /// </summary>
        /// <param name="tweetFromFriend">The tweet from friend.</param>
        /// <returns></returns>
        public bool RetweetATweet(Tweet tweetFromFriend)
        {
            string url = "https://api.twitter.com/1.1/statuses/retweet/" + tweetFromFriend.Id + ".json";

            // TODO FIND THE GOOD OBJECT
            string returnTest = TwitterApiCall<string>(url, NameRequestTwitterAPI.RetweetOrUpdate, HTTPVerb.POST, new Dictionary<string, object>());

            return true;
        }

        /// <summary>
        /// Likes a tweet.
        /// </summary>
        /// <param name="tweetFromFriend">The tweet from friend.</param>
        public void LikeATweet(Tweet tweetFromFriend)
        {
            string url = "https://api.twitter.com/1.1/favorites/create.json?id=" + tweetFromFriend.Id;

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            //queryParameters.Add("id", tweetFromFriend.Id);

            // TODO FIND THE GOOD OBJECT
            string returnTest = TwitterApiCall<string>(url, NameRequestTwitterAPI.RetweetOrUpdate, HTTPVerb.POST, queryParameters);
        }

        /// <summary>
        /// Replies the specified tweet from friend.
        /// </summary>
        /// <param name="tweetFromFriend">The tweet from friend.</param>
        /// <param name="status">The status.</param>
        public void Reply(Tweet tweetFromFriend, string status)
        {
            string url = "https://api.twitter.com/1.1/statuses/update.json?status=" + status + "&in_reply_to_status_id="+ tweetFromFriend.Id;

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            queryParameters.Add("status", status);
            queryParameters.Add("in_reply_to_status_id", tweetFromFriend.Id);

            //TODO FIND THE GOOD OBJECT
            string returnTest = TwitterApiCall<string>(url, NameRequestTwitterAPI.RetweetOrUpdate, HTTPVerb.POST, queryParameters);
        }

        /// <summary>
        /// Adds the friends.
        /// </summary>
        /// <param name="twitterMention">The twitter mention.</param>
        public void AddFriends(UserMention twitterMention)
        {
            string url = "https://api.twitter.com/1.1/friendships/create.json?user_id="+ twitterMention.Id + "&follow=true";
            //string url = "https://api.twitter.com/1.1/friendships/create.json";

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            //queryParameters.Add("user_id", twitterMention.Id);
            //queryParameters.Add("follow", "true");

            //TODO FIND THE GOOD OBJECT
            string returnTest = TwitterApiCall<string>(url, NameRequestTwitterAPI.RetweetOrUpdate, HTTPVerb.POST, queryParameters);
        }

        /// <summary>
        /// Twitters the API get call.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="address">The address.</param>
        /// <param name="requestTwitter">The request twitter.</param>
        /// <returns></returns>
        private T TwitterApiCall<T>(string address, NameRequestTwitterAPI requestTwitter, HTTPVerb verb, Dictionary<string, object> queryParameters, bool multipart = false, string data = "")
        {
            try
            {
                limitManagerList[requestTwitter].AddCall();

                Uri uri = new Uri(address.Replace("http://", "https://"));               

                WebRequestBuilder requestBuilder = new WebRequestBuilder(uri, verb, oAuthTokens) { Multipart = multipart };

                if (data != string.Empty)
                {
                    Encoding encoding = new UTF8Encoding();
                    requestBuilder.DataInByte = encoding.GetBytes(data);
                }

                foreach (var item in queryParameters)
                {
                    requestBuilder.Parameters.Add(item.Key, item.Value);
                }

                HttpWebResponse response = requestBuilder.ExecuteRequest();

                //OAuthRequest client = OAuthRequest.ForProtectedResource("GET", consumerKey, consumerSecret, access_token, access_token_secret);
                //client.RequestUrl = address;

                //// add HTTP header authorization
                //string auth = client.GetAuthorizationHeader();
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(client.RequestUrl);
                //request.Headers.Add("Authorization", auth);

                //Console.WriteLine("Calling " + address);

                //// make the call and print the string value of the response JSON
                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string strResponse = reader.ReadToEnd();

                if (typeof(T) == typeof(string))
                {
                    return (T)Convert.ChangeType(strResponse, typeof(T));
                }
                else
                {
                    return JsonConvert.DeserializeObject<T>(strResponse);
                }
            }
            catch (System.Net.WebException ex)
            {
                if ((int)((HttpWebResponse)ex.Response).StatusCode == 429)
                {
                    LogTools.WriteLog("erreur 429, " + TimeOutBeforeRequest + "m d'attente pour l'appel : " + address);
                    Thread.Sleep(TimeOutBeforeRequest * 1000 * 60);
                    return TwitterApiCall<T>(address, requestTwitter, verb, queryParameters, multipart);
                }
                else if ((int)((HttpWebResponse)ex.Response).StatusCode == 403)
                {
                    LogTools.WriteLog("Erreur 403 pour l'adresse : " + address);
                    return default(T);
                }
                else
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool SendDirectMessage(long id, string text)
        {
            string url = "https://api.twitter.com/1.1/direct_messages/events/new.json";

            EventTwitter eventTwitter = new EventTwitter();
            eventTwitter.Type = "message_create";
            eventTwitter.MessageCreate.Target.RecipientId = id.ToString();
            eventTwitter.MessageCreate.MessageData.Text = text;

            Dictionary<string, object> queryParameters = new Dictionary<string, object>();
            string data = "{\"event\" :" + JsonConvert.SerializeObject(eventTwitter) + "}";

            //TODO FIND THE GOOD OBJECT
            string returnTest = TwitterApiCall<string>(url, NameRequestTwitterAPI.SendDirectMessage, HTTPVerb.POST, queryParameters, false, data); ;

            return true;

            //Dictionary<string, object> queryParameters = new Dictionary<string, object>();

            //string url = "https://api.twitter.com/1.1/direct_messages/new.json?user_id="+id+"&text=some message";
            //string returnTest = TwitterApiCall<string>(url, NameRequestTwitterAPI.SendDirectMessage, HTTPVerb.POST, queryParameters);

            //return true;
        }        

        ///// <summary>
        ///// Twitters the API post call.
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="address">The address.</param>
        ///// <param name="requestTwitter">The request twitter.</param>
        ///// <returns></returns>
        //private T TwitterApiPostCall<T>(string address, NameRequestTwitterAPI requestTwitter)
        //{
        //    try
        //    {
        //        limitManagerList[requestTwitter].AddCall();

        //        OAuthRequest client = OAuthRequest.ForProtectedResource("POST", consumerKey, consumerSecret, access_token, access_token_secret);
        //        client.RequestUrl = address;

        //        // add HTTP header authorization
        //        string auth = client.GetAuthorizationHeader();
        //        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(client.RequestUrl);
        //        request.Headers.Add("Authorization", auth);

        //        Console.WriteLine("Calling " + address);

        //        // make the call and print the string value of the response JSON
        //        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //        Stream dataStream = response.GetResponseStream();
        //        StreamReader reader = new StreamReader(dataStream);
        //        string strResponse = reader.ReadToEnd();

        //        if (typeof(T) == typeof(string))
        //        {
        //            return (T)Convert.ChangeType(strResponse, typeof(T));
        //        }
        //        else
        //        {
        //            return JsonConvert.DeserializeObject<T>(strResponse);
        //        }

        //    }
        //    catch (System.Net.WebException ex)
        //    {
        //        if ((int)((HttpWebResponse)ex.Response).StatusCode == 429)
        //        {
        //            limitManagerList[requestTwitter].WaitBeforeNewTry();
        //            return TwitterApiGetCall<T>(address, requestTwitter);
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// Creates the limit manager list.
        /// </summary>
        /// <returns></returns>
        private Dictionary<NameRequestTwitterAPI, LimitManager> CreateLimitManagerList()
        {
            Dictionary<NameRequestTwitterAPI, LimitManager> limitManagerList = new Dictionary<NameRequestTwitterAPI, LimitManager>();

            limitManagerList.Add(NameRequestTwitterAPI.FriendList, new LimitManager(
                new List<Tuple<int, int>>() {
                    new Tuple<int, int>(15, 15)
                })
            );

            limitManagerList.Add(NameRequestTwitterAPI.GetUserTimeline, new LimitManager(
                new List<Tuple<int, int>>() {
                    new Tuple<int, int>(900, 15),
                    new Tuple<int, int>(100000, 1440)
                })
            );

            limitManagerList.Add(NameRequestTwitterAPI.VerifyCredential, new LimitManager(
                new List<Tuple<int, int>>() {
                    new Tuple<int, int>(75, 15)
                })
            );

            limitManagerList.Add(NameRequestTwitterAPI.RetweetOrUpdate, new LimitManager(
                new List<Tuple<int, int>>() {
                    new Tuple<int, int>(300, 180)
                })
            );

            limitManagerList.Add(NameRequestTwitterAPI.DeleteFriend, new LimitManager(
                new List<Tuple<int, int>>() {
                    new Tuple<int, int>(15, 15)
                })
            );

            limitManagerList.Add(NameRequestTwitterAPI.SendDirectMessage, new LimitManager(
                new List<Tuple<int, int>>() {
                    new Tuple<int, int>(1000, 1440)
                })
            );            

            limitManagerList.Add(NameRequestTwitterAPI.MentionTimeline, new LimitManager(
                new List<Tuple<int, int>>() {
                    new Tuple<int, int>(75, 15),
                    new Tuple<int, int>(100000, 1440)
                })
            );

            limitManagerList.Add(NameRequestTwitterAPI.GetDirectMessage, new LimitManager(
                new List<Tuple<int, int>>() {
                    new Tuple<int, int>(15, 15)
                })
            );

            return limitManagerList;
        }

        //private string GetTweetIdFromUrl(string strTweetUrl)
        //{
        //    string[] strLinkParts = strTweetUrl.Split(new[] { '/' });
        //    return strLinkParts[strLinkParts.Length - 1];
        //}

        //public TwitterTweetDetails GetTweetDetails(string strTweetUrl)
        //{
        //    string strTweetId = this.GetTweetIdFromUrl(strTweetUrl);
        //    TwitterTweetDetails d = new TwitterTweetDetails();
        //    d.TweetLink = strTweetUrl;
        //    d.TweetId = strTweetId;
        //    JObject jTweet = this.GetTweetDetailsFromTweet(strTweetId);
        //    JObject jUser = null;
        //    string strUserId = string.Empty;
        //    if (jTweet["user"] != null && jTweet["user"]["id_str"] != null)
        //    {
        //        strUserId = jTweet["user"]["id_str"].ToString();
        //        d.UserId = strUserId;
        //        jUser = GetUserDetails(strUserId);
        //    }
        //    if (jUser != null)
        //    {
        //        if (jUser["name"] != null) d.AuthorName = jUser["name"].ToString();
        //        if (jUser["screen_name"] != null) d.AuthorNick = jUser["screen_name"].ToString();
        //        if (jUser["profile_image_url"] != null) d.AuthorPhoto = jUser["profile_image_url"].ToString().Replace("_normal.", ".");
        //    }
        //    if (jTweet["text"] != null)
        //    {
        //        d.Content = Regex.Replace(jTweet["text"].ToString(), @"\p{Cs}", string.Empty);//remove emoji
        //    }

        //    if (jTweet["entities"] != null
        //        && jTweet["entities"]["media"] != null
        //        && jTweet["entities"]["media"][0] != null
        //        && jTweet["entities"]["media"][0]["media_url"] != null)
        //    {
        //        d.Picture = jTweet["entities"]["media"][0]["media_url"].ToString();
        //    }

        //    return d;
        //}

        //public JObject TwitterApiGetCall(string address)
        //{
        //    WebRequest request = WebRequest.Create(address);
        //    request.Headers["Authorization"] = "Bearer " + this.BearerToken;
        //    request.Method = "GET";
        //    request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
        //    string responseJson = string.Empty;

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponseAsync().GetAwaiter().GetResult();
        //    if (response.StatusCode == HttpStatusCode.OK)
        //    {
        //        Stream responseStream = response.GetResponseStream();
        //        responseJson = new StreamReader(responseStream).ReadToEnd();
        //    }

        //    JObject jobjectResponse = JObject.Parse(responseJson);
        //    return jobjectResponse;
        //}

        //public JObject GetTweetDetailsFromTweet(string tweet)
        //{
        //    string address = string.Format("https://api.twitter.com/1.1/statuses/show/{0}.json?trim_user=true", tweet);
        //    return TwitterApiGetCall(address);
        //}

        //public JObject GetUserDetails(string user_id)
        //{
        //    string address = string.Format("https://api.twitter.com/1.1/users/show.json?user_id={0}&include_entities=false", user_id);
        //    return TwitterApiGetCall(address);
        //}
    }
}
