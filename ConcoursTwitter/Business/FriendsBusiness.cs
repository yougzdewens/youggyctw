using ConcoursTwitter.Core;
using ConcoursTwitter.Data;
using ConcoursTwitter.Model;
using ConcoursTwitter.Tools;
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
	public class FriendsBusiness
	{
		/// <summary>
		/// The nb hour before research from twitter
		/// </summary>
		private const int nbHourBeforeResearchFromTwitter = 24;

		/// <summary>
		/// The friends dal
		/// </summary>
		private FriendsDAL friendsDal = new FriendsDAL();

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
			DataTable isExistResult = friendsDal.IsExist(idTwitterUser);

			if (isExistResult.Rows.Count > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Saves the specified friends model.
		/// </summary>
		/// <param name="friendsModel">The friends model.</param>
		internal void Save(FriendsModel friendsModel)
		{
			friendsDal.Save(friendsModel);
		}

		/// <summary>
		/// Deletes the specified identifier friend.
		/// </summary>
		/// <param name="idFriend">The identifier friend.</param>
		internal void Delete(int idFriend)
		{
			friendsDal.Delete(idFriend);
		}

		internal void Delete(long idTwitterUser)
		{
			friendsDal.Delete(idTwitterUser);
		}

		/// <summary>
		/// Gets all.
		/// </summary>
		/// <returns></returns>
		internal List<FriendsModel> GetAll(List<YouggyTw.Model.User> twitterFriends)
		{
			List<FriendsModel> allFriends = new List<FriendsModel>();
			List<FriendsModel> allFriendsAdded = new List<FriendsModel>();

			LogTools.WriteLog("Récupération des utilisateurs dans la table");

			DataTable allFriendsInTable = friendsDal.GetAll();

			LogTools.WriteLog("Fin de récupération des utilisateurs dans la table " + allFriendsInTable.Rows.Count + " trouvé");

			foreach (DataRow friendInRow in allFriendsInTable.Rows)
			{
				FriendsModel friend = new FriendsModel();
				friend.IdFriends = int.Parse(friendInRow["IdFriend"].ToString());
				friend.IdTwitterFriend = long.Parse(friendInRow["IdTwitterFriend"].ToString());
				friend.DateAdded = DateTime.Parse(friendInRow["DateAdded"].ToString());
				friend.Name = SqlTools.SanitizeForSql(friendInRow["Name"].ToString());

				allFriends.Add(friend);

				LogTools.WriteLog(friend.Name + " trouvé");
			}

			DataTable allFriendsAddedInTable = friendsAddedDal.GetAll();

			foreach (DataRow friendInRow in allFriendsAddedInTable.Rows)
			{
				FriendsModel friend = new FriendsModel();
				friend.IdFriends = int.Parse(friendInRow["IdFriendsAdded"].ToString());
				friend.IdTwitterFriend = long.Parse(friendInRow["IdTwitterFriend"].ToString());
				friend.DateAdded = DateTime.Parse(friendInRow["DateAdded"].ToString());

				allFriendsAdded.Add(friend);
			}

			if (lastDateFromTwitter != null)
			{
				LogTools.WriteLog("lastDateFromTwitter null");
			}
			else
			{
				LogTools.WriteLog(lastDateFromTwitter.ToString());
			}

			if (lastDateFromTwitter == null || lastDateFromTwitter.AddHours(nbHourBeforeResearchFromTwitter) < DateTime.Now)
			{
				LogTools.WriteLog("Récupération des utilisateurs sur twitter");

				UpdateFriendsFromTwitter(allFriends, allFriendsAdded, twitterFriends);

				if (lastDateFromTwitter == null)
				{
					lastDateFromTwitter = DateTime.Now;
				}
				else
				{
					lastDateFromTwitter = DateTime.Now;
					return this.GetAll(twitterFriends);
				}

				LogTools.WriteLog("Fin de Récupération des utilisateurs sur twitter");
			}

			LogTools.WriteLog("Fin de Récupération des utilisateurs");

			return allFriends;
		}

		/// <summary>
		/// Updates the friends from twitter.
		/// </summary>
		/// <param name="allFriends">All friends.</param>
		private void UpdateFriendsFromTwitter(List<FriendsModel> allFriends, List<FriendsModel> allFriendsAdded, List<YouggyTw.Model.User> twitterFriends)
		{
            LogTools.WriteLog("Fin de Get utilisateurs sur twitter ");
            LogTools.WriteLog(twitterFriends.Count + " trouvé sur twitter");

            foreach (YouggyTw.Model.User user in twitterFriends)
            {
                if (!allFriends.Select(f => f.IdTwitterFriend).Contains(user.Id) && !allFriendsAdded.Select(f => f.IdTwitterFriend).Contains(user.Id))
                {
                    LogTools.WriteLog(user.Name + " trouvé sur twitter");

                    FriendsModel friendsToAdd = new FriendsModel();
                    friendsToAdd.IdTwitterFriend = user.Id;
                    friendsToAdd.DateAdded = DateTime.Now;
                    friendsToAdd.Name = user.Name;

                    this.Save(friendsToAdd);
                }
            }

            List<FriendsModel> allFriendsDeleted = allFriends.Where(f => !twitterFriends.Select(tf => tf.Id).Contains(f.IdTwitterFriend)).ToList();
            allFriendsDeleted = allFriendsDeleted.Where(f => !allFriendsAdded.Select(tf => tf.IdTwitterFriend).Contains(f.IdTwitterFriend)).ToList();

            foreach (FriendsModel friend in allFriendsDeleted)
            {
                this.Delete(friend.IdFriends);
            }
        }

		internal void DeleteALL()
		{
			friendsDal.DeleteALL();
		}
	}
}
