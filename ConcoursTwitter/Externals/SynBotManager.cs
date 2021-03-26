using ConcoursTwitter.Tools;
using Syn.Bot.Siml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace YouggyCtw.Externals
{
    public class SynBotManager
    {
		#region private field
		/// <summary>
		/// The bot discussions
		/// </summary>
		private static List<Model.BotDiscussion> botDiscussions = new List<Model.BotDiscussion>();

		/// <summary>
		/// The path of siml package
		/// </summary>
		private string pathOfSimlPackage = ConfigurationTools.PathOfSimlPackage;
		#endregion

		/// <summary>
		/// Sanitizes the bot.
		/// </summary>
		public void SanitizeBot()
		{
			botDiscussions = botDiscussions.Where(x => x.DateCreation > DateTime.Now.AddHours(-24)).ToList();
		}

		/// <summary>
		/// Creates the bot discussion.
		/// </summary>
		/// <param name="package">The package.</param>
		/// <param name="idFriend">The identifier friend.</param>
		/// <returns></returns>
		public Model.BotDiscussion CreateBotDiscussion(string package, long idFriend)
		{
			SimlBot simlBot = new SimlBot();

			BotUser botUser = simlBot.CreateUser();

			var packageString = File.ReadAllText(package);
			simlBot.PackageManager.LoadFromString(packageString);

			return new Model.BotDiscussion(simlBot, botUser, idFriend);
		}

		/// <summary>
		/// Gets the response from bot.
		/// </summary>
		/// <param name="idFriend">The identifier friend.</param>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public string GetResponseFromBot(long idFriend, string text)
		{
			Model.BotDiscussion botDiscussion = null;

			if (botDiscussions.Select(x => x.IdUserOfDiscution).Contains(idFriend))
			{
				botDiscussion = botDiscussions.Where(x => x.IdUserOfDiscution == idFriend).First();
			}
			else
			{
				botDiscussion = CreateBotDiscussion(pathOfSimlPackage, idFriend);
				botDiscussions.Add(botDiscussion);
			}

			ChatRequest chatRequest = new ChatRequest(text, botDiscussion.MyBotUser);
			ChatResult chatResult = botDiscussion.MySimlBot.Chat(chatRequest);

			if (chatResult.BotMessage.Trim() != string.Empty)
			{
				return chatResult.BotMessage;
			}
			else
			{
				return string.Empty;
			}
		}

		/// <summary>
		/// Sends the message to the chat.
		/// </summary>
		/// <param name="idUser">The identifier user.</param>
		/// <param name="text">The text.</param>
		public void SendMessageToTheChat(long idUser, string text)
		{
			Model.BotDiscussion botDiscussion = null;

			if (!botDiscussions.Select(x => x.IdUserOfDiscution).Contains(idUser))
			{
				botDiscussion = CreateBotDiscussion(pathOfSimlPackage, idUser);
				botDiscussion.MySimlBot.Settings["AlreadySayHello"].Value = "true";
				botDiscussions.Add(botDiscussion);
			}
			else
			{
				botDiscussion = botDiscussions.Where(x => x.IdUserOfDiscution == idUser).First();
			}

			ChatRequest chatRequest = new ChatRequest(text, botDiscussion.MyBotUser);
			ChatResult chatResult = botDiscussion.MySimlBot.Chat(chatRequest);
		}
	}
}
