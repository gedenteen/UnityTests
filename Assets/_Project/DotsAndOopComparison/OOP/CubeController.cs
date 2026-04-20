using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OOP
{
    public class CubeController : MonoBehaviour
    {
        public Vector3 moveDirection;
        public float moveSpeed;

        private void Update()
        {
            transform.position = transform.position +
                                 moveDirection * moveSpeed * Time.deltaTime;
        }
    }
}
