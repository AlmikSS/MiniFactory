using System;
using System.Collections.Generic;
using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.Logging;
using MiniFactory.Persistence.Data;
using MiniFactory.Persistence.Interfaces;
using UnityEngine;

namespace MiniFactory.Persistence.Logic
{
    public sealed class SaveService : ISystemTickable
    {
        private const float AUTO_SAVE_INTERVAL = 5f;

        private readonly ISaveStorage _storage;
        private readonly List<ISaveable> _saveables = new();

        private SaveData _lastLoadedData;
        private float _autoSaveTimer;
        private bool _isDirty;

        public SaveService(ISaveStorage storage)
        {
            _storage = storage;
        }

        public SaveData GetData() => _lastLoadedData;
        
        public void Register(ISaveable saveable)
        {
            if (saveable == null || _saveables.Contains(saveable))
                return;

            _saveables.Add(saveable);
        }

        public void Unregister(ISaveable saveable)
        {
            _saveables.Remove(saveable);
        }

        public void MarkDirty()
        {
            _isDirty = true;
        }

        public void Restore()
        {
            var content = _storage.Read();
            SaveData data;

            if (string.IsNullOrEmpty(content))
            {
                data = new SaveData();
                Log.Message("No save file found, starting fresh.");
            }
            else
            {
                try
                {
                    data = JsonUtility.FromJson<SaveData>(content) ?? new SaveData();
                }
                catch (Exception ex)
                {
                    Log.Error($"Failed to parse save file: {ex.Message}. Starting fresh.");
                    data = new SaveData();
                }
            }

            foreach (var saveable in _saveables)
            {
                try
                {
                    saveable.Restore(data);
                }
                catch (Exception ex)
                {
                    Log.Error($"Restore failed for {saveable.GetType().Name}: {ex.Message}");
                }
            }

            _lastLoadedData = data;
            _isDirty = false;
        }

        public void Save()
        {
            var data = new SaveData();

            foreach (var saveable in _saveables)
            {
                try
                {
                    saveable.Capture(data);
                }
                catch (Exception ex)
                {
                    Log.Error($"Capture failed for {saveable.GetType().Name}: {ex.Message}");
                }
            }

            data.LastExitUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var content = JsonUtility.ToJson(data, true);
            _storage.Write(content);
            _isDirty = false;
        }

        public void Tick(float deltaTime)
        {
            if (!_isDirty)
                return;

            _autoSaveTimer += deltaTime;
            if (_autoSaveTimer < AUTO_SAVE_INTERVAL)
                return;

            _autoSaveTimer = 0f;
            Save();
        }
    }
}