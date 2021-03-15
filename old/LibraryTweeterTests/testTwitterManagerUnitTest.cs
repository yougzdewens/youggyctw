using System;
using LibraryTweeter.Externals;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTweeterTests
{
	[TestClass]
	public class testTwitterManagerUnitTest
	{
		TwitterManager twitterManager = new TwitterManager();
		private long idTwitterOfFriendForTest = 921285293805883392;

		[TestMethod]
		public void GetAllFriends()
		{
			var friends = twitterManager.GetAllFriends();

			Assert.IsTrue(friends.Count > 0);
		}

		[TestMethod]
		public void GetTweetFromFriend()
		{
			var tweets = twitterManager.GetTweetFromFriend(idTwitterOfFriendForTest);

			long? lastId = null;

			if (tweets.Count > 2)
			{
				lastId = tweets[1].Id;
			}

			var tweets2 = twitterManager.GetTweetFromFriend(idTwitterOfFriendForTest, lastId);

			Assert.IsTrue(tweets != null);
		}

		[TestMethod]
		public void RetweetATweet()
		{
			var tweets = twitterManager.GetTweetFromFriend(idTwitterOfFriendForTest);
			bool retweeted = twitterManager.RetweetATweet(tweets[0]);

			Assert.IsTrue(retweeted == true);
		}

		[TestMethod]
		public void FavoritedATweet()
		{
			var tweets = twitterManager.GetTweetFromFriend(idTwitterOfFriendForTest);
			bool favorited = twitterManager.LikeATweet(tweets[0]);

			Assert.IsTrue(favorited == true);
		}

		[TestMethod]
		public void DeleteDirectMessages()
		{
			var msgSent = twitterManager.GetDirectMessageSent();
			Assert.IsTrue(msgSent.Count > 0);

			var msgReceived = twitterManager.GetDirectMessageReceived(null);
			Assert.IsTrue(msgReceived.Count > 0);

			int countDeleted = twitterManager.DeleteDirectMessages();

			Assert.IsTrue(countDeleted > 0);
		}
	}
}