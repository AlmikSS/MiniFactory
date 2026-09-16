using System.Collections.Generic;

namespace MiniFactory.Analytics
{
    public sealed class AnalyticsParametersBuilder
    {
        private readonly Dictionary<string, object> _parameters = new();

        public AnalyticsParametersBuilder Add(string key, object value)
        {
            _parameters[key] = value;
            return this;
        }

        public IReadOnlyDictionary<string, object> Build() => _parameters;
    }
}