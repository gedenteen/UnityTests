using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CanvasToStartScene : MonoBehaviour
{
    [SerializeField] private Button _myButton;

    private void Awake()
    {
        _myButton.onClick.AddListener(() => SceneManager.LoadScene(0));
    }
}
