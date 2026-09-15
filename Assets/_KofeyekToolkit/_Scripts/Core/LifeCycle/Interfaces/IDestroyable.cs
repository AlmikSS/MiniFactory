namespace KofeyekToolkit.Core.LifeCycle.Core.Interfaces
{
    /// <summary>
    /// Определяет обработчик окончательного уничтожения объекта.
    /// </summary>
    public interface IDestroyable
    {
        void OnDestroyed();
    }
}