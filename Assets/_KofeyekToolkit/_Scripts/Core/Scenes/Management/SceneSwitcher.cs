using System.Collections;
using KofeyekToolkit.Core.LifeCycle.Core;
using KofeyekToolkit.Core.Scenes.Core;
using KofeyekToolkit.Core.Scenes.Visual;
using KofeyekToolkit.DI.Core;
using KofeyekToolkit.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KofeyekToolkit.Core.Scenes.Management
{
    /// <summary>
    /// Асинхронно загружает сцену, отображает экран загрузки и запускает её инициализацию.
    /// </summary>
    public sealed class SceneSwitcher
    {
        private readonly SpawnService _spawnService;
        private readonly LoadScreen _loadScreen;
        private readonly DIContainer _container;
        private bool _isLoggingEnabled = true;

        public bool IsLoggingEnabled => _isLoggingEnabled;

        public void EnableLogging(bool enable) => _isLoggingEnabled = enable;
        
        /// <summary>
        /// Предоставляет API-член <c>SceneSwitcher</c>.
        /// </summary>
        public SceneSwitcher(SpawnService spawnService, LoadScreen loadScreen, DIContainer container)
        {
            _loadScreen = loadScreen;
            _container = container;
            _spawnService = spawnService;
        }
        
        /// <summary>
        /// Запускает асинхронную загрузку сцены и передаёт ей аргументы.
        /// </summary>
        public void LoadScene(string sceneName, ISceneArgs sceneArgs)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Error("Cannot load a scene with an empty name.");
                return;
            }

            Message($"Started loading scene '{sceneName}'.");
            CoroutinePerformer.Instance.StartCoroutine(LoadSceneRoutine(sceneName, sceneArgs));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, ISceneArgs sceneArgs)
        {
            _spawnService.DespawnInSceneObjects();
            _loadScreen.Show();
            var waitOperation = SceneManager.LoadSceneAsync(sceneName);
            
            yield return new WaitUntil(() => waitOperation.isDone);
            
            _loadScreen.Hide();
            var bootstrap = Object.FindAnyObjectByType<SceneBootstrap>();
            if (bootstrap == null)
            {
                Warning($"Scene '{sceneName}' has no SceneBootstrap.");
                yield break;
            }
            
            _container.Inject(bootstrap);
            bootstrap.Initialize(sceneArgs);
            _spawnService.SpawnInSceneObjects();
            Message($"Scene '{sceneName}' loaded and initialized.");
        }

        private void Message(string message)
        {
            if (_isLoggingEnabled)
                Log.Message(message);
        }

        private void Warning(string message)
        {
            if (_isLoggingEnabled)
                Log.Warning(message);
        }

        private void Error(string message)
        {
            if (_isLoggingEnabled)
                Log.Error(message);
        }

    }
}
