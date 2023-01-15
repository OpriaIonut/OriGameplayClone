using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class CameraMovement : MonoBehaviour
    {
        public Transform target;
        public float speed;

        private Vector3 offset;

        private void Start()
        {
            offset = new Vector3(0.0f, 0.0f, transform.position.z - target.position.z);
        }

        private void FixedUpdate()
        {
            if (UIManager.Instance.GamePaused)
                return;

            transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.fixedDeltaTime * speed);
        }
    }
}