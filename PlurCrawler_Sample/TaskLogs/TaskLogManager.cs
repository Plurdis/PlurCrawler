﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using PlurCrawler.Attributes;
using PlurCrawler.Extension;

namespace PlurCrawler_Sample.TaskLogs
{
    public static class TaskLogManager
    {
        // TODO: Rich Log Add Implement

        public delegate void LogDelegate(TaskLog taskLog);

        public static event LogDelegate LogAdded;

        static StreamWriter sw;

        public static void Init()
        {
        }

        private static void OnLogAdded(TaskLog taskLog)
        {
            LogAdded?.Invoke(taskLog);
        }

        static TaskLogManager()
        {
            sw = new StreamWriter("Log.txt");
        }
        
        public static void AddLog(string message, TaskLogType type)
        {
            OnLogAdded(new TaskLog()
            {
                DateTime = DateTime.Now,
                LogType = type,
                Message = message
            });

            sw.WriteLine($"[{DateTime.Now.ToString("MM/dd HH:mm:ss")}] [{type.GetAttributeFromEnum<NoteAttribute>().Message}] {message}");
            sw.Flush();
        }
    }
}
