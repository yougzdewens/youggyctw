using ConcoursTwitter.Data;
using ConcoursTwitter.Model;
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
	public class LastIdSeenBusiness
	{
		/// <summary>
		/// The last identifier seen dal
		/// </summary>
		private LastIdSeenDAL lastIdSeenDAL = new LastIdSeenDAL();

		/// <summary>
		/// Adds the or update.
		/// </summary>
		/// <param name="lastIdSeenModel">The last identifier seen model.</param>
		internal void AddOrUpdate(LastIdSeenModel lastIdSeenModel)
		{
			DataTable lastIdSeenResult = lastIdSeenDAL.Get(lastIdSeenModel.IdTwitterFriend);

			if (lastIdSeenResult.Rows.Count > 0)
			{
				lastIdSeenDAL.Update(lastIdSeenModel);
			}
			else
			{
				lastIdSeenDAL.Add(lastIdSeenModel);
			}
		}

		/// <summary>
		/// Gets the specified user.
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns></returns>
		internal long? Get(long idTwitterFriend)
		{
			DataTable tableLastSeen = lastIdSeenDAL.Get(idTwitterFriend);

			if(tableLastSeen != null && tableLastSeen.Rows.Count > 0)
			{
				return long.Parse(tableLastSeen.Rows[0]["IdOfLastTweetSeen"].ToString());
			}
			else
			{
				return null;
			}
		}

		internal void DeleteALL()
		{
			lastIdSeenDAL.DeleteALL();
		}
	}
}