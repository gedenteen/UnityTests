using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public bool SceneStack = true;

    private const string _sceneName = "TestFishNet_World1";
    private const int _worldInstanceCount = 2; // кол-во инстансов одной сцены на сервере

    // FishNet отличает stacked-инстансы одной сцены по Unity Scene.handle.
    // Здесь храним handles двух серверных инстансов, между которыми распределяем игроков.
    // Примечание: Юнити при загрузке возвращает handle типа int, который идентифицирует ЗАГРУЖЕННУЮ сцену.
    private readonly List<int> _stackedSceneHandles = new();
    // Если игрок подключился до завершения прогрева сцен, временно держим его здесь.
    private readonly HashSet<NetworkConnection> _pendingConnections = new();
    // Подключение должно оставаться в назначенном инстансе, даже если событие загрузки придёт повторно.
    private readonly Dictionary<NetworkConnection, int> _assignedHandleByConnection = new();

    private bool _isPrewarming;
    // Счётчик именно входов на сервер: 1-й игрок -> инстанс 0, 2-й -> инстанс 1, 3-й -> инстанс 0.
    private int _joinIndex;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
        InstanceFinder.SceneManager.OnClientLoadedStartScenes += SceneManagerOnOnClientLoadedStartScenes;
        InstanceFinder.ServerManager.OnRemoteConnectionState += ServerManager_OnRemoteConnectionState;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null)
        {
            InstanceFinder.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= SceneManagerOnOnClientLoadedStartScenes;
        }

        if (InstanceFinder.ServerManager != null)
        {
            InstanceFinder.ServerManager.OnRemoteConnectionState -= ServerManager_OnRemoteConnectionState;
        }
    }

    // Метод вызывается когда клиент подключился и прогрузил начальные сцены
    private void SceneManagerOnOnClientLoadedStartScenes(NetworkConnection connection, bool asServer)
    {
        if (!asServer)
            return;

        if (!SceneStack)
        {
            LoadScene(connection, 0);
            return;
        }

        if (!AreWorldInstancesReady())
        {
            // Сначала создаём два stacked-инстанса на сервере, а игрока загрузим после получения handles.
            _pendingConnections.Add(connection);
            PrewarmWorldInstances();
            return;
        }

        LoadConnectionIntoAssignedInstance(connection);
    }

    // "Прогревка" инстансов сцены: разрешаем стакать инстансы, загружаем (без ожидания) инстансы
    private void PrewarmWorldInstances()
    {
        if (_isPrewarming || AreWorldInstancesReady())
            return;

        _isPrewarming = true;

        for (int i = _stackedSceneHandles.Count; i < _worldInstanceCount; i++)
        {
            // Handle 0 означает "найти сцену по имени"; AllowStacking заставит FishNet создать новый инстанс.
            SceneLookupData lookupData = new SceneLookupData(0, _sceneName);
            SceneLoadData loadData = new SceneLoadData(lookupData);

            loadData.Options.AllowStacking = true;
            loadData.Options.AutomaticallyUnload = false;
            loadData.ReplaceScenes = ReplaceOption.None;

            // Загрузка без списка connections создаёт сцену только на сервере.
            // Клиенты позже будут загружены в конкретный handle через LoadConnectionScenes(connection, sld).
            InstanceFinder.SceneManager.LoadConnectionScenes(loadData);
        }
    }

    // Метод вызывается при загрузке сцены Фишнетом. Запоминаем handle сцены. Если все инстансы сцены
    // готовы, то подключаем игроков.
    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs args)
    {
        if (!args.QueueData.AsServer)
            return;

        if (!SceneStack)
            return;

        foreach (UnityEngine.SceneManagement.Scene loadedScene in args.LoadedScenes)
        {
            if (loadedScene.name != _sceneName)
                continue;

            // OnLoadEnd может приходить для разных очередей загрузки, поэтому не добавляем один handle повторно.
            if (_stackedSceneHandles.Contains(loadedScene.handle))
                continue;

            if (_stackedSceneHandles.Count >= _worldInstanceCount)
                break;

            _stackedSceneHandles.Add(loadedScene.handle);
            Debug.Log($"Registered {_sceneName} instance #{_stackedSceneHandles.Count}: handle={loadedScene.handle}");
        }

        if (!AreWorldInstancesReady())
            return;

        // Оба инстанса готовы: теперь можно отправить ожидающих игроков в назначенные сцены.
        _isPrewarming = false;
        FlushPendingConnections();
    }

    // Метод для "подвешенных" подключений. Если такие есть, то подключаем игроков на нужные сцены.
    private void FlushPendingConnections()
    {
        if (_pendingConnections.Count == 0)
            return;

        NetworkConnection[] pendingConnections = _pendingConnections.ToArray();
        _pendingConnections.Clear();

        foreach (NetworkConnection connection in pendingConnections)
        {
            if (connection == null || !connection.IsActive)
                continue;

            LoadConnectionIntoAssignedInstance(connection);
        }
    }

    // Метод для подключения игрока к нанзначенной ему сцене
    private void LoadConnectionIntoAssignedInstance(NetworkConnection connection)
    {
        if (!_assignedHandleByConnection.TryGetValue(connection, out int sceneHandle))
        {
            _joinIndex++;

            // Распределение по порядку входа: нечётные подключения в первый инстанс, чётные во второй.
            int instanceIndex = (_joinIndex - 1) % _worldInstanceCount;
            sceneHandle = _stackedSceneHandles[instanceIndex];

            _assignedHandleByConnection.Add(connection, sceneHandle);

            Debug.Log($"Connection {connection.ClientId} joined as #{_joinIndex}; assigned scene handle={sceneHandle}");
        }

        LoadScene(connection, sceneHandle);
    }

    // Метод для загрузки префаба игрока на сцену (которая определяется по sceneHandle)
    private void LoadScene(NetworkConnection connection, int sceneHandle)
    {
        // Если sceneHandle != 0, FishNet загрузит именно этот stacked-инстанс, а не создаст/выберет другой.
        SceneLookupData lookupData = new SceneLookupData(sceneHandle, _sceneName);
        SceneLoadData loadData = new SceneLoadData(lookupData);

        loadData.Options.AllowStacking = true;
        // Переносим все NetworkObject игрока в ту же сцену, чтобы SceneCondition видела их только внутри инстанса.
        loadData.MovedNetworkObjects = connection.Objects.ToArray();
        // Клиент оставляет только назначенный мир; стартовые/предыдущие сцены заменяются.
        loadData.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadConnectionScenes(connection, loadData);
    }

    private bool AreWorldInstancesReady()
    {
        return _stackedSceneHandles.Count >= _worldInstanceCount;
    }

    private void ServerManager_OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState != RemoteConnectionState.Stopped)
            return;

        // Назначение удаляем, но _joinIndex не уменьшаем: правило работает по порядку входов, а не по текущему онлайну.
        _pendingConnections.Remove(connection);
        _assignedHandleByConnection.Remove(connection);
    }
}