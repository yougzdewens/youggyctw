using ConcoursTwitter.Core;
using ConcoursTwitter.Model;
using ConcoursTwitter.Tools;
using System;
using System.Data;

namespace ConcoursTwitter.Data
{
	/// <summary>
	/// 
	/// </summary>
	public class RetweetDAL
	{
		/// <summary>
		/// The database manager
		/// </summary>
		private DatabaseSQLServerManagerTools dbManager = new DatabaseSQLServerManagerTools();

		/// <summary>
		/// Purges this instance.
		/// </summary>
		internal void Purge()
		{
			dbManager.InsertOrDelete(string.Format("DELETE FROM T_Retweets WHERE DateInserted < CAST ('{0}' AS datetime)", DateTime.Now.AddDays(-90)));
		}

		/// <summary>
		/// Determines whether the specified identifier tweet is exist.
		/// </summary>
		/// <param name="idTweet">The identifier tweet.</param>
		/// <returns></returns>
		internal DataTable IsExist(long idTweet)
		{
			return dbManager.Select("SELECT IdTweet FROM T_Retweets WHERE IdTweet = " + idTweet);
		}

		/// <summary>
		/// Saves the specified retweet model.
		/// </summary>
		/// <param name="retweetModel">The retweet model.</param>
		internal void Save(RetweetModel retweetModel)
		{
			dbManager.InsertOrDelete(string.Format("Insert into T_Retweets (IdTweet, DateInserted, IdTwitterFriend) VALUES ('{0}', '{1}', '{2}')", retweetModel.IdTweet, retweetModel.DateInserted.ToString("yyyy-MM-dd HH:mm:ss.fffffff"), retweetModel.IdTwitterFriend));
		}

		/// <summary>
		/// Deletes all.
		/// </summary>
		internal void DeleteALL()
		{
			dbManager.InsertOrDelete("DELETE FROM T_Retweets");
		}
	}
}
