using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioMetrixCore.Utilities
{
    public static class Logger
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public static void HandleException(Exception ex)
        {
            LogError(ex.ToString());
        }

        public static void LogError(string message)
        {
            logger.Log(LogLevel.Error, message);
        }

        public static void LogInfo(string message)
        {
            logger.Log(LogLevel.Info, message);
        }

        public static void LogDebug(string message)
        {
            logger.Log(LogLevel.Debug, message);
        }
    }
}