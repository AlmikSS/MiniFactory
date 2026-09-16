using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.DI.Attributes;
using MiniFactory.Gameplay.Boost;
using MiniFactory.Gameplay.Economy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFactory.Gameplay.UI
{
    public sealed class BoostUI : MonoBehaviour, IConstructable, IDestroyable, IUITickable
    {
        [SerializeField] private Button _boostButton;
        [SerializeField] private TMP_Text _boostLabel;

        private BoostService _boostService;
        private WalletService _walletService;

        [Inject]
        private void Construct(BoostService boostService, WalletService walletService)
        {
            _boostService = boostService;
            _walletService = walletService;
        }

        public void OnConstruct()
        {
            _boostButton.onClick.AddListener(OnBoostClicked);
            _walletService.MoneyChangedEvent += OnMoneyChanged;
            Refresh();
        }

        public void OnDestroyed()
        {
            _boostButton.onClick.RemoveListener(OnBoostClicked);
            _walletService.MoneyChangedEvent -= OnMoneyChanged;
        }

        public void Tick(float deltaTime)
        {
            if (_boostService.IsActive)
                _boostLabel.text = $"Boost x{_boostService.Multiplier} — {_boostService.RemainingSeconds:0}s";
        }

        private void OnBoostClicked()
        {
            if (_boostService.TryStart())
                Refresh();
        }

        private void OnMoneyChanged(int balance)
        {
            Refresh();
        }

        private void Refresh()
        {
            if (!_boostService.IsEnabled)
            {
                _boostLabel.text = "Boost disabled";
                _boostButton.interactable = false;
                return;
            }

            if (_boostService.IsActive)
            {
                _boostLabel.text = $"Boost x{_boostService.Multiplier} — {_boostService.RemainingSeconds:0}s";
                _boostButton.interactable = false;
                return;
            }

            _boostLabel.text = $"Boost x{_boostService.Multiplier} ({_boostService.Price})";
            _boostButton.interactable = _boostService.CanStart();
        }
    }
}