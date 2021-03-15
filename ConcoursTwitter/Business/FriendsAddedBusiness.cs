using ConcoursTwitter.Data;
using ConcoursTwitter.Model;
using ConcoursTwitter.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcoursTwitter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class FriendsAddedBusiness
	{
		/// <summary>
		/// The nb hour before research from twitter
		/// </summary>
		private const int nbHourBeforeResearchFromTwitter = 24;

		/// <summary>
		/// The friends dal
		/// </summary>
		private FriendsAddedDAL friendsAddedDal = new FriendsAddedDAL();

		/// <summary>
		/// The last date from twitter
		/// </summary>
		private DateTime lastDateFromTwitter;

		/// <summary>
		/// The twitter manager
		/// </summary>
		//private TwitterManager twitterManager = new TwitterManager();

		/// <summary>
		/// Determines whether the specified user is exist.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>
		///   <c>true</c> if the specified user is exist; otherwise, <c>false</c>.
		/// </returns>
		internal bool IsExist(long idTwitterUser)
		{
			//DataTable isExistResult = friendsAddedDal.IsExist(idTwitterUser);

			//if (isExistResult.Rows.Count > 0)
			//{
			//	return true;
			//}
			//else
			//{
			//	return false;
			//}
			return false;
		}

		/// <summary>
		/// Saves the specified friends model.
		/// </summary>
		/// <param name="friendsModel">The friends model.</param>
		internal void Save(FriendsModel friendsModel)
		{
			friendsAddedDal.Save(friendsModel);
		}

		/// <summary>
		/// Deletes the specified identifier friend.
		/// </summary>
		/// <param name="idFriend">The identifier friend.</param>
		internal void Delete(int idFriend)
		{
			friendsAddedDal.Delete(idFriend);
		}

		internal void Delete(long idTwitterUser)
		{
			friendsAddedDal.Delete(idTwitterUser);
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns></returns>
		internal List<FriendsAddedModel> GetAll()
		{
			List<FriendsAddedModel> allFriends = new List<FriendsAddedModel>();

			DataTable allFriendsInTable = friendsAddedDal.GetAll();

			foreach (DataRow friendInRow in allFriendsInTable.Rows)
			{
                FriendsAddedModel friend = new FriendsAddedModel();
				friend.IdFriendsAdded = int.Parse(friendInRow["IdFriendsAdded"].ToString());
				friend.IdTwitterFriend = long.Parse(friendInRow["IdTwitterFriend"].ToString());
				friend.DateAdded = DateTime.Parse(friendInRow["DateAdded"].ToString());

				allFriends.Add(friend);
			}	
            
			return allFriends;
		}

		internal void DeleteALL()
		{
			friendsAddedDal.DeleteALL();
		}
	}
}
