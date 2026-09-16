using KofeyekToolkit.Core.Scenes.Core;
using KofeyekToolkit.Core.TickSystem;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.DI.Core;
using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using MiniFactory.Gameplay.Boost;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Machines;
using MiniFactory.Gameplay.Offline;
using MiniFactory.Persistence.Logic;

namespace MiniFactory.GamePlay.Boot
{
    public sealed class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;

        [Inject]
        public void Construct(DIContainer container)
        {
            _container = container;
        }

        public override void Initialize(ISceneArgs sceneArgs)
        {
            var walletService = _container.Resolve<WalletService>();
            var eventBus = _container.Resolve<EventBus>();
            var saveService = _container.Resolve<SaveService>();

            var machineRegistry = FindAnyObjectByType<MachineRegistry>();
            if (machineRegistry == null)
            {
                Log.Error($"{nameof(MachineRegistry)} not found on scene.");
                return;
            }
            
            var machineController = new MachineController(machineRegistry.Machines, walletService, eventBus, saveService);
            saveService.Register(machineController);
            saveService.Restore();
            
            var economyConfig = _container.Resolve<EconomyConfig>();
            var boostService = _container.Resolve<BoostService>();
            var offline = new OfflineProgressService(economyConfig, walletService, machineController, boostService, saveService, eventBus);
            
            _container.RegisterInstance(machineController);
            _container.RegisterInstance(offline);
            
            var tickService = _container.Resolve<TickService>();
            tickService.Register(offline);
        }
    }
}