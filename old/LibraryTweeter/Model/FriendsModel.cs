using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTweeter.Model
{
	public class FriendsModel
	{
		public int IdFriends { get; set; }

		public DateTime DateAdded { get; set; }

		public long IdTwitterFriend { get; set; }

		public string Name { get; set; }
	}
}
