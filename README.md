\# Mini Factory



Тестовое задание для Unity Developer: мобильная idle-игра про фабрику.



\- \*\*Unity:\*\* 6000.3.22f1

\- \*\*Платформа:\*\* Android

\- \*\*IAP:\*\* Unity In-App Purchasing v5 (Fake Store)

\- \*\*Время работы:\*\* \[N] часов



\## 1. Запуск и Android build



\### Запуск в редакторе

1\. Открыть проект в Unity 6000.3.22f1.

2\. Открыть сцену `Assets/\_MiniFactory/Scenes/Boot.unity`.

3\. Нажать \*\*Play\*\*. `AppBootstrap` сам инициализирует системы и загрузит Gameplay-сцену.



\### Тесты

`Window → General → Test Runner → EditMode → Run All`.



\### Android build

1\. `File → Build Settings` → Platform: \*\*Android\*\* → `Switch Platform`.

2\. `Player Settings`: Package Name, Minimum API 22+.

3\. `Build and Run`.



\### IAP

\- В редакторе по умолчанию активен `StubPurchaseGateway` (см. `PurchaseConfig → Use Stub In Editor`) — покупки мгновенно успешны.

\- Для Fake Store: снять `Use Stub In Editor`, открыть `Window → Unity IAP → Fake Store`, выбрать \*\*Fake Store\*\*.



\## 2. Основные системы и архитектурные решения



\*\*Основа — собственный тулкит (`KofeyekToolkit`):\*\*

`DIContainer` (DI), `TickService` (централизованные тики), `SpawnService` (спавн с жизненным циклом), `EventBus` (шина игровых событий), `SceneSwitcher` (загрузка сцен), `Log` (логирование).



\*\*Порядок инициализации:\*\*

\- `MainBootstrap` (Boot-сцена) — регистрирует конфиги, `SaveService`, `WalletService`, `BoostService`, `AnalyticsService`, `IPurchaseGateway`, восстанавливает сохранения.

\- `GameplayBootstrap` (Gameplay-сцена) — создаёт `MachineController`, `OfflineProgressService`, регистрирует их.



\*\*Ключевые системы:\*\*

\- `WalletService` — баланс, `Add`/`TrySpend`, событие `MoneyChanged`, сохраняется.

\- `MachineController` — `TryUnlock`/`TryUpgrade`, `GetTotalIncomePerSecond`, шлёт `MachineUnlockedEvent`/`MachineUpgradedEvent`.

\- `BoostService` — покупка и активация временного множителя, работает по UTC.

\- `OfflineProgressService` — доход за offline-период с учётом boost и лимита.

\- `SaveService` — JSON в `persistentDataPath`, автосейв раз в 5 сек, при сворачивании и выходе.

\- `AnalyticsService` — фасад над `IAnalyticsProvider`, легко подключаются реальные SDK.

\- `IPurchaseGateway` — абстракция покупок (`UnityIapGateway`, `StubPurchaseGateway`).



\*\*Решения:\*\*

\- Игровая логика не использует `Update` — только `TickService`.

\- Gameplay не зависит от Unity IAP API и от `AnalyticsService` — работает через `IPurchaseGateway` и `EventBus`.

\- Boost и offline работают по UTC — переживают сворачивание приложения.

\- Экономика в ScriptableObject'ах (`EconomyConfig`, `MachineConfig`, `BoostConfig`, `PurchaseConfig`).

\- Формулы вынесены в чистые static-классы (`MachineMath`, `BoostMath`, `OfflineMath`) и покрыты тестами.



\## 3. Packages / SDK



\- \*\*Unity In-App Purchasing\*\* (`com.unity.purchasing` v5.x)

\- \*\*TextMeshPro\*\* (встроен в Unity 6)

\- \*\*KofeyekToolkit\*\* — собственный тулкит (DI, тики, спавн, EventBus, сцены, логирование)

\- \*\*NUnit\*\* (входит в Unity Test Framework)



\## 4. Известные ограничения



\- Boost нельзя продлить, пока активен.

\- Нет серверной валидации чеков IAP.

\- Analytics-события не буферизуются при отсутствии сети.

\- Firebase Remote Config не подключён.

\- UI минимальный, без анимаций.

\- Реальный Analytics Provider (AppMetrica/Firebase) не подключён.



\## 5. Устройство для проверки



\- Редактор: Unity 6000.3.22f1, Windows.

\- Android: \[Sumsung Galaxy A56].

