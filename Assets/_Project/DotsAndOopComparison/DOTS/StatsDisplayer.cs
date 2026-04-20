using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DOTS
{
    public class StatsDisplayer : MonoBehaviour
    {
        public TMP_Text _textTime;

        private float _timer;

        private void Update()
        {
            _timer += Time.deltaTime;

            _textTime.text = _timer.ToString("F1");
        }
    }
}
