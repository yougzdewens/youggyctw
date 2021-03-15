using LibraryTweeter.Model;
using System;
using System.Data;

namespace LibraryTweeter.DAL
{
	/// <summary>
	/// 
	/// </summary>
	public class MessageDAL
	{
		/// <summary>
		/// The database manager
		/// </summary>
		private DatabaseSQLServerManager dbManager = new DatabaseSQLServerManager();

		/// <summary>
		/// Gets this instance.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		internal DataTable Get()
		{
			throw new Exception();
		}

		/// <summary>
		/// Gets the specified identifier message.
		/// </summary>
		/// <param name="idMessage">The identifier message.</param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		internal DataTable Get(int idMessage)
		{
			throw new Exception();
		}

		/// <summary>
		/// Deletes all.
		/// </summary>
		internal void DeleteAll()
		{
			dbManager.InsertOrDelete("DELETE FROM T_Messages");
		}

		/// <summary>
		/// Messages the using identifier message tweeter.
		/// </summary>
		/// <param name="idMessageTweeter">The identifier message tweeter.</param>
		/// <returns></returns>
		internal DataTable MessageUsingIdMessageTweeter(int idMessageTweeter)
		{
			string sqlSelectMessage = string.Format("SELECT * FROM T_Messages WHERE IdMessageTweeter = {0}", idMessageTweeter);
			DataTable resultMessage = dbManager.Select(sqlSelectMessage);

			return resultMessage;
		}

		/// <summary>
		/// Saves the specified message model.
		/// </summary>
		/// <param name="messageModel">The message model.</param>
		internal void Save(MessageModel messageModel)
		{
			dbManager.InsertOrDelete(string.Format("Insert into T_Messages (IdMessageTweeter, Text, IdTwitterFriend, Date, Response) VALUES ({0}, '{1}', {2}, '{3}', '{4}')", messageModel.IdMessageTweeter, messageModel.Text, messageModel.IdTwitterFriend, messageModel.Date.ToString("yyyy-MM-dd HH:mm:ss.fffffff"), messageModel.Response));
		}

		/// <summary>
		/// Determines whether [is message exist with identifier message twitter] [the specified identifier message tweeter].
		/// </summary>
		/// <param name="idMessageTweeter">The identifier message tweeter.</param>
		/// <returns></returns>
		internal DataTable IsMessageExistWithIdMessageTwitter(long idMessageTweeter)
		{
			string sqlSelectMessage = string.Format("SELECT IdMessageTweeter FROM T_Messages WHERE IdMessageTweeter = {0}", idMessageTweeter);
			DataTable resultMessage = dbManager.Select(sqlSelectMessage);

			return resultMessage;
		}

		internal DataTable GetLastMessage()
		{
			string sqlSelectMessage = "SELECT TOP 1 IdTwitterFriend FROM T_Messages order by Date desc";
			return dbManager.Select(sqlSelectMessage);
		}
	}
}
