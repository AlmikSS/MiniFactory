using System.Collections.Generic;
using KofeyekToolkit.Logging;

namespace MiniFactory.Analytics
{
    public sealed class AnalyticsService
    {
        private readonly List<IAnalyticsProvider> _providers = new();

        public IReadOnlyList<IAnalyticsProvider> Providers => _providers;

        public void Register(IAnalyticsProvider provider)
        {
            if (provider == null || _providers.Contains(provider))
                return;

            _providers.Add(provider);
            provider.Initialize();
            Log.Message($"Analytics provider registered: {provider.Name}");
        }

        public void Unregister(IAnalyticsProvider provider)
        {
            _providers.Remove(provider);
        }

        public void SendEvent(string eventName, IReadOnlyDictionary<string, object> parameters = null)
        {
            if (string.IsNullOrEmpty(eventName))
                return;

            parameters ??= EmptyParameters;

            foreach (var provider in _providers)
            {
                try
                {
                    provider.SendEvent(eventName, parameters);
                }
                catch (System.Exception ex)
                {
                    Log.Error($"Analytics provider '{provider.Name}' failed on '{eventName}': {ex.Message}");
                }
            }
        }

        private static readonly Dictionary<string, object> EmptyParameters = new();
    }
}