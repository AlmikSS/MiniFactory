using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using MiniFactory.Analytics;
using UnityEngine;

namespace MiniFactory.Purchases
{
    public static class PurchaseServiceFactory
    {
        public static IPurchaseGateway Create(PurchaseConfig config, EventBus eventBus, AnalyticsService analytics)
        {
            if (Application.isEditor && config.UseStubInEditor)
            {
                Log.Message("[Purchases] Using stub gateway (editor).");
                return new StubPurchaseGateway(config, eventBus, analytics);
            }

            Log.Message("[Purchases] Using Unity IAP gateway.");
            return new UnityIapGateway(config, eventBus, analytics);
        }
    }
}