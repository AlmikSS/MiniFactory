using UnityEngine;

namespace KofeyekToolkit.Core.Scenes.Core
{
    /// <summary>
    /// Базовый компонент инициализации, получающий аргументы после загрузки сцены.
    /// </summary>
    public abstract class SceneBootstrap : MonoBehaviour
    {
        public abstract void Initialize(ISceneArgs sceneArgs);
    }
}