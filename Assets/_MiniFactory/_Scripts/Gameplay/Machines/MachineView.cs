using KofeyekToolkit.Core.LifeCycle.Core.Interfaces;
using KofeyekToolkit.DI.Attributes;
using KofeyekToolkit.Events;
using MiniFactory.Gameplay.Events;
using MiniFactory.Gameplay.Machines;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MiniFactory.Gameplay.UI
{
    public sealed class MachineView : MonoBehaviour, IConstructable, IDestroyable, IPointerClickHandler
    {
        [SerializeField] private Machine _machine;
        [SerializeField] private TMP_Text _statusLabel;

        private EventBus _eventBus;

        [Inject]
        private void Construct(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void OnConstruct()
        {
            _eventBus.Register<MachineUnlockedEvent>(OnMachineUnlocked);
            _eventBus.Register<MachineUpgradedEvent>(OnMachineUpgraded);
            Refresh();
        }

        public void OnDestroyed()
        {
            _eventBus.Unregister<MachineUnlockedEvent>(OnMachineUnlocked);
            _eventBus.Unregister<MachineUpgradedEvent>(OnMachineUpgraded);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_machine == null || _eventBus == null)
                return;

            _eventBus.Invoke(new MachineClickedEvent { Machine = _machine });
        }

        private void OnMachineUnlocked(MachineUnlockedEvent evt)
        {
            if (_machine != null && evt.MachineId == _machine.Id)
                Refresh();
        }

        private void OnMachineUpgraded(MachineUpgradedEvent evt)
        {
            if (_machine != null && evt.MachineId == _machine.Id)
                Refresh();
        }

        private void Refresh()
        {
            if (_machine == null || _statusLabel == null)
                return;

            if (_machine.IsLocked)
            {
                _statusLabel.text = $"Locked\n{_machine.UnlockCost}";
                return;
            }

            _statusLabel.text = $"Lv.{_machine.Level}\n+{_machine.Productivity}/{_machine.AddMoneyDelay:0.#}s";
        }
    }
}