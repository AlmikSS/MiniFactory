using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace KofeyekToolkit.Logging
{
    public static class Log
    {
        private static bool _isInitialized;
        private static string _logFilePath;

        private static bool _fileLoggingEnabled = true;
        private static int _maxLogFiles = 10;
        private static readonly object FileLock = new();

        /// <summary>
        /// Инициализация логгера. Создаёт папку, удаляет старые файлы (если превышен лимит) и формирует путь к новому файлу.
        /// </summary>
        /// <param name="fileName">Базовое имя файла (к нему добавится временная метка)</param>
        public static void Initialize(string fileName = "log.log")
        {
            if (_isInitialized)
                return;

            var folder = Application.persistentDataPath + "/Logs/";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            if (_maxLogFiles > 0)
            {
                var files = Directory.GetFiles(folder, "*.log");
                if (files.Length >= _maxLogFiles)
                {
                    var sortedFiles = files.OrderBy(f => f).ToList();
                    var toDelete = sortedFiles.Count - _maxLogFiles + 1;
                    for (var i = 0; i < toDelete; i++)
                    {
                        try { File.Delete(sortedFiles[i]); }
                        catch (Exception ex) { Debug.LogWarning($"Не удалось удалить старый лог: {ex.Message}"); }
                    }
                }
            }

            var data = DateTime.Now.ToString("yyyy-MM-dd HH-mm");
            _logFilePath = Path.Combine(folder, data + "_" + fileName);

            _isInitialized = true;
        }

        /// <summary>Включить/выключить запись в файл</summary>
        public static void EnableFileLogging(bool enable)
        {
            _fileLoggingEnabled = enable;
        }

        /// <summary>Установить максимальное количество хранимых файлов (0 = без ограничений)</summary>
        public static void SetMaxLogFiles(int max)
        {
            _maxLogFiles = Math.Max(1, max);
        }
        
        public static void Message(string msg, [CallerFilePath] string filePath = "")
        {
            Write(msg, filePath, LogLevel.Log);
        }

        public static void Warning(string msg, [CallerFilePath] string filePath = "")
        {
            Write(msg, filePath, LogLevel.Warning);
        }

        public static void Error(string msg, [CallerFilePath] string filePath = "")
        {
            Write(msg, filePath, LogLevel.Error);
        }
        
        private static void Write(string msg, string filePath, LogLevel level)
        {
            if (!_isInitialized)
                Initialize();
            
            var sender = Path.GetFileNameWithoutExtension(filePath);
            var dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var log = $"[{sender}]:[{dateTime}] - {msg}";

            switch (level)
            {
                case LogLevel.Log:
                    Debug.Log($"<color=#00BFFF>{log}</color>");
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning($"<color=#FFA500>{log}</color>");
                    break;
                case LogLevel.Error:
                    Debug.LogError($"<color=#FF4444>{log}</color>");
                    break;
            }

            if (!_fileLoggingEnabled || string.IsNullOrEmpty(_logFilePath))
                return;
            
            var logToFile = $"[{level.ToString()}] {log}";
                
            lock (FileLock)
            {
                try
                {
                    using var writer = new StreamWriter(_logFilePath, append: true);
                    writer.WriteLine(logToFile);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Ошибка записи в файл лога: {ex.Message}");
                }
            }
        }
    }

    public enum LogLevel
    {
        Log,
        Warning,
        Error
    }
}