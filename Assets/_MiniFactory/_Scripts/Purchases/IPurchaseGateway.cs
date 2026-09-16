using System;
using System.Collections.Generic;

namespace MiniFactory.Purchases
{
    public interface IPurchaseGateway
    {
        bool IsInitialized { get; }
        bool IsAvailable { get; }
        event Action Initialized;
        IReadOnlyList<PurchaseProduct> Products { get; }

        void Initialize();
        void Buy(string productId);
    }
}