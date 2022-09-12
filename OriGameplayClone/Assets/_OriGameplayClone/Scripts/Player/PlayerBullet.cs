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

        private PlayerLogic playerScript;
        private EnemyBase enemyScript;

        private void Update()
        {
            MoveBullet();

            if(enemyScript == null && currentInterpolation >= 1.0f)
            {
                DestroyImmediate(gameObject);
            }
        }

        public void Init(PlayerLogic _playerScript, EnemyBase enemy, Vector3 _startPoint, Vector3 _endPoint, float _damage)
        {
            playerScript = _playerScript;
            enemyScript = enemy;

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
            EnemyBase script = other.transform.root.GetComponent<EnemyBase>();
            if(script)
            {
                bool enemyDied = script.TakeDamage(damage);

                if(enemyDied)
                {
                    playerScript.EnemyDied(enemyScript);
                }

                Destroy(this.gameObject);
            }
        }
    }
}