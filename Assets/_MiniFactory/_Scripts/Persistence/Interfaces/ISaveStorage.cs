namespace MiniFactory.Persistence.Interfaces
{
    public interface ISaveStorage
    {
        bool Exists();
        string Read();
        void Write(string content);
        void Delete();
    }
}