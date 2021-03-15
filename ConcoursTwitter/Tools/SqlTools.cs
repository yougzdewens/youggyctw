using System;
using System.Collections.Generic;
using System.Text;

namespace ConcoursTwitter.Tools
{
	public class SqlTools
	{
		internal static string SanitizeForSql(string contentToSanitize)
		{
			contentToSanitize = contentToSanitize.Replace("\"", "");
			contentToSanitize = contentToSanitize.Replace("'", "");

			return contentToSanitize;
		}
	}
}
