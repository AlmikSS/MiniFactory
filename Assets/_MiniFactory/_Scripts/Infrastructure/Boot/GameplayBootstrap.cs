using KofeyekToolkit.Core.Scenes.Core;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.DI.Core;
using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Machines;

namespace MiniFactory.GamePlay.Boot
{
    public sealed class GameplayBootstrap : SceneBootstrap
    {
        [Inject] private DIContainer _container;
        [Inject] private WalletService _walletService;
        [Inject] private EventBus _eventBus;
        
        public override void Initialize(ISceneArgs sceneArgs)
        {
            var machineRegistry = FindAnyObjectByType<MachineRegistry>();
            if (machineRegistry == null)
            {
                Log.Error($"{nameof(MachineRegistry)} not found on scene.");
                return;
            }
            
            var machineController = new MachineController(machineRegistry.Machines, _walletService, _eventBus);
            _container.RegisterInstance(machineController);
        }
    }
}