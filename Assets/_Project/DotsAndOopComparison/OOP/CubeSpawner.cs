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

        private float _timer;

        // Update is called once per frame
        void Update()
        {
            _timer += Time.deltaTime;
            if (_timer >= spawnRate)
            {
                _timer -= spawnRate;

                CubeController cubeController = Instantiate(cubePrefab);
                cubeController.moveDirection = new Vector3(
                    UnityEngine.Random.Range(0f, 1f),
                    UnityEngine.Random.Range(0f, 1f),
                    UnityEngine.Random.Range(0f, 1f)
                );
                cubeController.moveSpeed = cubeMoveSpeed;

                CountOfSpawnedCubes++;
            }
        }
    }
}
