using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class PlayerAttackPoint : MonoBehaviour
    {
        public float speed = 1.0f;
        public float middleHeightFactor = 2.0f;
        public float speedUpThreshold = 5.0f;
        public float speedUpFactor;

        public Vector2 maxStride;
        public Transform playerTransform;

        private Vector3 startPoint;
        private Vector3 targetPoint;
        private float sideFlipper = 1.0f;
        private float currentInterpolation = 0.0f;

        private void Start()
        {
            startPoint = transform.position;
        }

        private void Update()
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (currentInterpolation >= 1.0f)
            {
                FindNewPos();
            }
            MovePoint();
        }

        private void FindNewPos()
        {
            float xStride = Random.Range(maxStride.x * 0.5f, maxStride.x);
            float yStride = Random.Range(maxStride.y * 0.5f, maxStride.y);
            targetPoint = new Vector3(xStride * sideFlipper, yStride, 0.0f);
            sideFlipper *= -1.0f;
            currentInterpolation = 0.0f;
            startPoint = transform.position;
        }

        public void MovePoint()
        {
            Vector3 endPoint = transform.position + targetPoint;

            Vector3 calculatedPos = Vector3.Lerp(startPoint, endPoint, currentInterpolation);
            float yFact = -4.0f * middleHeightFactor * currentInterpolation * currentInterpolation + 4.0f * middleHeightFactor * currentInterpolation;
            calculatedPos.y = yFact + Mathf.Lerp(startPoint.y, endPoint.y, currentInterpolation);

            float currentSpeed = speed;
            if (Vector3.Distance(transform.position, playerTransform.position) > speedUpThreshold)
                currentSpeed *= speedUpFactor;

            transform.position = Vector3.Lerp(transform.position, playerTransform.position + targetPoint, currentSpeed * Time.deltaTime);

            float interpolationFact = currentSpeed;
            currentInterpolation += interpolationFact * Time.deltaTime;
        }
    }
}