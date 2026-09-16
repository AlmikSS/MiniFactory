using KofeyekToolkit.Events;
using MiniFactory.Gameplay.Events;

namespace MiniFactory.Analytics
{
    public sealed class GameplayAnalyticsListener
    {
        private readonly EventBus _eventBus;
        private readonly AnalyticsService _analytics;
        
        public GameplayAnalyticsListener(EventBus eventBus, AnalyticsService analytics)
        {
            _eventBus = eventBus;
            _analytics = analytics;

            _eventBus.Register<MachineUnlockedEvent>(OnMachineUnlocked);
            _eventBus.Register<MachineUpgradedEvent>(OnMachineUpgraded);
            _eventBus.Register<BoostStartedEvent>(OnBoostStarted);
            _eventBus.Register<BoostFinishedEvent>(OnBoostFinished);
            _eventBus.Register<OfflineIncomeAppliedEvent>(OnOfflineIncomeApplied);
        }

        public void Dispose()
        {
            _eventBus.Unregister<MachineUnlockedEvent>(OnMachineUnlocked);
            _eventBus.Unregister<MachineUpgradedEvent>(OnMachineUpgraded);
            _eventBus.Unregister<BoostStartedEvent>(OnBoostStarted);
            _eventBus.Unregister<BoostFinishedEvent>(OnBoostFinished);
            _eventBus.Unregister<OfflineIncomeAppliedEvent>(OnOfflineIncomeApplied);
        }

        private void OnMachineUnlocked(MachineUnlockedEvent evt)
        {
            var parameters = new AnalyticsParametersBuilder()
                .Add(AnalyticsParameterNames.MACHINE_ID, evt.MachineId)
                .Add(AnalyticsParameterNames.COST, evt.Cost)
                .Build();
            _analytics.SendEvent(AnalyticsEventNames.MACHINE_UNLOCKED, parameters);
        }

        private void OnMachineUpgraded(MachineUpgradedEvent evt)
        {
            var parameters = new AnalyticsParametersBuilder()
                .Add(AnalyticsParameterNames.MACHINE_ID, evt.MachineId)
                .Add(AnalyticsParameterNames.LEVEL, evt.NewLevel)
                .Add(AnalyticsParameterNames.COST, evt.Cost)
                .Build();
            _analytics.SendEvent(AnalyticsEventNames.MACHINE_UPGRADED, parameters);
        }

        private void OnBoostStarted(BoostStartedEvent evt)
        {
            var parameters = new AnalyticsParametersBuilder()
                .Add(AnalyticsParameterNames.COST, evt.Cost)
                .Add(AnalyticsParameterNames.DURATION, evt.DurationSeconds)
                .Add(AnalyticsParameterNames.MULTIPLIER, evt.Multiplier)
                .Build();
            _analytics.SendEvent(AnalyticsEventNames.BOOST_STARTED, parameters);
        }

        private void OnBoostFinished(BoostFinishedEvent evt)
        {
            var parameters = new AnalyticsParametersBuilder()
                .Add(AnalyticsParameterNames.MULTIPLIER, evt.Multiplier)
                .Build();
            _analytics.SendEvent(AnalyticsEventNames.BOOST_FINISHED, parameters);
        }

        private void OnOfflineIncomeApplied(OfflineIncomeAppliedEvent evt)
        {
            var parameters = new AnalyticsParametersBuilder()
                .Add(AnalyticsParameterNames.AMOUNT, evt.Amount)
                .Add(AnalyticsParameterNames.ELAPSED_SECONDS, evt.ElapsedSeconds)
                .Add(AnalyticsParameterNames.BOOSTED_SECONDS, evt.BoostedSeconds)
                .Build();
            _analytics.SendEvent(AnalyticsEventNames.OFFLINE_INCOME_APPLIED, parameters);
        }
    }
}