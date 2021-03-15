using ConcoursTwitter.Model;
using System.Data;
using ConcoursTwitter.Core;
using ConcoursTwitter.Tools;

namespace ConcoursTwitter.Data
{
	/// <summary>
	/// 
	/// </summary>
	public class LastIdSeenDAL
	{
		/// <summary>
		/// The database manager
		/// </summary>
		private DatabaseSQLServerManagerTools dbManager = new DatabaseSQLServerManagerTools();

		/// <summary>
		/// Gets the specified user.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		internal DataTable Get(long idTwitterFriend)
		{
			string sqlSelectFriends = string.Format("SELECT * FROM T_LastIdSeen WHERE IdTwitterFriend = {0}", idTwitterFriend);
			return dbManager.Select(sqlSelectFriends);
		}

		/// <summary>
		/// Adds the specified last identifier seen model.
		/// </summary>
		/// <param name="lastIdSeenModel">The last identifier seen model.</param>
		internal void Add(LastIdSeenModel lastIdSeenModel)
		{
			string sqlInsert = string.Format("INSERT into T_LastIdSeen (IdOfLastTweetSeen, IdTwitterFriend) VALUES ({0}, {1})", lastIdSeenModel.IdOfLastTweetSeen, lastIdSeenModel.IdTwitterFriend);
			dbManager.InsertOrDelete(sqlInsert);
		}

		/// <summary>
		/// Updates the specified last identifier seen model.
		/// </summary>
		/// <param name="lastIdSeenModel">The last identifier seen model.</param>
		internal void Update(LastIdSeenModel lastIdSeenModel)
		{
			string sqlInsert = string.Format("Update T_LastIdSeen SET IdOfLastTweetSeen = {0} WHERE IdTwitterFriend = {1}", lastIdSeenModel.IdOfLastTweetSeen, lastIdSeenModel.IdTwitterFriend);
			dbManager.InsertOrDelete(sqlInsert);
		}

		/// <summary>
		/// Deletes all.
		/// </summary>
		internal void DeleteALL()
		{
			dbManager.InsertOrDelete("DELETE FROM T_LastIdSeen");
		}
	}
}