using ConcoursTwitter.Data;
using ConcoursTwitter.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace ConcoursTwitter.Business
{
	/// <summary>
	/// 
	/// </summary>
	public class MessageBusiness
	{
		/// <summary>
		/// The message dal
		/// </summary>
		private MessageDAL messageDal = new MessageDAL();
		
		/// <summary>
		/// Gets the message using identifier message tweeter.
		/// </summary>
		/// <param name="idMessageTweeter">The identifier message tweeter.</param>
		/// <returns></returns>
		internal List<MessageModel> GetMessageUsingIdMessageTweeter(int idMessageTweeter)
		{
			DataTable messages = messageDal.MessageUsingIdMessageTweeter(idMessageTweeter);
			List<MessageModel> messagesModel = new List<MessageModel>();

			foreach (DataRow message in messages.Rows)
			{
				MessageModel messageModel = new MessageModel();
				messageModel.IdMessage = int.Parse(message["IdMessage"].ToString());
				messageModel.IdMessageTweeter = int.Parse(message["IdMessageTweeter"].ToString());
				messageModel.Text = message["Text"].ToString();
				messageModel.IdTwitterFriend = long.Parse(message["IdTwitterFriend"].ToString());
				messageModel.Date = DateTime.Parse(message["Date"].ToString());
				messageModel.Response = message["Response"].ToString();

				messagesModel.Add(messageModel);
			}

			return messagesModel;
		}

		/// <summary>
		/// Determines whether [is message exist with identifier message twitter] [the specified identifier message tweeter].
		/// </summary>
		/// <param name="idMessageTweeter">The identifier message tweeter.</param>
		/// <returns>
		///   <c>true</c> if [is message exist with identifier message twitter] [the specified identifier message tweeter]; otherwise, <c>false</c>.
		/// </returns>
		internal bool IsMessageExistWithIdMessageTwitter(long idMessageTweeter)
		{
			DataTable messages = messageDal.IsMessageExistWithIdMessageTwitter(idMessageTweeter);
			
			if (messages.Rows.Count > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// Saves the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		internal void Save(MessageModel message)
		{
			messageDal.Save(message);
		}

		/// <summary>
		/// Deletes all.
		/// </summary>
		internal void DeleteAll()
		{
			messageDal.DeleteAll();
		}

		internal long? GetLastMessage()
		{
			DataTable lastMessageResult = messageDal.GetLastMessage();

			if (lastMessageResult != null && lastMessageResult.Rows.Count > 0)
			{
				return long.Parse(lastMessageResult.Rows[0]["IdTwitterFriend"].ToString());
			}
			else
			{
				return null;
			}
		}
	}
}
