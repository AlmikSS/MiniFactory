using KofeyekToolkit.Core.Scenes.Core;
using KofeyekToolkit.Core.TickSystem;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.DI.Core;
using KofeyekToolkit.Events;
using MiniFactory.Analytics;
using MiniFactory.Gameplay.Boost;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Persistence.Logic;
using UnityEngine;

namespace MiniFactory.Gameplay.Boot
{
    public sealed class MainBootstrap : SceneBootstrap
    {
        [SerializeField] private EconomyConfig _economyConfig;
        [SerializeField] private BoostConfig _boostConfig;

        private DIContainer _container;

        [Inject]
        private void Construct(DIContainer container)
        {
            _container = container;
        }

        public override void Initialize(ISceneArgs sceneArgs)
        {
            var eventBus = _container.Resolve<EventBus>();

            var saveStorage = new FileSaveStorage();
            var saveService = new SaveService(saveStorage);
            var walletService = new WalletService(_economyConfig, saveService);
            var boostService = new BoostService(_boostConfig, walletService, eventBus, saveService);
            
            _container.RegisterInstance(saveService);
            _container.RegisterInstance(walletService);
            _container.RegisterInstance(boostService);
            _container.RegisterInstance(_economyConfig);
            
            saveService.Register(walletService);
            saveService.Register(boostService);
            
            saveService.Restore();
            
            var tickService = _container.Resolve<TickService>();
            tickService.Register(boostService);
            
            var analytics = new AnalyticsService();
            analytics.Register(new DebugAnalyticsProvider());
            _container.RegisterInstance(analytics);

            var analyticsListener = new GameplayAnalyticsListener(eventBus, analytics);
            _container.RegisterInstance(analyticsListener);

            analytics.SendEvent(AnalyticsEventNames.GAME_STARTED);
        }
    }
}