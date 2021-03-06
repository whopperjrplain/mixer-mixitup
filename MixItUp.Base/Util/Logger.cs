﻿using MixItUp.Base.Model.API;
using MixItUp.Base.Services;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace MixItUp.Base.Util
{
    public static class Logger
    {
        public static bool IsDebug { get; private set; }
        public static string CurrentLogFilePath { get; private set; }

        private const string LogsDirectoryName = "Logs";
        private const string LogFileNameFormat = "MixItUpLog-{0}.txt";

        private static IFileService fileService;

        public static void Initialize(IFileService fileService)
        {
            Logger.fileService = fileService;
            Logger.fileService.CreateDirectory(LogsDirectoryName);
            Logger.CurrentLogFilePath = Path.Combine(LogsDirectoryName, string.Format(LogFileNameFormat, DateTime.Now.ToString("yyyy-MM-dd-HH-mm", CultureInfo.InvariantCulture)));

            Mixer.Base.Util.Logger.LogOccurred += Logger_LogOccurred;

            #if DEBUG
                Logger.IsDebug = true;
            #endif
        }

        public static void Log(string message)
        {
            try
            {
                Logger.fileService.AppendFile(Logger.CurrentLogFilePath, DateTimeOffset.Now.ToString() + " - " + message + Environment.NewLine + Environment.NewLine);
            }
            catch (Exception) { }
        }

        public static void Log(Exception ex, bool isCrashing = false)
        {
            if (!Logger.IsDebug)
            {
                Task.Run(async () => { await ChannelSession.Services.MixItUpService.SendErrorEvent(new ErrorEvent(JsonConvert.SerializeObject(ex.ToString()), isCrashing)); });
            }
            Logger.Log(ex.ToString());
        }

        private static void Logger_LogOccurred(object sender, string e)
        {
            Logger.Log(e);
        }
    }
}
