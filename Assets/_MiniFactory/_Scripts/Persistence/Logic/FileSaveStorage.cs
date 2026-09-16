using System.IO;
using KofeyekToolkit.Logging;
using MiniFactory.Persistence.Interfaces;
using UnityEngine;

namespace MiniFactory.Persistence.Logic
{
    public sealed class FileSaveStorage : ISaveStorage
    {
        private readonly string _path;

        public FileSaveStorage(string fileName = "save.json")
        {
            _path = Path.Combine(Application.persistentDataPath, fileName);
        }

        public bool Exists() => File.Exists(_path);

        public string Read()
        {
            if (!File.Exists(_path))
                return string.Empty;

            try
            {
                return File.ReadAllText(_path);
            }
            catch (IOException ex)
            {
                Log.Error($"Failed to read save file: {ex.Message}");
                return string.Empty;
            }
        }

        public void Write(string content)
        {
            try
            {
                File.WriteAllText(_path, content);
            }
            catch (IOException ex)
            {
                Log.Error($"Failed to write save file: {ex.Message}");
            }
        }

        public void Delete()
        {
            if (!File.Exists(_path))
                return;

            try
            {
                File.Delete(_path);
            }
            catch (IOException ex)
            {
                Log.Error($"Failed to delete save file: {ex.Message}");
            }
        }
    }
}