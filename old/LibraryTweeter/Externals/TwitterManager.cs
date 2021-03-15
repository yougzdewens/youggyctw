using LibraryTweeter.Business;
using LibraryTweeter.Model;
using LibraryTweeter.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TweetSharp;

namespace LibraryTweeter.Externals
{
    /// <summary>
    /// 
    /// </summary>
    public class TwitterManager
    {
        #region private field
        /// <summary>
        /// The consumer key
        /// </summary>
        private string consumerKey = System.Configuration.ConfigurationManager.AppSettings["consumerKey"];

        /// <summary>
        /// The consumer secret
        /// </summary>
        private string consumerSecret = System.Configuration.ConfigurationManager.AppSettings["consumerSecret"];

        /// <summary>
        /// The token application
        /// </summary>
        private string tokenApp = System.Configuration.ConfigurationManager.AppSettings["tokenApp"];

        /// <summary>
        /// The token secret
        /// </summary>
        private string tokenSecret = System.Configuration.ConfigurationManager.AppSettings["tokenSecret"];
        #endregion

        /// <summary>
        /// Gets all friends.
        /// </summary>
        /// <returns></returns>
        public List<TwitterUser> GetAllFriends()
        {
            LogTool.WriteLog("consumerKey : " + consumerKey);
            LogTool.WriteLog("consumerSecret : " + consumerSecret);

            LogTool.WriteLog("tokenApp : " + tokenApp);
            LogTool.WriteLog("tokenSecret : " + tokenSecret);

            TweetSharp.TwitterService service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(tokenApp, tokenSecret);
            
            TwitterCursorList<TwitterUser> friendsList = service.ListFriends(new ListFriendsOptions { Cursor = -1, SkipStatus = true });
            CheckLimit(service);

            LogTool.WriteLog(service.Response.Response);

            List<TwitterUser> twitterUsersFriends = new List<TwitterUser>();

            while (friendsList != null && friendsList.NextCursor != null && friendsList.NextCursor >= 0)
            {
                twitterUsersFriends.AddRange(friendsList.ToList());

                friendsList = service.ListFriends(new ListFriendsOptions { Cursor = friendsList.NextCursor, SkipStatus = true });
                CheckLimit(service);
            }

            return twitterUsersFriends;
        }

        public long GetMyId()
        {
            var service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(tokenApp, tokenSecret);
            var user = service.GetUserProfile(new GetUserProfileOptions() { });

            if (user != null)
            {
                return user.Id;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Gets the tweet from friend.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <param name="lastTweetSeen">The last tweet seen.</param>
        /// <returns></returns>
        public List<TwitterStatus> GetTweetFromFriend(long userId, long? lastTweetSeen = null)
        {
            var service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(tokenApp, tokenSecret);

            ListTweetsOnUserTimelineOptions listTweetsOptions = new ListTweetsOnUserTimelineOptions();
            listTweetsOptions.IncludeRts = false;
            listTweetsOptions.ExcludeReplies = true;
            listTweetsOptions.TweetMode = TweetMode.Extended;

            if (lastTweetSeen != null)
            {
                listTweetsOptions.SinceId = lastTweetSeen;
            }

            listTweetsOptions.UserId = userId;

            IEnumerable<TwitterStatus> tweetsFromFriend = service.ListTweetsOnUserTimeline(listTweetsOptions);

            CheckLimit(service);

            if (tweetsFromFriend != null)
            {
                return tweetsFromFriend.ToList();
            }
            else
            {
                return new List<TwitterStatus>();
            }
        }

        /// <summary>
        /// Retweets a tweet.
        /// </summary>
        /// <param name="tweet">The tweet.</param>
        /// <returns></returns>
        public bool RetweetATweet(TwitterStatus tweet)
        {
            if (tweet.IsRetweeted == false)
            {
                var service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
                service.AuthenticateWith(tokenApp, tokenSecret);

                RetweetOptions retweetsOptions = new RetweetOptions();
                retweetsOptions.Id = tweet.Id;

                service.Retweet(retweetsOptions);

                CheckLimit(service);

                return TestResponseOk(tweet, service);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Likes a tweet.
        /// </summary>
        /// <param name="tweet">The tweet.</param>
        /// <returns></returns>
        public bool LikeATweet(TwitterStatus tweet)
        {
            if (tweet.IsFavorited == false)
            {
                var service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
                service.AuthenticateWith(tokenApp, tokenSecret);

                FavoriteTweetOptions favoriteOptions = new FavoriteTweetOptions();
                favoriteOptions.Id = tweet.Id;

                service.FavoriteTweet(favoriteOptions);

                CheckLimit(service);

                return TestResponseOk(tweet, service);
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the direct message received.
        /// </summary>
        /// <param name="sinceLastId">The since last identifier.</param>
        /// <returns></returns>
        public Messages GetDirectMessageReceived(long? sinceLastId)
        {
            TweetSharp.TwitterService service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(tokenApp, tokenSecret);

            TwitterAsyncResult<TwitterDirectMessageResult> responseDirectMessage = null;

            CheckLimit(service);

            var test2 = service.ListDirectMessages(new ListDirectMessagesOptions() { Count = 5 });

            Messages msg = null;

            if (test2 != null)
            {
                msg = (Messages)new JsonSerializer().Deserialize(test2.RawSource, typeof(Messages));
            }

            return msg; 
        }

        /// <summary>
        /// Deletes the direct messages.
        /// </summary>
        /// <returns>
        /// nb deleted messages
        /// </returns>
        public int DeleteDirectMessages()
        {
            int countDeleted = 0;

            TweetSharp.TwitterService service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(tokenApp, tokenSecret);

            TwitterDirectMessageListResult directMessages = service.ListDirectMessages(new ListDirectMessagesOptions() { });

            if (directMessages != null)
            {
                foreach (TwitterDirectMessageEvent eventTwitter in directMessages.Events)
                {
                    service.DeleteDirectMessage(new DeleteDirectMessageOptions() { Id = eventTwitter.Id });
                }
            }

            return countDeleted;
        }

        /// <summary>
        /// Gets the tweets mentionning me.
        /// </summary>
        /// <returns></returns>
        public List<TwitterStatus> GetTweetsMentionningMe()
        {
            TweetSharp.TwitterService service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(tokenApp, tokenSecret);

            CitationBusiness citationBusiness = new CitationBusiness();
            long? lastCitationSeend = citationBusiness.GetLastCitationSeen();

            IEnumerable<TwitterStatus> tweets = service.ListTweetsMentioningMe(new ListTweetsMentioningMeOptions() { ContributorDetails = true, SinceId = lastCitationSeend });
            CheckLimit(service);

            if (tweets != null)
            {
                return tweets.ToList();
            }
            else
            {
                return new List<TwitterStatus>();
            }
        }

        /// <summary>
        /// Sends the direct message.
        /// </summary>
        /// <param name="idFriend">The identifier friend.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public bool SendDirectMessage(long idFriend, string message)
        {
            TweetSharp.TwitterService service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(tokenApp, tokenSecret);

            service.SendDirectMessage(new SendDirectMessageOptions() { Recipientid = idFriend, Text = message });
            CheckLimit(service);

            if (service.Response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                LogTool.WriteLog("erreurStatusCode : " + service.Response.StatusCode);
                return false;
            }
        }

        private static void CheckLimit(TweetSharp.TwitterService service)
        {
            if (service.Response != null)
            {
                TwitterRateLimitStatus rate = service.Response.RateLimitStatus;
                if (rate != null && rate.RemainingHits == 0)
                {
                    TimeSpan timeToWait = service.Response.RateLimitStatus.ResetTime - DateTime.Now;

                    LogTool.WriteLog(string.Format("limite trouvée pour {0} attente de {1} secondes", rate.RawSource, timeToWait.TotalSeconds));
                    Thread.Sleep(timeToWait);
                }
            }
        }

        /// <summary>
        /// Tests the response ok.
        /// </summary>
        /// <param name="tweet">The tweet.</param>
        /// <param name="service">The service.</param>
        /// <returns></returns>
        private static bool TestResponseOk(TwitterStatus tweet, TweetSharp.TwitterService service)
        {
            if (service.Response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool AddFriends(TwitterMention twitterMention)
        {
            TweetSharp.TwitterService service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(tokenApp, tokenSecret);

            service.FollowUser(new FollowUserOptions() { ScreenName = twitterMention.ScreenName });
            CheckLimit(service);

            if (service.Response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                LogTool.WriteLog("erreurStatusCode : " + service.Response.StatusCode);
                return false;
            }
        }

        internal bool MentionFriend(TwitterStatus twitterStatus, string friendname)
        {
            TweetSharp.TwitterService service = new TweetSharp.TwitterService(consumerKey, consumerSecret);
            service.AuthenticateWith(tokenApp, tokenSecret);

            service.SendTweet(new SendTweetOptions() { AutoPopulateReplyMetadata = true, InReplyToStatusId = twitterStatus.Id, Status = friendname });
            CheckLimit(service);

            if (service.Response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return true;
            }
            else
            {
                LogTool.WriteLog("erreurStatusCode : " + service.Response.StatusCode);
                return false;
            }
        }
    }
}