using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace StockApp.UI.Logging
{
	internal static class LogConfigurator
	{
		public static void Configure()
		{
			try
			{
				var logDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "StockApp");
				Directory.CreateDirectory(logDir);

				// active file stays "StockApp.log"
				var logFile = Path.Combine(logDir, "StockApp.log");

				var layout = new PatternLayout
				{
					ConversionPattern = "%date [%thread] %level %logger - %message%newline"
				};
				layout.ActivateOptions();

				var roller = new RollingFileAppender
				{
					Name = "RollingFile",
					File = logFile,
					AppendToFile = true,
					RollingStyle = RollingFileAppender.RollingMode.Composite,
					MaxSizeRollBackups = 5,
					MaximumFileSize = "2MB",
					// DatePattern used for archived names; PreserveLogFileNameExtension ensures date is placed before ".log"
					DatePattern = "-yyyy-MM-dd'.log'",
					// keep current filename as "StockApp.log"
					StaticLogFileName = true,
					// insert date before extension for archives: "StockApp-YYYY-MM-DD.log"
					PreserveLogFileNameExtension = true,
					Layout = layout,
					LockingModel = new FileAppender.MinimalLock()
				};
				roller.ActivateOptions();

				var hierarchy = (Hierarchy)LogManager.GetRepository();
				hierarchy.Root.RemoveAllAppenders();
				hierarchy.Root.AddAppender(roller);
				hierarchy.Root.Level = log4net.Core.Level.Info;
				hierarchy.Configured = true;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"LogConfigurator.Configure failed: {ex}");
			}
		}

		internal static string GetLogFile()
		{
			var _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			// Read path from log4net configuration (FileAppender / RollingFileAppender).
			string file = null;
			try
			{
				if (LogManager.GetRepository() is Hierarchy hierarchy)
				{
					var appenders = hierarchy.GetAppenders();
					var fileAppender = appenders.OfType<FileAppender>().FirstOrDefault()
									   ?? appenders.OfType<RollingFileAppender>().FirstOrDefault();

					if (fileAppender != null)
					{
						file = (fileAppender as FileAppender)?.File ?? (fileAppender as RollingFileAppender)?.File;
					}
				}
			}
			catch (Exception ex)
			{
				_log?.Warn("Unable to read log4net appenders to locate log file path.", ex);
				return null;
			}

			if (string.IsNullOrWhiteSpace(file))
			{
				_log?.Warn("No file appender configured in log4net; cannot determine log folder.");
				return null;
			}

			// Make path absolute if relative
			if (!Path.IsPathRooted(file))
			{
				file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file);
			}

			return file;

		}
	}
}