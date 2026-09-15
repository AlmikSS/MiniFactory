namespace KofeyekToolkit.Core.LifeCycle.Core.Interfaces
{
    /// <summary>
    /// Определяет обработчик возврата объекта в пул или деактивации.
    /// </summary>
    public interface IDespawnable
    {
        void OnDespawn();
    }
}