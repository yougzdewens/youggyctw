using ConcoursTwitter.Data;
using ConcoursTwitter.Model;
using ConcoursTwitter.Core;
using System;
using System.Data;

namespace ConcoursTwitter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class CitationBusiness
	{
		/// <summary>
		/// The citation dal
		/// </summary>
		private CitationDAL citationDAL = new CitationDAL();

		/// <summary>
		/// Determines whether [is citation exist] [the specified identifier tweet].
		/// </summary>
		/// <param name="idTweet">The identifier tweet.</param>
		/// <returns>
		///   <c>true</c> if [is citation exist] [the specified identifier tweet]; otherwise, <c>false</c>.
		/// </returns>
		internal bool IsCitationExist(long idTweet)
		{
			DataTable isExist = citationDAL.IsCitationExist(idTweet);

			if (isExist.Rows.Count > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Saves the specified citation model.
		/// </summary>
		/// <param name="citationModel">The citation model.</param>
		internal void Save(CitationModel citationModel)
		{
			citationDAL.Save(citationModel);
		}

		///// <summary>
		///// Saves the specified tweet.
		///// </summary>
		///// <param name="tweet">The tweet.</param>
		//internal void Save(TwitterStatus tweet)
		//{
		//	CitationModel citationModel = new CitationModel();
		//	citationModel.Date = DateTime.Now;
		//	citationModel.Text = SqlTool.SanitizeForSql(tweet.Text);
		//	citationModel.IdTwitterFriend = tweet.User.Id;
		//	citationModel.IdTweet = tweet.Id;

		//	this.Save(citationModel);
		//}

		internal void DeleteALL()
		{
			citationDAL.DeleteALL();
		}

		internal long? GetLastCitationSeen()
		{
			DataTable lastCitationSeen = citationDAL.GetLastCitationSeen();

			if (lastCitationSeen != null && lastCitationSeen.Rows.Count > 0)
			{
				return long.Parse(lastCitationSeen.Rows[0]["IdTweet"].ToString());
			}
			else
			{
				return null;
			}
		}
	}
}
