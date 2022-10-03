using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class PukingFrogBullet : MonoBehaviour
    {
        public float interpolationTime = 0.5f;
        public float middleHeightFactor = 3.0f;

        private Vector3 startPoint;
        private Vector3 endPoint;
        private Vector3 calculatedMiddle;

        private float damage;
        private float currentInterpolation = 0.0f;
        private float spawnTime = 0.0f;

        private void Start()
        {
            spawnTime = Time.time;
        }

        private void Update()
        {
            MoveBullet();
        }

        public void Init(Vector3 _startPoint, Vector3 _endPoint, float _damage)
        {
            startPoint = _startPoint;
            endPoint = _endPoint;
            calculatedMiddle = (startPoint + endPoint) / 2.0f;
            damage = _damage;

            float yDiff = endPoint.y - startPoint.y;
            if (Mathf.Abs(yDiff) < 0.1f)
                yDiff = 0.5f;

            calculatedMiddle.y += yDiff * middleHeightFactor;
        }

        private void MoveBullet()
        {
            Vector3 calculatedPos = Vector3.Lerp(startPoint, endPoint, currentInterpolation);
            float yFact = -4.0f * middleHeightFactor * currentInterpolation * currentInterpolation + 4.0f * middleHeightFactor * currentInterpolation;
            calculatedPos.y = yFact + Mathf.Lerp(startPoint.y, endPoint.y, currentInterpolation);

            transform.position = calculatedPos;

            float interpolationFact = 1.0f / interpolationTime;
            currentInterpolation += interpolationFact * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "PlayerHitbox")
            {
                PlayerLogic script = other.transform.root.GetComponent<PlayerLogic>();
                if (script)
                {
                    script.TakeDamage(damage, transform);
                    Destroy(this.gameObject);
                }
            }
            else if(other.tag == "GroundPlatform" || other.tag == "ClimbableWall" || other.tag == "BreakablePlatform")
            {
                if (Time.time - spawnTime > 0.25f)
                    Destroy(this.gameObject);
            }
        }
    }
}