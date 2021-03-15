using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTweeter.Model
{
	public class CitationModel
	{
		public int IdCitation { get; set; }

		public string Text { get; set; }

		public long IdTwitterFriend { get; set; }

		public DateTime Date { get; set; }

		public long IdTweet { get; set; }
	}
}