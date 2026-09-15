namespace KofeyekToolkit.Core.LifeCycle.Core.Interfaces
{
    /// <summary>
    /// Определяет обработчик первичной инициализации объекта.
    /// </summary>
    public interface IConstructable
    {
        void OnConstruct();
    }
}