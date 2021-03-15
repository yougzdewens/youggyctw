using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTweeter.Tools
{
	public class LogTool
	{
		/// <summary>
		/// The log
		/// </summary>
		protected static readonly ILog _log = LogManager.GetLogger(typeof(LogTool));

		/// <summary>
		/// Writes the log.
		/// </summary>
		/// <param name="text">The text.</param>
		public static void WriteLog(string text)
		{
			_log.Info(text);
		}
	}
}
