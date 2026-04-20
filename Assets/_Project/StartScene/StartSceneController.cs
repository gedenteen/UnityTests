using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private ButtonLoadScene _prefabButtonLoadScene;
    [SerializeField] private Transform _buttonsHolder;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;

        int scenesCount = SceneManager.sceneCountInBuildSettings;

        for (int i = 1; i < scenesCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);

            ButtonLoadScene buttonLoadScene = Instantiate(_prefabButtonLoadScene, _buttonsHolder);
            buttonLoadScene.TextMesh.text = sceneName;

            // Захватываем имя в локальную переменную для замыкания
            string capturedName = sceneName;
            buttonLoadScene.Button.onClick.AddListener(() => SceneManager.LoadScene(capturedName));
        }
    }
}
