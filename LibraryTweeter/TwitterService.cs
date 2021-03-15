using LibraryTweeter.Business;
using LibraryTweeter.Externals;
using LibraryTweeter.Model;
using LibraryTweeter.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TweetSharp;

namespace LibraryTweeter
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TwitterService
    {
        #region private field
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
        private SynBotManager synBotManager = new SynBotManager();

        /// <summary>
        /// The friends business
        /// </summary>
        private FriendsBusiness friendsBusiness = new FriendsBusiness();

        /// <summary>
        /// the friends added business
        /// </summary>
        private FriendsAddedBusiness friendsAddedBusiness = new FriendsAddedBusiness();

        /// <summary>
        /// The twitter manager
        /// </summary>
        private TwitterManager twitterManager = new TwitterManager();

        private long idUser = 0;

        private static TwitterService instance = null;

        private static readonly object padlock = new object();

        public static bool Started { get; set; }

        private InfoService infoService = new InfoService();

        #endregion

        public bool IsStopping { get; set; }

        public bool IsStopped { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterService" /> class.
        /// </summary>
        private TwitterService()
        {
            idUser = twitterManager.GetMyId();
            twitterManager.DeleteDirectMessages();
            this.messageBusiness.DeleteAll();
        }

        public static InfoService GetInfoService()
        {
            return TwitterService.Instance.infoService;
        }

        public static TwitterService Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new TwitterService();
                        Started = false;
                    }

                    return instance;
                }
            }
        }

        /// <summary>
        /// Starts the search direct message.
        /// </summary>
        private void StartSearchDirectMessage()
        {
            while (!IsStopping)
            {
                try
                {
                    SearchDirectMessage();
                    Waiting(1);
                }
                catch (Exception ex)
                {
                    LogTool.WriteLog("erreur StartSearchDirectMessage");
                    LogTool.WriteLog(ex.ToString());
                    Waiting(5);
                }
            }
        }

        private void Waiting(int nbMinutes)
        {
            for (int i = 0; i < nbMinutes; i++)
            {
                for (int j = 0; j < 60; j++)
                {
                    Thread.Sleep(new TimeSpan(0, 0, 1));

                    if (IsStopping)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Starts the search citation.
        /// </summary>
        private void StartSearchCitation()
        {
            bool isFirstLaunch = false;

            while (!IsStopping)
            {
                try
                {
                    SearchCitation(isFirstLaunch);
                    Waiting(5);
                }
                catch (Exception ex)
                {
                    LogTool.WriteLog("erreur StartSearchCitation");
                    LogTool.WriteLog(ex.ToString());
                    Waiting(5);
                }

                isFirstLaunch = false;
            }
        }

        public void StartAll()
        {           
            bool wasstarted = TestStart();
            IsStopped = false;

            if (!wasstarted)
            {
                IsStopping = false;
                Task task1 = Task.Factory.StartNew(() => { StartCycle(); });
                Task task2 = Task.Factory.StartNew(() => { StartSearchCitation(); });
                Task task3 = Task.Factory.StartNew(() => { StartSearchDirectMessage(); });

                Task taskWait = Task.Factory.StartNew(() =>
                {
                    task1.Wait();
                    task2.Wait();
                    task3.Wait();
                    LogTool.WriteLog("Fin des tâches");
                    IsStopping = false;
                    IsStopped = true;

                    lock (padlock)
                    {
                        Started = false;
                    }

                    friendsBusiness = new FriendsBusiness();
                });
            }
            else
            {
                LogTool.WriteLog("Déjà demarré");
            }

        }

        /// <summary>
        /// Starts the cycle.
        /// </summary>
        private void StartCycle()
        {
            while (!IsStopping)
            {
                try
                {
                    SearchConcoursFromOurContacts();
                }
                catch (Exception ex)
                {
                    LogTool.WriteLog("erreur SearchConcoursFromOurContacts");
                    LogTool.WriteLog(ex.ToString());
                    Waiting(5);
                }

                retweetBusiness.Purge();
                synBotManager.SanitizeBot();
                Waiting(20);
            }
        }

        private static bool TestStart()
        {
            bool wasstarted = false;

            lock (padlock)
            {
                if (Started == true)
                {
                    wasstarted = true;
                }
                else
                {
                    Started = true;
                    wasstarted = false;
                }
            }

            return wasstarted;
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

        /// <summary>
        /// Searches the direct message.
        /// </summary>
        private void SearchDirectMessage()
        {
            long? lastId = messageBusiness.GetLastMessage();
            Messages tweets = twitterManager.GetDirectMessageReceived(lastId);

            if (tweets != null)
            {
                foreach (Event tweet in tweets.Events.Where(x => x.MessageCreate.SenderId != idUser.ToString() && JavaTimeStampToDateTime(double.Parse(x.CreatedTimestamp)) > DateTime.Now.AddMinutes(-5)))
                {
                    if (!messageBusiness.IsMessageExistWithIdMessageTwitter(long.Parse(tweet.Id)))
                    {
                        string responseText = synBotManager.GetResponseFromBot(long.Parse(tweet.MessageCreate.SenderId), tweet.MessageCreate.MessageData.Text);

                        if (responseText != string.Empty)
                        {
                            bool isSent = twitterManager.SendDirectMessage(long.Parse(tweet.MessageCreate.SenderId), responseText);

                            if (isSent)
                            {
                                MessageModel message = new MessageModel();
                                message.IdMessageTweeter = long.Parse(tweet.Id);
                                message.Text = SqlTool.SanitizeForSql(tweet.MessageCreate.MessageData.Text);
                                message.IdTwitterFriend = long.Parse(tweet.MessageCreate.SenderId);
                                message.Date = DateTime.Now;
                                message.Response = SqlTool.SanitizeForSql(responseText);

                                messageBusiness.Save(message);
                            }
                        }
                    }
                }
            }
        }

        public DateTime JavaTimeStampToDateTime(double javaTimeStamp)
        {
            // Java timestamp is milliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(javaTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        /// <summary>
        /// Searches the citation.
        /// </summary>
        /// <param name="isFirstLaunch">if set to <c>true</c> [is first launch].</param>
        private void SearchCitation(bool isFirstLaunch = false)
        {
            List<TwitterStatus> tweets = twitterManager.GetTweetsMentionningMe();

            foreach (TwitterStatus tweet in tweets)
            {
                List<List<string>> listOfListWords = new List<List<string>>();
                listOfListWords.Add(new List<string>() { " dm", "remport" });
                listOfListWords.Add(new List<string>() { " mp", "remport" });
                listOfListWords.Add(new List<string>() { " mp", "gagn" });
                listOfListWords.Add(new List<string>() { "message", "priv", "gagn" });

                string contentToTest = tweet.FullText != null ? tweet.FullText : tweet.Text;

                if (!citationBusiness.IsCitationExist(tweet.Id))
                {
                    if (!isFirstLaunch && StringTool.TestIfContainsWord(contentToTest, listOfListWords, true))
                    {
                        bool isSent = twitterManager.SendDirectMessage(tweets.First().User.Id, "Salut, visiblement j'ai peut être gagné, tu voulais me voir en MP ? :)");

                        if (isSent)
                        {
                            synBotManager.SendMessageToTheChat(tweet.User.Id, contentToTest);
                        }
                    }

                    citationBusiness.Save(tweet);
                }
            }
        }

        /// <summary>
        /// Searches the concours from our contacts.
        /// </summary>
        private void SearchConcoursFromOurContacts()
        {
            infoService.DateLastCheck = DateTime.Now;

            List<FriendsModel> allFriends = friendsBusiness.GetAll();

            string searchAndFollow = System.Configuration.ConfigurationManager.AppSettings["searchAndFollow"];
            List<List<string>> listWordsToFindRTFollow = searchAndFollow.Split(';').Select(x => x.Split(',').ToList()).ToList();

            LogTool.WriteLog("Utilisateurs trouvés au final : " + allFriends.Count);

            LogTool.WriteLog("Récupération des tweets");

            infoService.NbFollowed = allFriends.Count;
            infoService.NBLastRetweet = 0;

            foreach (FriendsModel friend in allFriends)
            {
                LogTool.WriteLog("Récupération des tweets pour " + friend.Name);

                List<TwitterStatus> tweetsFromFriend = twitterManager.GetTweetFromFriend(friend.IdTwitterFriend, lastIdSeenBusiness.Get(friend.IdTwitterFriend));

                if (tweetsFromFriend != null && tweetsFromFriend.Count > 0)
                {
                    LogTool.WriteLog(string.Format("{0} Tweets trouvé pour {1}", tweetsFromFriend.Count, friend.Name));

                    LastIdSeenModel lastIdSeenModel = new LastIdSeenModel();
                    lastIdSeenModel.IdOfLastTweetSeen = tweetsFromFriend[0].Id;
                    lastIdSeenModel.IdTwitterFriend = friend.IdTwitterFriend;

                    lastIdSeenBusiness.AddOrUpdate(lastIdSeenModel);

                    foreach (TwitterStatus twitterStatus in tweetsFromFriend)
                    {
                        string contentToTest = twitterStatus.FullText != null ? twitterStatus.FullText : twitterStatus.Text;

                        LogTool.WriteLog(contentToTest);

                        if (StringTool.TestIfContainsWord(contentToTest, listWordsToFindRTFollow, true))
                        {
                            if (!retweetBusiness.IsExist(twitterStatus.Id))
                            {
                                bool retweetOk = twitterManager.RetweetATweet(twitterStatus);

                                infoService.NBLastRetweet++;

                                LogTool.WriteLog(string.Format("Tweet n°{0} de {1} retweeté", twitterStatus.Id, twitterStatus.User.ScreenName));

                                if (StringTool.TestIfContainsWord(contentToTest, new List<string>() { "like", "aime" }))
                                {
                                    twitterManager.LikeATweet(twitterStatus);
                                    LogTool.WriteLog(string.Format("Tweet n°{0} de {1} liké", twitterStatus.Id, twitterStatus.User.ScreenName));
                                }

                                if (StringTool.TestIfContainsWord(contentToTest, new List<string>() { "tag" }) || StringTool.TestIfContainsWord(contentToTest, new List<string>() { "mention" }))
                                {
                                    string status = "@YouggyG";
                                    LogTool.WriteLog(string.Format("Tweet n°{0} de {1} mentionné", twitterStatus.Id, twitterStatus.User.ScreenName));

                                    if (StringTool.TestIfContainsWord(contentToTest, new List<string>() { "deux" }) || StringTool.TestIfContainsWord(contentToTest, new List<string>() { "2" }))
                                    {
                                        status = "@YouggyG @yougzPpbqa";
                                    }

                                    twitterManager.MentionFriend(twitterStatus, status);
                                }

                                foreach (TwitterMention twitterMention in twitterStatus.Entities.Mentions)
                                {
                                    if (twitterMention.ScreenName != twitterStatus.Author.ScreenName)
                                    {
                                        twitterManager.AddFriends(twitterMention);
                                        FriendsModel friendModel = new FriendsModel();
                                        friendModel.DateAdded = DateTime.Now;
                                        friendModel.IdTwitterFriend = twitterMention.Id;
                                        friendModel.Name = twitterMention.ScreenName;

                                        friendsAddedBusiness.Save(friendModel);
                                    }
                                }

                                RetweetModel retweetModel = new RetweetModel();
                                retweetModel.IdTweet = twitterStatus.Id;
                                retweetModel.IdTwitterFriend = friend.IdTwitterFriend;
                                retweetModel.DateInserted = DateTime.Now;

                                retweetBusiness.Save(retweetModel);
                            }
                        }
                    }
                }
                else
                {
                    LogTool.WriteLog("Aucun tweet trouvé pour " + friend.Name);
                }
            }
        }
    }
}