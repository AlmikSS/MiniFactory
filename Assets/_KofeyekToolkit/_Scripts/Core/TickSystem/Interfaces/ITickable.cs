namespace KofeyekToolkit.Core.TickSystem.Interfaces
{
    /// <summary>
    /// Определяет объект, который может получать обновления от <see cref="TickService"/>.
    /// </summary>
    public interface ITickable
    {
        void Tick(float deltaTime);
    }
}