using System;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public bool SceneStack = true;

    private float _timer;
    private bool _isLoading;
    private int _stackedSceneHandle = 0;

    private const string _sceneName = "TestFishNet_World1";

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
        InstanceFinder.SceneManager.OnClientLoadedStartScenes += SceneManagerOnOnClientLoadedStartScenes;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null)
        {
            InstanceFinder.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
            InstanceFinder.SceneManager.OnClientLoadedStartScenes -= SceneManagerOnOnClientLoadedStartScenes;
        }
    }

    private void LoadScene(NetworkConnection networkConnection)
    {
        SceneLookupData lookup = new SceneLookupData(_stackedSceneHandle, _sceneName);
        SceneLoadData sld = new SceneLoadData(lookup);
        sld.Options.AllowStacking = true;

        sld.MovedNetworkObjects = networkConnection.Objects.ToArray();
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadConnectionScenes(networkConnection, sld);
    }

    private void LoadScene(NetworkObject networkObject)
    {
        SceneLookupData lookup = new SceneLookupData(_stackedSceneHandle, _sceneName);
        SceneLoadData sld = new SceneLoadData(lookup);
        sld.Options.AllowStacking = true;

        sld.MovedNetworkObjects = new NetworkObject[] { networkObject };
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadConnectionScenes(networkObject.Owner, sld);
    }

    private void SceneManagerOnOnClientLoadedStartScenes(NetworkConnection arg1, bool arg2)
    {
        LoadScene(arg1);
    }

    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj)
    {
        // Server handles scene loading and syncing
        // so do not bother setting up scene stacking if it was a client
        // that completed the scene load
        if (!obj.QueueData.AsServer)
            return;
        if (!SceneStack)
            return;

        // Stacked scene id is already set, not interested in creating a new stacked scene
        if (_stackedSceneHandle != 0)
            return;

        // Set the first loaded scene as the handle
        if (obj.LoadedScenes.Length > 0)
            _stackedSceneHandle = obj.LoadedScenes[0].handle;
    }
}
