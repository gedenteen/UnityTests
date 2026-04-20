using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OOP
{
    public class StatsDisplayer : MonoBehaviour
    {
        public CubeSpawner cubeSpawner;
        public TMP_Text _textTime;
        public TMP_Text _textCubesCount;

        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;

            _textTime.text = _timer.ToString("F1");
            _textCubesCount.text = cubeSpawner.CountOfSpawnedCubes.ToString();
        }
    }
}
