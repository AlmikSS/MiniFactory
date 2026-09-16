using System;

namespace MiniFactory.Purchases
{
    [Serializable]
    public sealed class PurchaseProduct
    {
        public string Id;
        public PurchaseProductType Type;
        public int RewardAmount;
    }
}