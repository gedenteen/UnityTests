using System;
using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using UnityEngine;

public class SceneLoader : MonoBehaviour
{
    public bool SceneStack = false;

    private float _timer;
    private bool _isLoading;
    private int _stackedSceneHandle = 0;

    private const string _sceneName = "TestFishNet_World1";

    private void Start()
    {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null)
            InstanceFinder.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
    }

    private void OnTriggerEnter(Collider other)
    {
        NetworkObject networkObject = other.GetComponent<NetworkObject>();
        if (networkObject != null)
            LoadScene(networkObject);
    }

    private void LoadScene(NetworkObject networkObject)
    {
        SceneLookupData lookup = new SceneLookupData(_stackedSceneHandle, _sceneName);
        SceneLoadData sld = new SceneLoadData(lookup);
        sld.Options.AllowStacking = true;

        sld.MovedNetworkObjects = new NetworkObject[] { networkObject };
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadConnectionScenes(networkObject.Owner, sld);
        // InstanceFinder.SceneManager.LoadGlobalScenes(sld);
    }

    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj)
    {
        //
        if (!obj.QueueData.AsServer)
            return;
        if (!SceneStack)
            return;

        //
        if (_stackedSceneHandle != 0)
            return;

        //
        if (obj.LoadedScenes.Length > 0)
            _stackedSceneHandle = obj.LoadedScenes[0].handle;
    }
}
