using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.Events;
using MiniFactory.Gameplay.Economy;
using MiniFactory.Gameplay.Events;
using MiniFactory.Gameplay.Machines;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MiniFactory.Gameplay.UI
{
    public sealed class MachinePanelUI : MonoBehaviour, IConstructable, IDestroyable
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private TMP_Text _statsLabel;
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _actionLabel;
        [SerializeField] private Button _closeButton;

        private WalletService _walletService;
        private MachineController _machineController;
        private EventBus _eventBus;
        private Machine _selected;

        [Inject]
        private void Construct(WalletService walletService, MachineController machineController, EventBus eventBus)
        {
            _walletService = walletService;
            _machineController = machineController;
            _eventBus = eventBus;
        }

        public void OnConstruct()
        {
            _eventBus.Register<MachineClickedEvent>(OnMachineClicked);
            _walletService.MoneyChangedEvent += OnMoneyChanged;

            _actionButton.onClick.AddListener(OnActionClicked);
            _closeButton.onClick.AddListener(Close);

            Close();
        }

        public void OnDestroyed()
        {
            _eventBus.Unregister<MachineClickedEvent>(OnMachineClicked);
            _walletService.MoneyChangedEvent -= OnMoneyChanged;

            _actionButton.onClick.RemoveListener(OnActionClicked);
            _closeButton.onClick.RemoveListener(Close);
        }

        private void OnMachineClicked(MachineClickedEvent evt)
        {
            _selected = evt.Machine;
            if (_selected == null)
            {
                Close();
                return;
            }

            _root.SetActive(true);
            Refresh();
        }

        private void OnMoneyChanged(int balance)
        {
            if (_selected != null && _root.activeSelf)
                Refresh();
        }

        private void OnActionClicked()
        {
            if (_selected == null)
                return;

            if (_selected.IsLocked)
                _machineController.TryUnlock(_selected);
            else
                _machineController.TryUpgrade(_selected);

            Refresh();
        }

        private void Refresh()
        {
            if (_selected == null)
                return;

            _titleLabel.text = _selected.DisplayName;

            if (_selected.IsLocked)
            {
                _statsLabel.text = $"Locked\nCost: {_selected.UnlockCost}";
                _actionLabel.text = $"Unlock ({_selected.UnlockCost})";
                _actionButton.interactable = _walletService.Balance >= _selected.UnlockCost;
                return;
            }

            _statsLabel.text =
                $"Level: {_selected.Level}\n" +
                $"Productivity: {_selected.Productivity} / {_selected.AddMoneyDelay:0.#}s";
            _actionLabel.text = $"Upgrade ({_selected.NextLevelPrice})";
            _actionButton.interactable = _walletService.Balance >= _selected.NextLevelPrice;
        }

        private void Close()
        {
            _selected = null;
            _root.SetActive(false);
        }
    }
}