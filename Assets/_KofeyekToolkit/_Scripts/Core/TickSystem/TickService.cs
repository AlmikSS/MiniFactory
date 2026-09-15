using System.Collections.Generic;
using System.Diagnostics;
using KofeyekToolkit.Core.TickSystem.Interfaces;
using KofeyekToolkit.DevConsole;
using KofeyekToolkit.Logging;

namespace KofeyekToolkit.Core.TickSystem
{
    /// <summary>
    /// Централизованный сервис управления детерминированными тиками с разделением по каналам.
    /// Обеспечивает фиксированный шаг для игровой логики (Gameplay) и
    /// плавающий шаг для систем, UI и презентационных слоёв.
    /// </summary>
    /// <remarks>
    /// Сервис поддерживает очередь регистрации/отмены регистрации тикеров,
    /// паузу игровой логики и измерение производительности.
    /// </remarks>
    public sealed class TickService
    {
        private int _tickCounter;
        private float _accumulator;
        private float _tickTimer;
        private bool _ticksStarted;
        private readonly Stopwatch _stopwatch = new();
        
        private readonly List<ISystemTickable> _systems = new();
        private readonly List<IGameplayTickable> _gameplay = new();
        private readonly List<IUITickable> _ui = new();
        private readonly List<IPresentationTickable> _presentation = new();
        
        private readonly Queue<ITickable> _registerQueue = new();
        private readonly Queue<ITickable> _unregisterQueue = new();

        /// <summary>
        /// Целевой тикрейт игровой логики.
        /// </summary>
        public int TargetTickRate { get; private set; }
        /// <summary>
        /// Фактически измеренный тикрейт игровой логики.
        /// </summary>
        public int RealTickRate { get; private set; }
        /// <summary>
        /// Время выполнения игрового тика в миллисекундах.
        /// </summary>
        public float TickExecutionTimeMs { get; private set; }
        /// <summary>
        /// Интервал между игровыми тиками в секундах.
        /// </summary>
        public float TickInterval { get; private set; }
        /// <summary>
        /// Признак паузы игрового канала тиков.
        /// </summary>
        public bool IsGameplayPaused { get; private set; }

        /// <summary>
        /// Определяет, выводит ли сервис диагностические сообщения.
        /// </summary>
        public bool IsLoggingEnabled { get; private set; } = true;

        public TickService() { }
        
        /// <summary>
        /// Конструктор сервиса тиков.
        /// </summary>
        /// <param name="tickRate">Целевое количество тиков в секунду для игровой логики.</param>
        /// <summary>
        /// Предоставляет API-член <c>TickService</c>.
        /// </summary>
        public TickService(int tickRate)
        {
            ChangeTickRate(tickRate);
            Message($"Initialized with target tick rate {TargetTickRate}.");
        }

        /// <summary>
        /// Включает или выключает диагностическое логирование этого сервиса.
        /// </summary>
        public void EnableLogging(bool enable)
        {
            IsLoggingEnabled = enable;
        }

        internal void EnableTicking(bool enable)
        {
            _ticksStarted = enable;
            Message(enable ? "Ticks started." : "Ticks stopped.");
        }
        
        /// <summary>
        /// Помещает тикер в очередь на регистрацию.
        /// Регистрация будет применена в ближайшем кадре.
        /// </summary>
        /// <param name="tickable">Объект, реализующий интерфейс ITickable.</param>
        /// <summary>
        /// Регистрирует сервис или обработчик в контейнере.
        /// </summary>
        public void Register(ITickable tickable)
        {
            if (tickable == null)
            {
                Warning("Skipped registration of a null tickable.");
                return;
            }

            _registerQueue.Enqueue(tickable);
            Message($"Queued registration for {tickable.GetType().Name}.");
        }
        /// <summary>
        /// Помещает тикер в очередь на отмену регистрации.
        /// Отмена будет применена в ближайшем кадре.
        /// </summary>
        /// <param name="tickable">Объект, реализующий интерфейс ITickable.</param>
        /// <summary>
        /// Удаляет обработчик из подписок на событие.
        /// </summary>
        public void Unregister(ITickable tickable)
        {
            if (tickable == null)
            {
                Warning("Skipped unregistration of a null tickable.");
                return;
            }

            _unregisterQueue.Enqueue(tickable);
            Message($"Queued unregistration for {tickable.GetType().Name}.");
        }

        /// <summary>
        /// Приостанавливает выполнение игровых тиков (Gameplay).
        /// Системные, UI и презентационные тики продолжают работу.
        /// </summary>
        public void Pause()
        {
            if (IsGameplayPaused)
            {
                Warning("Gameplay ticks are already paused.");
                return;
            }

            IsGameplayPaused = true;
            Message("Gameplay ticks paused.");
        }
        /// <summary>
        /// Возобновляет выполнение игровых тиков (Gameplay).
        /// </summary>
        public void Resume()
        {
            if (!IsGameplayPaused)
            {
                Warning("Gameplay ticks are already running.");
                return;
            }

            IsGameplayPaused = false;
            Message("Gameplay ticks resumed.");
        }

        /// <summary>
        /// Основной цикл обновления. Вызывается каждый кадр.
        /// Обрабатывает очереди регистрации, выполняет тики по каналам
        /// с учетом паузы и фиксированного шага игровой логики.
        /// </summary>
        /// <param name="unscaledDeltaTime">Неизменяемая дельта времени (не зависит от Time.timeScale).</param>
        internal void Update(float unscaledDeltaTime)
        {
            if (!_ticksStarted)
                return;
            
            _tickTimer += unscaledDeltaTime;
            if (_tickTimer >= 1f)
            {
                RealTickRate = _tickCounter;
                _tickCounter = 0;
                _tickTimer -= 1f;
            }
            
            ReleaseRegisterQueue();
            ReleaseUnregisterQueue();
            
            Tick(_systems.ToArray(), unscaledDeltaTime);
            Tick(_ui.ToArray(), unscaledDeltaTime);

            if (!IsGameplayPaused)
            {
                _accumulator += unscaledDeltaTime;
            
                while (_accumulator >= TickInterval)
                {
                    _stopwatch.Restart();
                
                    Tick(_gameplay.ToArray(), TickInterval);
                    _tickCounter++;
                
                    _stopwatch.Stop();
                    TickExecutionTimeMs = (float)_stopwatch.Elapsed.TotalMilliseconds;
                
                    _accumulator -= TickInterval;
                }
                
                Tick(_presentation.ToArray(), unscaledDeltaTime);
            }
        }

        /// <summary>
        /// Обрабатывает очередь регистрации тикеров.
        /// Добавляет тикеры в соответствующие списки по типу интерфейса.
        /// </summary>
        private void ReleaseRegisterQueue()
        {
            while (_registerQueue.Count > 0)
            {
                var tickable = _registerQueue.Dequeue();
                
                switch (tickable)
                {
                    case ISystemTickable systemTickable:
                        _systems.Add(systemTickable);
                        break;
                    case IGameplayTickable gameplayTickable:
                        _gameplay.Add(gameplayTickable);
                        break;
                    case IUITickable uiTickable:
                        _ui.Add(uiTickable);
                        break;
                    case IPresentationTickable presentationTickable:
                        _presentation.Add(presentationTickable);
                        break;
                }
            }
        }

        /// <summary>
        /// Обрабатывает очередь отмены регистрации тикеров.
        /// Удаляет тикеры из соответствующих списков по типу интерфейса.
        /// </summary>
        private void ReleaseUnregisterQueue()
        {
            while (_unregisterQueue.Count > 0)
            {
                var tickable = _unregisterQueue.Dequeue();
                
                switch (tickable)
                {
                    case ISystemTickable systemTickable:
                        if (_systems.Contains(systemTickable))
                            _systems.Remove(systemTickable);
                        break;
                    case IGameplayTickable gameplayTickable:
                        if (_gameplay.Contains(gameplayTickable))
                            _gameplay.Remove(gameplayTickable);
                        break;
                    case IUITickable uiTickable:
                        if (_ui.Contains(uiTickable))
                            _ui.Remove(uiTickable);
                        break;
                    case IPresentationTickable presentationTickable:
                        if (_presentation.Contains(presentationTickable))
                            _presentation.Remove(presentationTickable);
                        break;
                }
            }
        }

        /// <summary>
        /// Выполняет тик для массива тикеров с переданной дельтой времени.
        /// </summary>
        /// <param name="tickables">Массив объектов, реализующих ITickable.</param>
        /// <param name="deltaTime">Дельта времени для данного тика.</param>
        private void Tick(ITickable[] tickables, float deltaTime)
        {
            foreach (var tickable in tickables)
            {
                tickable.Tick(deltaTime);
            }
        }

        /// <summary>
        /// Изменяет целевой тикрейт игровой логики.
        /// Пересчитывает интервал тика и сбрасывает аккумулятор.
        /// Доступна через консольную команду "change_tick_rate".
        /// </summary>
        /// <param name="newTickRate">Новое количество тиков в секунду.</param>
        [Command("change_tick_rate", "Изменяет целевой тикрейт игровой логики.")]
        private void ChangeTickRate(int newTickRate)
        {
            if (newTickRate <= 0)
            {
                Error($"Cannot set a non-positive tick rate: {newTickRate}.");
                return;
            }

            TargetTickRate = newTickRate;
            TickInterval = 1f / newTickRate;
            _accumulator = 0f;
            Message($"Target tick rate changed to {newTickRate}.");
        }

        private void Message(string message)
        {
            if (IsLoggingEnabled)
                Log.Message(message);
        }

        private void Warning(string message)
        {
            if (IsLoggingEnabled)
                Log.Warning(message);
        }

        private void Error(string message)
        {
            if (IsLoggingEnabled)
                Log.Error(message);
        }
    }
}
