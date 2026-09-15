using KofeyekToolkit.DI.Core;

namespace KofeyekToolkit.Core.LifeCycle.Core.Interfaces
{
    /// <summary>
    /// Определяет запрос, который <see cref="SpawnService"/> выполняет в своём тике.
    /// </summary>
    public interface ISpawnRequest
    {
        void Execute(SpawnService spawnService);
    }
}