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
    private const int _worldInstanceCount = 2;

    private readonly List<int> _stackedSceneHandles = new();
    private readonly HashSet<NetworkConnection> _pendingConnections = new();
    private readonly Dictionary<NetworkConnection, int> _assignedHandleByConnection = new();

    private bool _isPrewarming;
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
            InstanceFinder.ServerManager.OnRemoteConnectionState -= ServerManager_OnRemoteConnectionState;
        }
    }

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
            _pendingConnections.Add(connection);
            PrewarmWorldInstances();
            return;
        }

        LoadConnectionIntoAssignedInstance(connection);
    }

    private void PrewarmWorldInstances()
    {
        if (_isPrewarming || AreWorldInstancesReady())
            return;

        _isPrewarming = true;

        for (int i = _stackedSceneHandles.Count; i < _worldInstanceCount; i++)
        {
            SceneLookupData lookup = new SceneLookupData(0, _sceneName);
            SceneLoadData sld = new SceneLoadData(lookup);

            sld.Options.AllowStacking = true;
            sld.Options.AutomaticallyUnload = false;
            sld.ReplaceScenes = ReplaceOption.None;

            // Server-only load. Clients will be moved into a specific handle later.
            InstanceFinder.SceneManager.LoadConnectionScenes(sld);
        }
    }

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

            if (_stackedSceneHandles.Contains(loadedScene.handle))
                continue;

            if (_stackedSceneHandles.Count >= _worldInstanceCount)
                break;

            _stackedSceneHandles.Add(loadedScene.handle);
            Debug.Log($"Registered {_sceneName} instance #{_stackedSceneHandles.Count}: handle={loadedScene.handle}");
        }

        if (!AreWorldInstancesReady())
            return;

        _isPrewarming = false;
        FlushPendingConnections();
    }

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

    private void LoadConnectionIntoAssignedInstance(NetworkConnection connection)
    {
        if (!_assignedHandleByConnection.TryGetValue(connection, out int sceneHandle))
        {
            _joinIndex++;

            int instanceIndex = (_joinIndex - 1) % _worldInstanceCount;
            sceneHandle = _stackedSceneHandles[instanceIndex];

            _assignedHandleByConnection.Add(connection, sceneHandle);

            Debug.Log($"Connection {connection.ClientId} joined as #{_joinIndex}; assigned scene handle={sceneHandle}");
        }

        LoadScene(connection, sceneHandle);
    }

    private void LoadScene(NetworkConnection connection, int sceneHandle)
    {
        SceneLookupData lookup = new SceneLookupData(sceneHandle, _sceneName);
        SceneLoadData sld = new SceneLoadData(lookup);

        sld.Options.AllowStacking = true;
        sld.MovedNetworkObjects = connection.Objects.ToArray();
        sld.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadConnectionScenes(connection, sld);
    }

    private void LoadScene(NetworkObject networkObject, int sceneHandle)
    {
        SceneLookupData lookup = new SceneLookupData(sceneHandle, _sceneName);
        SceneLoadData sld = new SceneLoadData(lookup);

        sld.Options.AllowStacking = true;
        sld.MovedNetworkObjects = new[] { networkObject };
        sld.ReplaceScenes = ReplaceOption.All;

        InstanceFinder.SceneManager.LoadConnectionScenes(networkObject.Owner, sld);
    }

    private bool AreWorldInstancesReady()
    {
        return _stackedSceneHandles.Count >= _worldInstanceCount;
    }

    private void ServerManager_OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState != RemoteConnectionState.Stopped)
            return;

        _pendingConnections.Remove(connection);
        _assignedHandleByConnection.Remove(connection);
    }
}