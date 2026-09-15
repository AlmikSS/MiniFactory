using KofeyekToolkit.Core.Scenes.Core;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.DI.Core;
using MiniFactory.Gameplay.Machines;
using UnityEngine;

namespace MiniFactory.GamePlay.Boot
{
    public sealed class MainBootstrap : SceneBootstrap
    {
        [SerializeField] private MachinesConfig _machinesConfig;

        private DIContainer _container;
        
        [Inject]
        private void Construct(DIContainer container)
        {
            _container = container;
        }
        
        public override void Initialize(ISceneArgs sceneArgs)
        {
            _container.RegisterInstance(_machinesConfig);
        }
    }
}