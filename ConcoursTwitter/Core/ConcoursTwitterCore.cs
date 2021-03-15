using ConcoursTwitter.Business;
using ConcoursTwitter.Data;
using ConcoursTwitter.Model;
using ConcoursTwitter.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YouggyTw.Model;

namespace ConcoursTwitter.Core
{
    public class ConcoursTwitterCore
    {
        #region private field
        static YouggyTw.Twitter twitter;

        private ConcoursTwitterCoreFriends concoursTwitterCoreFriends;

        /// <summary>
        /// The message business
        /// </summary>
        private MessageBusiness messageBusiness = new MessageBusiness();

        /// <summary>
        /// The last identifier seen business
        /// </summary>
        private LastIdSeenBusiness lastIdSeenBusiness = new LastIdSeenBusiness();

        /// <summary>
        /// The citation business
        /// </summary>
        private CitationBusiness citationBusiness = new CitationBusiness();

        /// <summary>
        /// The retweet business
        /// </summary>
        private RetweetBusiness retweetBusiness = new RetweetBusiness();

        /// <summary>
        /// The syn bot manager
        /// </summary>
        //private SynBotManager synBotManager = new SynBotManager();

        /// <summary>
        /// The friends business
        /// </summary>
        private FriendsBusiness friendsBusiness = new FriendsBusiness();

        private FriendsAddedBusiness friendsAddedBusiness = new FriendsAddedBusiness();

        /// <summary>
        /// The twitter manager
        /// </summary>
        //private TwitterManager twitterManager = new TwitterManager();

        private long idUser = 0;

        //private static TwitterService instance = null;

        //private static readonly object padlock = new object();

        //public static bool Started { get; set; }

        private InfoServiceModel infoService = new InfoServiceModel();

        #endregion

        //public bool IsStopping { get; set; }

        //public bool IsStopped { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterService" /> class.
        /// </summary>
        public ConcoursTwitterCore()
        {
            twitter = new YouggyTw.Twitter(
                ConfigurationTools.ConsumerKey,
                ConfigurationTools.ConsumerSecret,
                ConfigurationTools.AccessToken,
                ConfigurationTools.AccessTokenSecret
            );

            DatabaseInit.CreateTables(ConfigurationTools.ResetMode == "true" ? true : false);

            idUser = twitter.VerifyCredentials().Id;

            concoursTwitterCoreFriends = new ConcoursTwitterCoreFriends(twitter);

            if (ConfigurationTools.ResetMode == "true")
            {
                concoursTwitterCoreFriends.DeleteAllFriends();
            }

            //twitter.DeleteFriend(new User() { Id = 187193029 });

            //twitterManager.DeleteDirectMessages();
            //this.messageBusiness.DeleteAll();
        }

        //public static InfoService GetInfoService()
        //{
        //    return TwitterService.Instance.infoService;
        //}

        //public static TwitterService Instance
        //{
        //    get
        //    {
        //        lock (padlock)
        //        {
        //            if (instance == null)
        //            {
        //                instance = new TwitterService();
        //                Started = false;
        //            }

        //            return instance;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Starts the search direct message.
        ///// </summary>
        //private void StartSearchDirectMessage()
        //{
        //    while (true)
        //    {
        //        try
        //        {
        //            SearchDirectMessage();
        //            Waiting(1);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogTools.WriteLog("erreur StartSearchDirectMessage");
        //            LogTools.WriteLog(ex.ToString());
        //            Waiting(5);
        //        }
        //    }
        //}

        ///// <summary>
        ///// Starts the search citation.
        ///// </summary>
        //private void StartSearchCitation()
        //{
        //    bool isFirstLaunch = false;

        //    while (true)
        //    {
        //        try
        //        {
        //            SearchCitation(isFirstLaunch);
        //            Waiting(5);
        //        }
        //        catch (Exception ex)
        //        {
        //            LogTools.WriteLog("erreur StartSearchCitation");
        //            LogTools.WriteLog(ex.ToString());
        //            Waiting(5);
        //        }

        //        isFirstLaunch = false;
        //    }
        //}

        public void StartAll()
        {
            Task task1 = Task.Factory.StartNew(() => { StartCycle(); });
            //Task task2 = Task.Factory.StartNew(() => { StartSearchCitation(); });
            //Task task3 = Task.Factory.StartNew(() => { StartSearchDirectMessage(); });


            task1.Wait();
            //task2.Wait();
            //task3.Wait();
            LogTools.WriteLog("Fin des tâches");

            friendsBusiness = new FriendsBusiness();

        }

        /// <summary>
        /// Starts the cycle.
        /// </summary>
        private void StartCycle()
        {
            while (true)
            {
                try
                {
                    SearchConcoursFromOurContacts();
                }
                catch (Exception ex)
                {
                    LogTools.WriteLog("erreur SearchConcoursFromOurContacts");
                    LogTools.WriteLog(ex.ToString());
                }

                retweetBusiness.Purge();
                //synBotManager.SanitizeBot();

                // WAIT 15 minutes before start another search
                LogTools.WriteLog("fin de recherche, attente de 15 minutes");
                Thread.Sleep(900000);   
            }
        }

        /// <summary>
        /// Deletes all data in database.
        /// </summary>
        public void DeleteAllDataInDB()
        {
            citationBusiness.DeleteALL();
            friendsBusiness.DeleteALL();
            friendsAddedBusiness.DeleteALL();
            lastIdSeenBusiness.DeleteALL();
            messageBusiness.DeleteAll();
            retweetBusiness.DeleteALL();
        }

        ///// <summary>
        ///// Searches the direct message.
        ///// </summary>
        //private void SearchDirectMessage()
        //{
        //    long? lastId = messageBusiness.GetLastMessage();
        //    Messages tweets = twitterManager.GetDirectMessageReceived(lastId);

        //    if (tweets != null)
        //    {
        //        foreach (Event tweet in tweets.Events.Where(x => x.MessageCreate.SenderId != idUser.ToString() && JavaTimeStampToDateTime(double.Parse(x.CreatedTimestamp)) > DateTime.Now.AddMinutes(-5)))
        //        {
        //            if (!messageBusiness.IsMessageExistWithIdMessageTwitter(long.Parse(tweet.Id)))
        //            {
        //                string responseText = synBotManager.GetResponseFromBot(long.Parse(tweet.MessageCreate.SenderId), tweet.MessageCreate.MessageData.Text);

        //                if (responseText != string.Empty)
        //                {
        //                    bool isSent = twitterManager.SendDirectMessage(long.Parse(tweet.MessageCreate.SenderId), responseText);

        //                    if (isSent)
        //                    {
        //                        MessageModel message = new MessageModel();
        //                        message.IdMessageTweeter = long.Parse(tweet.Id);
        //                        message.Text = SqlTool.SanitizeForSql(tweet.MessageCreate.MessageData.Text);
        //                        message.IdTwitterFriend = long.Parse(tweet.MessageCreate.SenderId);
        //                        message.Date = DateTime.Now;
        //                        message.Response = SqlTool.SanitizeForSql(responseText);

        //                        messageBusiness.Save(message);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        public DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {
            // Java timestamp is milliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        ///// <summary>
        ///// Searches the citation.
        ///// </summary>
        ///// <param name="isFirstLaunch">if set to <c>true</c> [is first launch].</param>
        //private void SearchCitation(bool isFirstLaunch = false)
        //{
        //    List<TwitterStatus> tweets = twitterManager.GetTweetsMentionningMe();

        //    foreach (TwitterStatus tweet in tweets)
        //    {
        //        List<List<string>> listOfListWords = new List<List<string>>();
        //        listOfListWords.Add(new List<string>() { " dm", "remport" });
        //        listOfListWords.Add(new List<string>() { " mp", "remport" });
        //        listOfListWords.Add(new List<string>() { " mp", "gagn" });
        //        listOfListWords.Add(new List<string>() { "message", "priv", "gagn" });

        //        string contentToTest = tweet.FullText != null ? tweet.FullText : tweet.Text;

        //        if (!citationBusiness.IsCitationExist(tweet.Id))
        //        {
        //            if (!isFirstLaunch && StringTool.TestIfContainsWord(contentToTest, listOfListWords, true))
        //            {
        //                bool isSent = twitterManager.SendDirectMessage(tweets.First().User.Id, "Salut, visiblement j'ai peut être gagné, tu voulais me voir en MP ? :)");

        //                if (isSent)
        //                {
        //                    synBotManager.SendMessageToTheChat(tweet.User.Id, contentToTest);
        //                }
        //            }

        //            citationBusiness.Save(tweet);
        //        }
        //    }
        //}

        /// <summary>
        /// Searches the concours from our contacts.
        /// </summary>
        private void SearchConcoursFromOurContacts()
        {
            infoService.DateLastCheck = DateTime.Now;

            List<FriendsModel> allFriends = friendsBusiness.GetAll();

            string searchAndFollow = ConfigurationTools.SearchAndFollow;
            List<List<string>> listWordsToFindRTFollow = searchAndFollow.Split(';').Select(x => x.Split(',').ToList()).ToList();

            LogTools.WriteLog("Utilisateurs trouvés au final : " + allFriends.Count);
            LogTools.WriteLog("Récupération des tweets");

            infoService.NbFollowed = allFriends.Count;
            infoService.NBLastRetweet = 0;

            foreach (FriendsModel friend in allFriends)
            {
                LogTools.WriteLog("Récupération des tweets pour " + friend.Name);

                List<Tweet> tweetsFromFriend = twitter.GetUserTimeline(friend.IdTwitterFriend.ToString(), lastIdSeenBusiness.Get(friend.IdTwitterFriend).ToString());

                if (tweetsFromFriend != null && tweetsFromFriend.Count > 0)
                {
                    LogTools.WriteLog(string.Format("{0} Tweets trouvé pour {1}", tweetsFromFriend.Count, friend.Name));

                    LastIdSeenModel lastIdSeenModel = new LastIdSeenModel();
                    lastIdSeenModel.IdOfLastTweetSeen = tweetsFromFriend[0].Id;
                    lastIdSeenModel.IdTwitterFriend = friend.IdTwitterFriend;

                    lastIdSeenBusiness.AddOrUpdate(lastIdSeenModel);

                    foreach (Tweet tweetFromFriend in tweetsFromFriend)
                    {
                        string contentToTest = tweetFromFriend.FullText != null ? tweetFromFriend.FullText : tweetFromFriend.Text;

                        LogTools.WriteLog(contentToTest);

                        if (StringTools.TestIfContainsWord(contentToTest, listWordsToFindRTFollow, true))
                        {
                            if (!retweetBusiness.IsExist(tweetFromFriend.Id))
                            {
                                bool retweetOk = twitter.RetweetATweet(tweetFromFriend);

                                infoService.NBLastRetweet++;

                                LogTools.WriteLog(string.Format("Tweet n°{0} de {1} retweeté", tweetFromFriend.Id, tweetFromFriend.User.ScreenName));

                                if (StringTools.TestIfContainsWord(contentToTest, new List<string>() { "like", "aime" }))
                                {
                                    twitter.LikeATweet(tweetFromFriend);
                                    LogTools.WriteLog(string.Format("Tweet n°{0} de {1} liké", tweetFromFriend.Id, tweetFromFriend.User.ScreenName));
                                }

                                if (StringTools.TestIfContainsWord(contentToTest, new List<string>() { "tag" }) || StringTools.TestIfContainsWord(contentToTest, new List<string>() { "mention" }))
                                {
                                    string status = "@YouggyG";
                                    LogTools.WriteLog(string.Format("Tweet n°{0} de {1} mentionné", tweetFromFriend.Id, tweetFromFriend.User.ScreenName));

                                    if (StringTools.TestIfContainsWord(contentToTest, new List<string>() { "deux" }) || StringTools.TestIfContainsWord(contentToTest, new List<string>() { "2" }))
                                    {
                                        status = "@YouggyG @yougzPpbqa";
                                    }

                                    // Reply with mention friends
                                    twitter.Reply(tweetFromFriend, status);
                                }

                                foreach (UserMention twitterMention in tweetFromFriend.Entities.UserMentions)
                                {
                                    if (twitterMention.ScreenName != tweetFromFriend.User.ScreenName)
                                    {
                                        twitter.AddFriends(twitterMention);
                                        FriendsModel friendModel = new FriendsModel();
                                        friendModel.DateAdded = DateTime.Now;
                                        friendModel.IdTwitterFriend = twitterMention.Id;
                                        friendModel.Name = twitterMention.ScreenName;

                                        friendsAddedBusiness.Save(friendModel);
                                    }
                                }

                                RetweetModel retweetModel = new RetweetModel();
                                retweetModel.IdTweet = tweetFromFriend.Id;
                                retweetModel.IdTwitterFriend = friend.IdTwitterFriend;
                                retweetModel.DateInserted = DateTime.Now;

                                retweetBusiness.Save(retweetModel);
                            }
                        }
                    }
                }
                else
                {
                    LogTools.WriteLog("Aucun tweet trouvé pour " + friend.Name);
                }
            }
        }
    }
}
