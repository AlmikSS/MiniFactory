using TriInspector;
using UnityEngine;

namespace KofeyekToolkit.Logging
{
    [CreateAssetMenu(fileName = "LogOptions", menuName = "KofeyekToolkit/LogOptions")]
    [DeclareFoldoutGroup("dev_console", Title = "$" + nameof(DevConsole))]
    [DeclareFoldoutGroup("tick_service", Title = "$" + nameof(TickService))]
    [DeclareFoldoutGroup("spawn_service", Title = "$" + nameof(SpawnService))]
    [DeclareFoldoutGroup("scene", Title = "$" + nameof(Scene))]
    [DeclareFoldoutGroup("event_bus", Title = "$" + nameof(EventBus))]
    [DeclareFoldoutGroup("di", Title = "$" + nameof(DI))]
    public sealed class LogOptions : ScriptableObject
    {
        [Group("dev_console")] [SerializeField] private bool _showCommandsRegistryDebug = true;
        [Group("dev_console")] [SerializeField] private bool _showDevUIDebug;
        [Group("dev_console")] [SerializeField] private bool _showCommandExecutorDebug =  true;
        
        [Group("tick_service")] [SerializeField] private bool _showTickServiceDebug = true;
        
        [Group("spawn_service")] [SerializeField] private bool _showSpawnServiceDebug = true;
        
        [Group("scene")] [SerializeField] private bool _showSceneSwitcherDebug = true;
        
        [Group("event_bus")] [SerializeField] private bool _showEventBusDebug = true;
        
        [Group("di")] [SerializeField] private bool _showDiDebug = true;
        
        public string DevConsole => "DevConsole";
        public string TickService => "TickService";
        public string SpawnService => "SpawnService";
        public string Scene =>  "Scene";
        public string EventBus => "EventBus";
        public string DI => "DI";

        public bool ShowCommandsRegistryDebug => _showCommandsRegistryDebug;
        public bool ShowDevUIDebug => _showDevUIDebug;
        public bool ShowCommandExecutorDebug => _showCommandExecutorDebug;
        public bool ShowTickServiceDebug => _showTickServiceDebug;
        public bool ShowSpawnServiceDebug => _showSpawnServiceDebug;
        public bool ShowSceneSwitcherDebug => _showSceneSwitcherDebug;
        public bool ShowEventBusDebug => _showEventBusDebug;
        public bool ShowDiDebug => _showDiDebug;
    }
}