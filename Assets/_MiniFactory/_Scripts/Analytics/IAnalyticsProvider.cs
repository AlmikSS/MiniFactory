using System.Collections.Generic;

namespace MiniFactory.Analytics
{
    public interface IAnalyticsProvider
    {
        string Name { get; }
        void Initialize();
        void SendEvent(string eventName, IReadOnlyDictionary<string, object> parameters);
    }
}