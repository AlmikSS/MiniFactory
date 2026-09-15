using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.DI.Attributes;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Machines;
using TMPro;
using UnityEngine;

namespace MiniFactory.Gameplay.UI
{
    public sealed class GameplayHUD : MonoBehaviour, IConstructable, IDestroyable, IUITickable
    {
        [SerializeField] private TMP_Text _balanceLabel;
        [SerializeField] private TMP_Text _incomeLabel;

        private WalletService _walletService;
        private MachineController _machineController;
        private float _refreshTimer;
        private const float REFRESH_INTERVAL = 0.25f;

        [Inject]
        private void Construct(WalletService walletService, MachineController machineController)
        {
            _walletService = walletService;
            _machineController = machineController;
        }

        public void OnConstruct()
        {
            _walletService.MoneyChangedEvent += OnMoneyChanged;
            RefreshAll();
        }

        public void OnDestroyed()
        {
            _walletService.MoneyChangedEvent -= OnMoneyChanged;
        }

        public void Tick(float deltaTime)
        {
            _refreshTimer += deltaTime;
            if (_refreshTimer < REFRESH_INTERVAL)
                return;

            _refreshTimer = 0f;
            RefreshIncome();
        }

        private void OnMoneyChanged(int balance)
        {
            _balanceLabel.text = $"Balance: {balance}";
            RefreshIncome();
        }

        private void RefreshAll()
        {
            _balanceLabel.text = $"Balance: {_walletService.Balance}";
            RefreshIncome();
        }

        private void RefreshIncome()
        {
            var income = _machineController.GetTotalIncomePerSecond();
            _incomeLabel.text = $"Income: {income}/s";
        }
    }
}