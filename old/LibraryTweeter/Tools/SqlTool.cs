using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTweeter.Tools
{
	public class SqlTool
	{
		internal static string SanitizeForSql(string contentToSanitize)
		{
			contentToSanitize = contentToSanitize.Replace("\"", "");
			contentToSanitize = contentToSanitize.Replace("'", "");

			return contentToSanitize;
		}
	}
}
