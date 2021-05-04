using ConcoursTwitter.Model;
using ConcoursTwitter.Core;
using System;
using System.Data;
using ConcoursTwitter.Tools;

namespace ConcoursTwitter.Data
{
	/// <summary>
	/// 
	/// </summary>
	public class CitationDAL
	{
		/// <summary>
		/// The database manager
		/// </summary>
		private DatabaseSQLServerManagerTools dbManager = new DatabaseSQLServerManagerTools();

		/// <summary>
		/// Determines whether [is citation exist] [the specified identifier tweet].
		/// </summary>
		/// <param name="idTweet">The identifier tweet.</param>
		/// <returns></returns>
		internal DataTable IsCitationExist(long idTweet)
		{
			string sqlSelectMessage = string.Format("SELECT IdTweet FROM T_Citations WHERE IdTweet = {0}", idTweet);
			return dbManager.Select(sqlSelectMessage);
		}

		/// <summary>
		/// Saves the specified citation model.
		/// </summary>
		/// <param name="citationModel">The citation model.</param>
		internal void Save(CitationModel citationModel)
		{
			dbManager.InsertOrDelete(string.Format("Insert into T_Citations (Text, IdTwitterFriend, Date, IdTweet) VALUES ('{0}', {1}, '{2}', {3})", SqlTools.SanitizeForSql(citationModel.Text), citationModel.IdTwitterFriend, citationModel.Date.ToString("yyyy-MM-dd HH:mm:ss.fffffff"), citationModel.IdTweet));
		}

		/// <summary>
		/// Deletes all.
		/// </summary>
		internal void DeleteALL()
		{
			dbManager.InsertOrDelete("DELETE FROM T_Citations");
		}

		/// <summary>
		/// Gets the last citation seen.
		/// </summary>
		/// <returns></returns>
		internal DataTable GetLastCitationSeen()
		{
			string sqlSelectMessage = "SELECT TOP 1 IdCitation FROM T_Citations order by Date desc";
			return dbManager.Select(sqlSelectMessage);
		}
	}
}
