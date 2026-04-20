using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace OOP
{
    public class CubeSpawner : MonoBehaviour
    {
        public CubeController cubePrefab;
        public float spawnRate = 0.2f;
        public float cubeMoveSpeed = 10f;
        public uint CountOfSpawnedCubes { get; private set; } = 0;
        private Queue<CubeController> _cubesControllers = new();
        private float _direction = 1f;

        private float _timerSpawn;
        private float _timerDirection;
        private float _maxTime = 5f;

        // Update is called once per frame
        void Update()
        {
            _timerSpawn += Time.deltaTime;
            if (_timerSpawn >= spawnRate)
            {
                _timerSpawn -= spawnRate;

                CubeController cubeController = Instantiate(cubePrefab);
                cubeController.moveDirection = new Vector3(
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f),
                    UnityEngine.Random.Range(-1f, 1f)
                ).normalized;

                cubeController.moveSpeed = cubeMoveSpeed;
                _cubesControllers.Enqueue(cubeController);

                CountOfSpawnedCubes++;
            }

            _timerDirection += Time.deltaTime;
            if (_timerDirection >= _maxTime)
            {
                _direction *= -1;
                _timerDirection -= _maxTime;
            }

            // Вычисляем скорость один раз на главном потоке
            float currentSpeed = _direction * Time.deltaTime * 300f;

            foreach (CubeController cubeController in _cubesControllers)
            {
                cubeController.moveSpeed = currentSpeed;
            }
        }
    }
}
