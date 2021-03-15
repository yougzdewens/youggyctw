using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTweeter.Model
{
	public class MessageModel
	{
		public int IdMessage { get; set; }

		public long IdMessageTweeter { get; set; }

		public string Text { get; set; }

		public long IdTwitterFriend { get; set; }

		public DateTime Date { get; set; }

		public string Response { get; set; }
	}
}
