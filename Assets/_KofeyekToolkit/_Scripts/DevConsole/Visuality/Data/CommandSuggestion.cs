namespace KofeyekToolkit.DevConsole
{
    /// <summary>
    /// Представляет подсказку команды с её именем, описанием и строкой использования.
    /// </summary>
    public sealed class CommandSuggestion
    {
        /// <summary>
        /// Имя сущности.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// Текстовое описание сущности.
        /// </summary>
        public readonly string Description;
        /// <summary>
        /// Строка использования команды.
        /// </summary>
        public readonly string Usage;

        /// <summary>
        /// Предоставляет API-член <c>CommandSuggestion</c>.
        /// </summary>
        public CommandSuggestion(string name, string description, string usage)
        {
            Name = name;
            Description = description;
            Usage = usage;
        }
    }
}