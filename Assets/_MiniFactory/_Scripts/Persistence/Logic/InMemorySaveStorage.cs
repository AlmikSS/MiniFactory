using MiniFactory.Persistence.Interfaces;

namespace MiniFactory.Persistence.Logic
{
    public sealed class InMemorySaveStorage : ISaveStorage
    {
        private string _content;

        public bool Exists() => !string.IsNullOrEmpty(_content);
        public string Read() => _content ?? string.Empty;
        public void Write(string content) => _content = content;
        public void Delete() => _content = null;
    }
}