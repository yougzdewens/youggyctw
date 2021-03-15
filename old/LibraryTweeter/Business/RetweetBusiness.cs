using LibraryTweeter.DAL;
using LibraryTweeter.Model;
using System.Data;

namespace LibraryTweeter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class RetweetBusiness
	{
		/// <summary>
		/// The retweet dal
		/// </summary>
		RetweetDAL retweetDal = new RetweetDAL();

		/// <summary>
		/// Purges this instance.
		/// </summary>
		internal void Purge()
		{
			retweetDal.Purge();
		}

		/// <summary>
		/// Determines whether the specified identifier tweet is exist.
		/// </summary>
		/// <param name="idTweet">The identifier tweet.</param>
		/// <returns>
		///   <c>true</c> if the specified identifier tweet is exist; otherwise, <c>false</c>.
		/// </returns>
		internal bool IsExist(long idTweet)
		{
			DataTable resultIsExist = retweetDal.IsExist(idTweet);

			if (resultIsExist.Rows.Count > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Saves the specified retweet model.
		/// </summary>
		/// <param name="retweetModel">The retweet model.</param>
		internal void Save(RetweetModel retweetModel)
		{
			retweetDal.Save(retweetModel);
		}

		internal void DeleteALL()
		{
			retweetDal.DeleteALL();
		}
	}
}
