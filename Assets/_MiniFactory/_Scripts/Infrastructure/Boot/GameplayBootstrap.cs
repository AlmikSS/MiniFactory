using KofeyekToolkit.Core.Scenes.Core;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.DI.Core;
using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Machines;
using MiniFactory.Persistence.Logic;

namespace MiniFactory.GamePlay.Boot
{
    public sealed class GameplayBootstrap : SceneBootstrap
    {
        private DIContainer _container;
        private WalletService _walletService;
        private EventBus _eventBus;
        private SaveService _saveService;

        [Inject]
        public void Construct(DIContainer container, WalletService walletService, EventBus eventBus, SaveService saveService)
        {
            _container = container;
            _walletService = walletService;
            _eventBus = eventBus;
            _saveService = saveService;
        }
        
        public override void Initialize(ISceneArgs sceneArgs)
        {
            var machineRegistry = FindAnyObjectByType<MachineRegistry>();
            if (machineRegistry == null)
            {
                Log.Error($"{nameof(MachineRegistry)} not found on scene.");
                return;
            }
            
            var machineController = new MachineController(machineRegistry.Machines, _walletService, _eventBus, _saveService);
            _container.RegisterInstance(machineController);
            _saveService.Register(machineController);
            _saveService.Restore();
        }
    }
}