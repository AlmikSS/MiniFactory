using System;
using System.Collections.Generic;
using KofeyekToolkit.Events;
using KofeyekToolkit.Logging;
using MiniFactory.Analytics;
using MiniFactory.Gameplay.Events;
using UnityEngine.Purchasing;

namespace MiniFactory.Purchases
{
    public sealed class UnityIapGateway : IPurchaseGateway, IDetailedStoreListener
    {
        private readonly PurchaseConfig _config;
        private readonly EventBus _eventBus;
        private readonly AnalyticsService _analytics;

        private IStoreController _storeController;
        private IExtensionProvider _extensionProvider;

        public bool IsInitialized { get; private set; }
        public bool IsAvailable => _storeController != null;
        public event Action Initialized;
        public IReadOnlyList<PurchaseProduct> Products => _config.Products;

        public UnityIapGateway(PurchaseConfig config, EventBus eventBus, AnalyticsService analytics)
        {
            _config = config;
            _eventBus = eventBus;
            _analytics = analytics;
        }

        public void Initialize()
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            foreach (var product in _config.Products)
            {
                var productType = ConvertType(product.Type);
                builder.AddProduct(product.Id, productType);
            }

            UnityPurchasing.Initialize(this, builder);
            Initialized?.Invoke();
        }

        public void Buy(string productId)
        {
            if (!IsAvailable)
            {
                Fail(productId, "iap_not_available");
                return;
            }

            var product = _storeController.products.WithID(productId);
            if (product == null || !product.availableToPurchase)
            {
                Fail(productId, "product_not_available");
                return;
            }

            _storeController.InitiatePurchase(product);
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            _storeController = controller;
            _extensionProvider = extensions;
            IsInitialized = true;
            Log.Message("[Purchases] Unity IAP initialized.");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            IsInitialized = true;
            Log.Error($"[Purchases] IAP init failed: {error}");
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            IsInitialized = true;
            Log.Error($"[Purchases] IAP init failed: {error} — {message}");
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var productId = args.purchasedProduct.definition.id;
            var product = _config.GetProduct(productId);

            if (product == null)
            {
                Fail(productId, "product_not_in_config");
                return PurchaseProcessingResult.Complete;
            }

            Log.Message($"[Purchases] Purchase succeeded: {productId}");

            _eventBus.Invoke(new PurchaseSucceededEvent
            {
                ProductId = productId,
                RewardAmount = product.RewardAmount
            });

            _analytics.SendEvent(AnalyticsEventNames.PURCHASE_SUCCEEDED, new AnalyticsParametersBuilder()
                    .Add(AnalyticsParameterNames.PRODUCT_ID, productId)
                    .Add(AnalyticsParameterNames.AMOUNT, product.RewardAmount)
                    .Build());

            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            var productId = product != null ? product.definition.id : "unknown";
            var reason = failureDescription != null ? failureDescription.reason.ToString() : "unknown";
            Fail(productId, reason);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            var productId = product != null ? product.definition.id : "unknown";
            Fail(productId, failureReason.ToString());
        }

        private void Fail(string productId, string reason)
        {
            Log.Warning($"[Purchases] Purchase failed: {productId} ({reason})");

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

        private static ProductType ConvertType(PurchaseProductType type)
        {
            return type switch
            {
                PurchaseProductType.Consumable => ProductType.Consumable,
                PurchaseProductType.NonConsumable => ProductType.NonConsumable,
                PurchaseProductType.Subscription => ProductType.Subscription,
                _ => ProductType.Consumable
            };
        }
    }
}