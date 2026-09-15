using KofeyekToolkit.Core.Scenes.Core;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.DI.Core;
using MiniFactory.Gameplay.Economy;
using UnityEngine;

namespace MiniFactory.Gameplay.Boot
{
    public sealed class MainBootstrap : SceneBootstrap
    {
        [SerializeField] private EconomyConfig _economyConfig;

        [Inject] private DIContainer _container;

        public override void Initialize(ISceneArgs sceneArgs)
        {
            _container.RegisterInstance(_economyConfig);
        }
    }
}