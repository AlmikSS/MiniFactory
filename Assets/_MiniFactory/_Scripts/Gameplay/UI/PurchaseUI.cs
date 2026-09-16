using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using KofeyekToolkit.DI.Attributes;
using MiniFactory.Purchases;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFactory.Gameplay.UI
{
    public sealed class PurchaseUI : MonoBehaviour, IConstructable, IDestroyable
    {
        [SerializeField] private string _productId = "coins_pack_small";
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _label;

        private IPurchaseGateway _gateway;

        [Inject]
        private void Construct(IPurchaseGateway gateway)
        {
            _gateway = gateway;
        }
        
        public void OnConstruct()
        {
            _buyButton.onClick.AddListener(OnBuyClicked);
            _gateway.Initialized += OnGatewayInitialized;
            Refresh();
        }

        public void OnDestroyed()
        {
            _buyButton.onClick.RemoveListener(OnBuyClicked);
            _gateway.Initialized -= OnGatewayInitialized;
        }

        private void OnGatewayInitialized()
        {
            Refresh();
        }

        private void OnBuyClicked()
        {
            _gateway.Buy(_productId);
        }

        private void Refresh()
        {
            if (!_gateway.IsAvailable)
            {
                _label.text = "Store unavailable";
                _buyButton.interactable = false;
                return;
            }

            var product = FindProduct(_productId);
            if (product == null)
            {
                _label.text = "Product missing";
                _buyButton.interactable = false;
                return;
            }

            _label.text = $"Buy {product.RewardAmount} coins";
            _buyButton.interactable = true;
        }

        private PurchaseProduct FindProduct(string id)
        {
            var products = _gateway.Products;
            foreach (var product in products)
            {
                if (product.Id == id)
                    return product;
            }

            return null;
        }
    }
}