using System;
using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Events;
using MiniFactory.Persistence.Data;
using MiniFactory.Persistence.Interfaces;
using MiniFactory.Persistence.Logic;

namespace MiniFactory.Gameplay.Boost
{
    public sealed class BoostService : IGameplayTickable, ISaveable
    {
        private readonly BoostConfig _config;
        private readonly WalletService _walletService;
        private readonly EventBus _eventBus;
        private readonly SaveService _saveService;

        private long _startUnixSeconds;
        private long _endUnixSeconds;
        private bool _finishEventSent;

        public bool IsActive => _config.Enabled && _endUnixSeconds > NowUnix;
        public bool IsEnabled => _config.Enabled;
        public int Price => _config.Price;
        public float Multiplier => _config.Multiplier;
        public float DurationSeconds => _config.DurationSeconds;
        public float RemainingSeconds => IsActive ? (_endUnixSeconds - NowUnix) : 0f;

        private static long NowUnix => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public BoostService(BoostConfig config, WalletService walletService, EventBus eventBus, SaveService saveService)
        {
            _config = config;
            _walletService = walletService;
            _eventBus = eventBus;
            _saveService = saveService;
        }

        public bool TryStart()
        {
            if (!_config.Enabled)
            {
                Log.Warning("Boost is disabled in config.");
                return false;
            }

            if (IsActive)
                return false;

            if (!_walletService.TrySpend(_config.Price))
                return false;

            var now = NowUnix;
            _startUnixSeconds = now;
            _endUnixSeconds = now + (long)_config.DurationSeconds;
            _finishEventSent = false;
            _saveService.MarkDirty();

            _eventBus.Invoke(new BoostStartedEvent
            {
                Cost = _config.Price,
                DurationSeconds = _config.DurationSeconds,
                Multiplier = _config.Multiplier
            });

            return true;
        }

        public bool CanStart()
        {
            return _config.Enabled && !IsActive && _walletService.Balance >= _config.Price;
        }

        public long GetBoostedSecondsInRange(long rangeStart, long rangeEnd)
        {
            if (_endUnixSeconds <= rangeStart || _startUnixSeconds >= rangeEnd)
                return 0;

            var overlapStart = Math.Max(_startUnixSeconds, rangeStart);
            var overlapEnd = Math.Min(_endUnixSeconds, rangeEnd);
            return Math.Max(0, overlapEnd - overlapStart);
        }

        public void Tick(float deltaTime)
        {
            if (_finishEventSent || _endUnixSeconds == 0)
                return;

            if (IsActive)
                return;

            _finishEventSent = true;
            _saveService.MarkDirty();

            _eventBus.Invoke(new BoostFinishedEvent
            {
                Multiplier = _config.Multiplier
            });
        }

        public void Capture(SaveData data)
        {
            data.Boost.IsActive = IsActive;
            data.Boost.StartUnixSeconds = _startUnixSeconds;
            data.Boost.EndUnixSeconds = _endUnixSeconds;
            data.Boost.Multiplier = _config.Multiplier;
        }

        public void Restore(SaveData data)
        {
            if (data.Boost == null || data.Boost.EndUnixSeconds == 0)
                return;

            _startUnixSeconds = data.Boost.StartUnixSeconds;
            _endUnixSeconds = data.Boost.EndUnixSeconds;
            _finishEventSent = !IsActive;
        }
    }
}