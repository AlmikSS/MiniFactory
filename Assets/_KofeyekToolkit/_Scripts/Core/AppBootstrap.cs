using KofeyekToolkit.Core.LifeCycle.Core;
using KofeyekToolkit.Core.Options;
using KofeyekToolkit.Core.Scenes.Core;
using KofeyekToolkit.Core.Scenes.Management;
using KofeyekToolkit.Core.Scenes.Visual;
using KofeyekToolkit.Core.TickSystem;
using KofeyekToolkit.DevConsole;
using KofeyekToolkit.DI.Core;
using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using UnityEngine;

namespace KofeyekToolkit.Core
{
    /// <summary>
    /// Глобальная точка входа в приложение
    /// </summary>
    internal static class AppBootstrap
    {
        private const string GAMEPLAY_SCENE = "Gameplay";
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            Log.EnableFileLogging(true);
            Log.Initialize();
            var diContainer = new DIContainer();
            diContainer.RegisterServicesFromAssemblies(typeof(AppBootstrap).Assembly);

            #if UNITY_EDITOR
                CommandsRegistry.RegisterAllCommands();
            #endif
            
            //var devConsoleUI = Object.FindAnyObjectByType<DevConsoleUI>();
            var loadScreen = Object.FindAnyObjectByType<LoadScreen>();
            var logOptions = Resources.Load<LogOptions>("LogOptions");
            
            var tickService = new TickService(TickOptions.TICK_RATE);
            var spawnService = new SpawnService(tickService, diContainer);
            var sceneSwitcher = new SceneSwitcher(spawnService, loadScreen, diContainer);
            var eventBus = new EventBus();

            if (logOptions != null)
            {
                tickService.EnableLogging(logOptions.ShowTickServiceDebug);
                spawnService.EnableLogging(logOptions.ShowSpawnServiceDebug);
                sceneSwitcher.EnableLogging(logOptions.ShowSceneSwitcherDebug);
                eventBus.EnableLogging(logOptions.ShowEventBusDebug);
                //devConsoleUI?.EnableLogging(logOptions.ShowDevUIDebug);
                diContainer.EnableLogging(logOptions.ShowDiDebug);
                //CommandExecutor.EnableLogging(logOptions.ShowCommandExecutorDebug);
                //CommandsRegistry.EnableLogging(logOptions.ShowCommandsRegistryDebug);
            }
            diContainer.RegisterInstance(diContainer);
            diContainer.RegisterInstance(tickService);
            diContainer.RegisterInstance(spawnService);
            diContainer.RegisterInstance(sceneSwitcher);
            diContainer.RegisterInstance(eventBus);
            
            tickService.Register(spawnService);
            spawnService.SpawnInSceneObjects();
            
            tickService.EnableTicking(true);
            
            var bootstrap = Object.FindAnyObjectByType<SceneBootstrap>();
            if (bootstrap != null)
            {
                diContainer.Inject(bootstrap);
                bootstrap.Initialize(null);
            }
            
            sceneSwitcher.LoadScene(GAMEPLAY_SCENE, null);
        }
    }
}