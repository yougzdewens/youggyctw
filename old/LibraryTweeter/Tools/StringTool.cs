using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTweeter.Tools
{
	public class StringTool
	{


		/// <summary>
		/// Tests if contains word.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="listofListWordsToFind">The listof list words to find.</param>
		/// <param name="allWord">if set to <c>true</c> [all word].</param>
		/// <returns></returns>
		public static bool TestIfContainsWord(string content, List<List<string>> listofListWordsToFind, bool allWord = false)
		{
			foreach (List<string> listWords in listofListWordsToFind)
			{
				if (TestIfContainsWord(content, listWords, allWord))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Tests if contains word.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <param name="listWordsToFind">The list words to find.</param>
		/// <param name="allWord">if set to <c>true</c> [all word].</param>
		/// <returns></returns>
		public static bool TestIfContainsWord(string content, List<string> listWordsToFind, bool allWord = false)
		{
			int countWordFound = 0;

			if (content == null)
			{
				throw new Exception("text content for TestIfContainsWord method can't be null");
			}

			content = content.ToLower();

			foreach (string word in listWordsToFind)
			{
				if (content.Contains(word.ToLower()))
				{
					if (allWord == false)
					{
						return true;
					}
					else
					{
						countWordFound++;
					}
				}
			}

			if (allWord == true && listWordsToFind.Count == countWordFound)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
