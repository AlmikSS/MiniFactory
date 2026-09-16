using KofeyekToolkit.Core.Scenes.Core;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.DI.Core;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Persistence.Logic;
using UnityEngine;

namespace MiniFactory.Gameplay.Boot
{
    public sealed class MainBootstrap : SceneBootstrap
    {
        [SerializeField] private EconomyConfig _economyConfig;

        private DIContainer _container;

        [Inject]
        private void Construct(DIContainer container)
        {
            _container = container;
        }

        public override void Initialize(ISceneArgs sceneArgs)
        {
            var saveStorage = new FileSaveStorage();
            var saveService = new SaveService(saveStorage);
            var walletService = new WalletService(_economyConfig, saveService);
            
            _container.RegisterInstance(saveService);
            _container.RegisterInstance(walletService);
            
            saveService.Register(walletService);
            saveService.Restore();
        }
    }
}