using System.Collections.Generic;
using System.Text;
using KofeyekToolkit.Logging;

namespace MiniFactory.Analytics
{
    public sealed class DebugAnalyticsProvider : IAnalyticsProvider
    {
        public string Name => "Debug";

        public void Initialize()
        {
            Log.Message("[Analytics] Debug provider initialized.");
        }

        public void SendEvent(string eventName, IReadOnlyDictionary<string, object> parameters)
        {
            var builder = new StringBuilder();
            builder.Append("[Analytics] ").Append(eventName);

            if (parameters != null && parameters.Count > 0)
            {
                builder.Append(" { ");
                var first = true;
                foreach (var pair in parameters)
                {
                    if (!first)
                        builder.Append(", ");

                    builder.Append(pair.Key).Append('=').Append(pair.Value);
                    first = false;
                }
                builder.Append(" }");
            }

            Log.Message(builder.ToString());
        }
    }
}