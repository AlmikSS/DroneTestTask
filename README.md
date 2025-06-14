## DroneTestTask

Реализовано:
1. Сцена с двумя базами напротив друг друга
2. Поведение дронов
3. Простейщая логика обхода дронов
4. Интерфейс натсроек
5. Спавнер ресурсов
6. Логика ресурсов

## Архитектура
# 1. Общее описание
Проект представляет собой симуляцию «сбора ресурсов» с помощью дронов в Unity. При старте сцены инициализируются настройки (интервал спавна, скорость дронов, их число и отображение путей), после чего периодически создаются объекты-ресурсы. Дроны, управляемые NavMeshAgent, бесконечно ищут свободные ресурсы, подбирают их за заданное время и возвращают в свою базу. Базы отслеживают и отображают счёт доставленных ресурсов.


# 2. Компоненты и логика (ключевые)
Bootstrap.cs
При старте вызывает инициализацию GameSettings и запускает ResourcesSpawner с интервалом из настроек. Подписывает базы и спавнер на события изменения параметров.

GameSettings.cs
UI (TMP_InputField, Slider) для правки интервала спавна, скорости дронов, их количества и флага «показывать путь». При изменении каждого параметра рассылает событие (OnIntervalChanged, OnDroneSpeedChanged, OnDroneCountChanged, OnShowPathChanged).

ResourcesSpawner.cs
Через Coroutine с текущим интервалом берёт объекты Resource из ObjectPool, размещает их в случайной области и хранит в списке. Подписывается на OnPickUpEvent от Resource, чтобы удалять «забранные» ресурсы из активного списка.

Resource.cs
Представляет «ресурс»: можно «забронировать» (Book), после чего при Pickup вызывает событие и возвращает себя в пул.

Base.cs
На старте создаёт указанное число дронов нужной команды, хранит ссылку на спавнер и сам счётчик ресурсов. При поступлении события о доставке от дрона увеличивает свой счёт и проигрывает VFX. Также реагирует на изменения скорости дронов и их количества из GameSettings.

Drone.cs
С NavMeshAgent и FSM (GoToRes → PickUpRes → GoToBase). Находит ближайший незабронированный ресурс, бронирует его, летит к нему, ждёт времени сборки, затем возвращается в базу. После доставки база инкрементирует счёт. При изменении скорости или флага «показывать путь» сразу реагирует через подписку на события.

DroneAvoidance.cs и AgentPathRenderer.cs
DroneAvoidance назначает случайный avoidancePriority у NavMeshAgent, чтобы дроны плавно избегали друг друга. AgentPathRenderer рисует линию маршрута по точкам agent.path.corners при включённом флаге отображения.

ObjectPool.cs
Синглтон, в Awake заполняет очереди для каждого типа (PoolType.Resource) заданным количеством префабов. Методы GetFromPool/RemoveToPool выдают и возвращают объекты без Instantiate/Destroy.


# 3. Используемые паттерны, инструменты, техники
1. Event-Driven
GameSettings рассылает события через C# Action<T>, на которые подписаны спавнер, базы и дроны. Это позволяет менять параметры в runtime без жёсткой связи.
2. Object Pooling
ObjectPool с Dictionary<PoolType, Queue<GameObject>> для переиспользования ресурсов и снижения затрат на Instantiate/Destroy.
3. Singleton
ObjectPool реализован как синглтон для глобального доступа.
4. FSM (State Machine) в дроне
Переключение состояний GoToRes → PickUpRes → GoToBase организовано внутри Drone.cs.
5. Enum-ы (Team, DroneState, PoolType)
Четко описывают команды, состояния дронов и типы пула, что упрощает логику и читаемость.
6. MVC-подобная схема (очень упрощённая)
View/UI: GameSettings (InputField, Sliders), ScoreShower (отображение счёта ресурсов).
Model/Logic: Base, Drone, Resource, ResourcesSpawner.
Controller/Оркестратор: Bootstrap связывает их вместе.

Сумарно потраченное время: 3 часа 56 минутм 55 секунд

# Демонстрация

https://github.com/user-attachments/assets/5471fd8d-c039-4a68-a028-ce054a6bb8ff

