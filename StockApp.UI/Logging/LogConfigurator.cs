using System;
using System.Diagnostics;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;

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
	}
}