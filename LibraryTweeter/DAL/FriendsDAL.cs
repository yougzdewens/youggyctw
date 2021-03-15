using LibraryTweeter.Model;
using LibraryTweeter.Tools;
using System;
using System.Data;

namespace LibraryTweeter.DAL
{
	public class FriendsDAL
	{
		private DatabaseSQLServerManager dbManager = new DatabaseSQLServerManager();

		internal DataTable IsExist(long idTwitterFriend)
		{
			string sqlSelectFriends = string.Format("SELECT IdTwitterFriend FROM T_Friends WHERE IdTwitterFriend = {0}", idTwitterFriend);
			return dbManager.Select(sqlSelectFriends);
		}

		internal void Save(FriendsModel friendsModel)
		{

			string sqlInsert = string.Format("Insert into T_Friends (IdTwitterFriend, DateAdded, Name) VALUES ({0}, '{1}', '{2}')", friendsModel.IdTwitterFriend, friendsModel.DateAdded.ToString("yyyy-MM-dd HH:mm:ss.fffffff"), SqlTool.SanitizeForSql(friendsModel.Name));

			dbManager.InsertOrDelete(sqlInsert);
		}

		internal DataTable GetAll()
		{
			return dbManager.Select("SELECT * FROM T_Friends");
		}

		internal void Delete(int idFriend)
		{
			dbManager.Select(string.Format("DELETE FROM T_Friends WHERE idFriend = {0}", idFriend));
		}

		internal void Delete(long idTwitterFriend)
		{
			dbManager.Select(string.Format("DELETE FROM T_Friends WHERE IdTwitterFriend = {0}", idTwitterFriend));
		}

		internal void DeleteALL()
		{
			dbManager.InsertOrDelete("DELETE FROM T_Friends");
		}
	}
}
