using Syn.Bot.Siml;
using System;
using System.Collections.Generic;
using System.Text;

namespace YouggyCtw.Model
{
    public class BotDiscussion
    {
		/// <summary>
		/// Gets or sets my siml bot.
		/// </summary>
		/// <value>
		/// My siml bot.
		/// </value>
		public SimlBot MySimlBot { get; set; }

		/// <summary>
		/// Gets or sets my bot user.
		/// </summary>
		/// <value>
		/// My bot user.
		/// </value>
		public BotUser MyBotUser { get; set; }

		/// <summary>
		/// Gets or sets the date creation.
		/// </summary>
		/// <value>
		/// The date creation.
		/// </value>
		public DateTime DateCreation { get; set; }

		/// <summary>
		/// Gets or sets the user of discution.
		/// </summary>
		/// <value>
		/// The user of discution.
		/// </value>
		public long IdUserOfDiscution { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BotDiscussion"/> class.
		/// </summary>
		/// <param name="bot">The bot.</param>
		/// <param name="botUser">The bot user.</param>
		/// <param name="userOfDiscution">The user of discution.</param>
		public BotDiscussion(SimlBot bot, BotUser botUser, long idUserOfDiscution)
		{
			this.MySimlBot = bot;
			this.IdUserOfDiscution = idUserOfDiscution;
			this.DateCreation = DateTime.Now;
			this.MyBotUser = botUser;
		}
	}
}
