using ConcoursTwitter.Business;
using ConcoursTwitter.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using YouggyTw.Model;

namespace ConcoursTwitter.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class ConcoursTwitterCoreFriends
    {
        /// <summary>
        /// The twitter
        /// </summary>
        private YouggyTw.Twitter twitter;

        /// <summary>
        /// The user list
        /// </summary>
        ConcurrentBag<User> userList = new ConcurrentBag<User>();

        private FriendsBusiness friendsBusiness = new FriendsBusiness();

        private FriendsAddedBusiness friendsAddedBusiness = new FriendsAddedBusiness();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConcoursTwitterCoreFriends"/> class.
        /// </summary>
        /// <param name="twitter">The twitter.</param>
        public ConcoursTwitterCoreFriends(YouggyTw.Twitter twitter)
        {
            this.twitter = twitter;

            Task fillUser = Task.Run(() => {

                // Create a timer and set a 10 hour
                System.Timers.Timer aTimer = new System.Timers.Timer();
                aTimer.Interval = 36000000;

                // Hook up the Elapsed event for the timer. 
                aTimer.Elapsed += EmptyAndRefillUser;

                // Have the timer fire repeated events (true is the default)
                aTimer.AutoReset = true;

                // Start the timer
                aTimer.Enabled = true;

                // take user first time
                EmptyAndRefillUser(null, null);
            });           

            // Create a timer and set a 100 hour
            System.Timers.Timer aTimerCleaningTempFriends = new System.Timers.Timer();
            aTimerCleaningTempFriends.Interval = 360000000;

            // Hook up the Elapsed event for the timer. 
            aTimerCleaningTempFriends.Elapsed += CleaningTempFriends;

            // Have the timer fire repeated events (true is the default)
            aTimerCleaningTempFriends.AutoReset = true;

            // Start the timer
            aTimerCleaningTempFriends.Enabled = true;

            Thread.Sleep(2000);
        }

        private void CleaningTempFriends(object sender, ElapsedEventArgs e)
        {
            List<Model.FriendsAddedModel> friendsAddedModel = friendsAddedBusiness.GetAll();

            foreach (Model.FriendsAddedModel friend in friendsAddedModel)
            {
                if (friend.DateAdded < DateTime.Now.AddDays(-60))
                {
                    twitter.DeleteFriend(friend.IdTwitterFriend);
                    friendsAddedBusiness.Delete(friend.IdTwitterFriend);
                }
            }
        }

        /// <summary>
        /// Gets all friends.
        /// </summary>
        /// <returns></returns>
        public List<User> GetAllFriends()
        {
            List<User> users = new List<User>();

            foreach (User user in userList)
            {
                users.Add(user);
            }

            return users;
        }

        /// <summary>
        /// Deletes all friends (only for the reset mode)
        /// </summary>
        public void DeleteAllFriends()
        {
            List<long> usersDeleted = new List<long>();

            bool userWasDeleted = true;

            while (userWasDeleted == true)
            {
                userWasDeleted = false;

                foreach (User user in userList)
                {
                    if (!usersDeleted.Contains(user.Id))
                    {
                        LogTools.WriteLog("Delete friend : " + user.Id);
                        twitter.DeleteFriend(user.Id);
                        usersDeleted.Add(user.Id);
                        userWasDeleted = true;
                    }
                }
            }
        }

        /// <summary>
        /// Adds the sub friend.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void AddSubFriend()
        {
            throw new NotImplementedException();
        }

        private List<User> GetUserAndUpdateDatabase()
        {
            List<User> users = GetUserList();

            foreach (User user in users)
            {
                if (!friendsBusiness.IsExist(user.Id) && !friendsAddedBusiness.IsExist(user.Id))
                {
                    friendsBusiness.Save(new Model.FriendsModel() { DateAdded = DateTime.Now, IdTwitterFriend = user.Id, Name = user.ScreenName });
                }                
            }

            return users;
        }

        /// <summary>
        /// Fills the user list.
        /// </summary>
        private List<User> GetUserList()
        {
            List<User> users = new List<User>();

            LogTools.WriteLog("GetFriends");
            Tuple<List<User>, long> usersAndCursor = GetFriendsAndCursor(0);

            users = usersAndCursor.Item1;

            while (usersAndCursor.Item2 > 0)
            {
                LogTools.WriteLog("GetFriends with cursor : " + usersAndCursor.Item2);
                usersAndCursor = GetFriendsAndCursor(usersAndCursor.Item2);

                users.AddRange(usersAndCursor.Item1);
            }

            return users;
        }

        private void EmptyAndRefillUser(object sender, ElapsedEventArgs e)
        {
            while (userList.Count > 0)
            {
                User user = new User();
                userList.TryTake(out user);
            }

            foreach (User user in GetUserAndUpdateDatabase())
            {
                userList.Add(user);
            }
        }

        /// <summary>
        /// Gets the friends and cursor.
        /// </summary>
        /// <param name="cursor">The cursor.</param>
        /// <returns></returns>
        private Tuple<List<User>, long> GetFriendsAndCursor(long cursor)
        {
            Tuple<List<User>, long> usersAndCursor = twitter.GetFriends(cursor);

            foreach (User user in usersAndCursor.Item1)
            {
                if (!friendsAddedBusiness.IsExist(user.Id))
                {
                    userList.Add(user);
                }
            }

            return usersAndCursor;
        }

        /// <summary>
        /// Purges the sub friend.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void PurgeSubFriend()
        {
            throw new NotImplementedException();
        }
    }
}
