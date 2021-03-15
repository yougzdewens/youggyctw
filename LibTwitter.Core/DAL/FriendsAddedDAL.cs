using LibraryTweeter.Model;
using System;
using System.Data;

namespace LibraryTweeter.DAL
{
	public class FriendsAddedDAL
	{
		private DatabaseSQLServerManager dbManager = new DatabaseSQLServerManager();

		internal DataTable IsExist(long idTwitterFriend)
		{
			string sqlSelectFriends = string.Format("SELECT IdTwitterFriend FROM T_FriendsAdded WHERE IdTwitterFriend = {0}", idTwitterFriend);
			return dbManager.Select(sqlSelectFriends);
		}

		internal void Save(FriendsModel friendsModel)
		{
			string sqlInsert = string.Format("Insert into T_FriendsAdded (IdTwitterFriend, DateAdded) VALUES ({0}, '{1}')", friendsModel.IdTwitterFriend, friendsModel.DateAdded.ToString("yyyy-MM-dd HH:mm:ss.fffffff"));

			dbManager.InsertOrDelete(sqlInsert);
		}

		internal DataTable GetAll()
		{
			return dbManager.Select("SELECT * FROM T_FriendsAdded");
		}

		internal void Delete(int idFriend)
		{
			dbManager.Select(string.Format("DELETE FROM T_FriendsAdded WHERE IdFriendsAdded = {0}", idFriend));
		}

		internal void Delete(long idTwitterFriend)
		{
			dbManager.Select(string.Format("DELETE FROM T_FriendsAdded WHERE IdTwitterFriend = {0}", idTwitterFriend));
		}

		internal void DeleteALL()
		{
			dbManager.InsertOrDelete("DELETE FROM T_FriendsAdded");
		}
	}
}
