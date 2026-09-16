using System.Collections.Generic;
using UnityEngine;

namespace MiniFactory.Purchases
{
    [CreateAssetMenu(fileName = "PurchaseConfig", menuName = "Configs/PurchaseConfig")]
    public sealed class PurchaseConfig : ScriptableObject
    {
        [SerializeField] private bool _useStubInEditor = true;
        [SerializeField] private List<PurchaseProduct> _products = new();

        public bool UseStubInEditor => _useStubInEditor;
        public IReadOnlyList<PurchaseProduct> Products => _products;

        public PurchaseProduct GetProduct(string id)
        {
            foreach (var product in _products)
            {
                if (product.Id == id)
                    return product;
            }

            return null;
        }
    }
}