using System;
using System.Collections.Generic;
using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using MiniFactory.Analytics;
using MiniFactory.Gameplay.Events;

namespace MiniFactory.Purchases
{
    public sealed class StubPurchaseGateway : IPurchaseGateway
    {
        private readonly PurchaseConfig _config;
        private readonly EventBus _eventBus;
        private readonly AnalyticsService _analytics;

        public bool IsInitialized { get; private set; }
        public bool IsAvailable => IsInitialized;
        public event Action Initialized;
        public IReadOnlyList<PurchaseProduct> Products => _config.Products;

        public StubPurchaseGateway(PurchaseConfig config, EventBus eventBus, AnalyticsService analytics)
        {
            _config = config;
            _eventBus = eventBus;
            _analytics = analytics;
        }

        public void Initialize()
        {
            IsInitialized = true;
            Log.Message("[Purchases] Stub gateway initialized.");
            Initialized?.Invoke();
        }

        public void Buy(string productId)
        {
            var product = _config.GetProduct(productId);
            if (product == null)
            {
                Fail(productId, "product_not_found");
                return;
            }

            Log.Message($"[Purchases] Stub purchase succeeded: {productId}");

            _eventBus.Invoke(new PurchaseSucceededEvent
            {
                ProductId = productId,
                RewardAmount = product.RewardAmount
            });

            _analytics.SendEvent(AnalyticsEventNames.PURCHASE_SUCCEEDED, new AnalyticsParametersBuilder()
                    .Add(AnalyticsParameterNames.PRODUCT_ID, productId)
                    .Add(AnalyticsParameterNames.AMOUNT, product.RewardAmount)
                    .Build());
        }

        private void Fail(string productId, string reason)
        {
            Log.Warning($"[Purchases] Stub purchase failed: {productId} ({reason})");

            _eventBus.Invoke(new PurchaseFailedEvent
            {
                ProductId = productId,
                Reason = reason
            });

            _analytics.SendEvent(AnalyticsEventNames.PURCHASE_FAILED, new AnalyticsParametersBuilder()
                    .Add(AnalyticsParameterNames.PRODUCT_ID, productId)
                    .Add(AnalyticsParameterNames.REASON, reason)
                    .Build());
        }
    }
}