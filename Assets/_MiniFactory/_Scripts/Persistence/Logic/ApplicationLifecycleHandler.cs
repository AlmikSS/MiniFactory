using KofeyekToolkit.DI.Attributes;
using MiniFactory.Persistence.Logic;
using UnityEngine;

namespace MiniFactory.Persistence
{
    public sealed class ApplicationLifecycleHandler : MonoBehaviour
    {
        private SaveService _saveService;

        [Inject]
        private void Construct(SaveService saveService)
        {
            _saveService = saveService;
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause && _saveService != null)
                _saveService.Save();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus && _saveService != null)
                _saveService.Save();
        }

        private void OnApplicationQuit()
        {
            _saveService?.Save();
        }
    }
}