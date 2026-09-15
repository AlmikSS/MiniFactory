using KofeyekToolkit.Core.LifeCycle.Core;
using KofeyekToolkit.Core.Options;
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
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            Log.EnableFileLogging(true);
            Log.Initialize();
            var diContainer = new DIContainer();
            diContainer.RegisterServicesFromAssemblies(typeof(AppBootstrap).Assembly);
            CommandsRegistry.RegisterAllCommands();
            
            var devConsoleUI = Object.FindAnyObjectByType<DevConsoleUI>();
            var loadScreen = Object.FindAnyObjectByType<LoadScreen>();
            var logOptions = Resources.Load<LogOptions>("LogOptions");
            
            var tickService = new TickService(TickOptions.TICK_RATE);
            var spawnService = new SpawnService(tickService, diContainer);
            var sceneSwitcher = new SceneSwitcher(spawnService, loadScreen);
            var eventBus = new EventBus();

            if (logOptions != null)
            {
                tickService.EnableLogging(logOptions.ShowTickServiceDebug);
                spawnService.EnableLogging(logOptions.ShowSpawnServiceDebug);
                sceneSwitcher.EnableLogging(logOptions.ShowSceneSwitcherDebug);
                eventBus.EnableLogging(logOptions.ShowEventBusDebug);
                devConsoleUI?.EnableLogging(logOptions.ShowDevUIDebug);
                diContainer.EnableLogging(logOptions.ShowDiDebug);
                CommandExecutor.EnableLogging(logOptions.ShowCommandExecutorDebug);
                CommandsRegistry.EnableLogging(logOptions.ShowCommandsRegistryDebug);
            }
            
            diContainer.RegisterInstance(tickService);
            diContainer.RegisterInstance(spawnService);
            diContainer.RegisterInstance(sceneSwitcher);
            diContainer.RegisterInstance(eventBus);
            
            tickService.Register(spawnService);
            spawnService.SpawnInSceneObjects();
            
            tickService.EnableTicking(true);
        }
    }
}