using System;
using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using MiniFactory.Gameplay.Boost;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Events;
using MiniFactory.Gameplay.Machines;
using MiniFactory.Persistence.Data;
using MiniFactory.Persistence.Logic;

namespace MiniFactory.Gameplay.Offline
{
    public sealed class OfflineProgressService : ISystemTickable

    {
        private readonly EconomyConfig _economyConfig;
        private readonly WalletService _walletService;
        private readonly MachineController _machineController;
        private readonly BoostService _boostService;
        private readonly SaveService _saveService;
        private readonly EventBus _eventBus;
        private bool _applied;

        private static long NowUnix => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public OfflineProgressService(EconomyConfig economyConfig, WalletService walletService, MachineController machineController, BoostService boostService, SaveService saveService, EventBus eventBus)
        {
            _economyConfig = economyConfig;
            _walletService = walletService;
            _machineController = machineController;
            _boostService = boostService;
            _saveService = saveService;
            _eventBus = eventBus;
        }

        public void Tick(float deltaTime)
        {
            if (_applied)
                return;

            _applied = true;
            Apply();
        }

        private void Apply()
        {
            var data = _saveService.GetData();
            if (data is not { LastExitUnixSeconds: > 0 })
                return;

            var now = NowUnix;
            var elapsed = now - data.LastExitUnixSeconds;
            if (elapsed <= 0)
                return;

            var cappedElapsed = Math.Min(elapsed, _economyConfig.MaxOfflineSeconds);
            if (cappedElapsed <= 0)
                return;

            var boostedSeconds = _boostService.GetBoostedSecondsInRange(
                data.LastExitUnixSeconds,
                data.LastExitUnixSeconds + cappedElapsed);

            var normalSeconds = cappedElapsed - boostedSeconds;

            double baseIncome = _machineController.GetTotalIncomePerSecond();
            var income = baseIncome * normalSeconds + baseIncome * _boostService.Multiplier * boostedSeconds;

            var rounded = (int)Math.Round(income);
            if (rounded <= 0)
                return;

            _walletService.Add(rounded);

            _eventBus.Invoke(new OfflineIncomeAppliedEvent
            {
                Amount = rounded,
                ElapsedSeconds = cappedElapsed,
                BoostedSeconds = boostedSeconds
            });

            Log.Message($"Offline income: {rounded} (elapsed {cappedElapsed}s, boosted {boostedSeconds}s)");
        }
    }
}