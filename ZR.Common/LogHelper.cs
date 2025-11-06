using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZR.Common
{
    public class LogHelper
    {
        public readonly ILogger Logger;

        public LogHelper(bool hasTime = true)
        {
            var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }

            var config = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .WriteTo.Seq("http://localhost:5341")
              .WriteTo.Console();

            if (!hasTime)
            {
                config.WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, outputTemplate: "{Message:lj}{NewLine}");
            }
            else
            {
                config.WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day);
            }
            Logger = config.CreateLogger();

        }
        public LogHelper(string logFilePath, bool hasTime = true)
        {

            if (!Directory.Exists(logFilePath))
            {
                Directory.CreateDirectory(logFilePath);
            }

            var config = new LoggerConfiguration()
            .MinimumLevel.Debug()
              .WriteTo.Seq("http://localhost:5341")
            .WriteTo.Console();

            if (!hasTime)
            {
                config.WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day, outputTemplate: "{Message:lj}{NewLine}");
            }
            else
            {
                config.WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day);
            }
            Logger = config.CreateLogger();
        }

    }
}
