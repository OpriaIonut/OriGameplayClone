using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class PlayerBullet : MonoBehaviour
    {
        public float interpolationTime = 0.5f;
        public float middleHeightFactor = 3.0f;
        public GameObject debugSphere;

        private Vector3 startPoint;
        private Vector3 endPoint;
        private Vector3 calculatedMiddle;

        private float damage;
        private float currentInterpolation = 0.0f;

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

            GameObject clone1 = Instantiate(debugSphere);
            clone1.transform.position = startPoint;
            clone1.name = "P0";

            GameObject clone2 = Instantiate(debugSphere);
            clone2.transform.position = calculatedMiddle;
            clone2.name = "P1";

            GameObject clone3 = Instantiate(debugSphere);
            clone3.transform.position = endPoint;
            clone3.name = "P2";
        }

        private void MoveBullet()
        {
            //(1 - t) ^ 2) * p0 + 2 * (1 - t) * t * p1 + t * 2 * p2
            //Vector3 calculatedPos = Mathf.Pow((1.0f - currentInterpolation), 2.0f) * startPoint + 2.0f * (1.0f - currentInterpolation) * calculatedMiddle + Mathf.Pow(currentInterpolation, 2.0f) * endPoint;

            float u = 1.0f - currentInterpolation;
            float tt = currentInterpolation * currentInterpolation;
            float uu = u * u;
            Vector3 p = uu * startPoint;
            p += 2 * u * tt * calculatedMiddle;
            p += tt * endPoint;

            Vector3 calculatedPos = p;


            //Vector3 calculatedPos = Vector3.Lerp(startPoint, endPoint, currentInterpolation);
            transform.position = calculatedPos;

            float interpolationFact = 1.0f / interpolationTime;
            currentInterpolation += interpolationFact * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            EnemyBase script = other.transform.root.GetComponent<EnemyBase>();
            if(script)
            {
                script.TakeDamage(damage);
                Destroy(this.gameObject);
            }
        }
    }
}