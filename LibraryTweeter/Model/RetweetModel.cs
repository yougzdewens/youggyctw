using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTweeter.Model
{
	public class RetweetModel
	{
		public long IdTweet { get; set; }

		public DateTime DateInserted { get; set; }

		public long IdTwitterFriend { get; set; }
	}
}
