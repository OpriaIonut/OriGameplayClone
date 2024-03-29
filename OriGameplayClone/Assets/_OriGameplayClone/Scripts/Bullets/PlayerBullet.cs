using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class PlayerBullet : MonoBehaviour
    {
        public float interpolationTime = 0.5f;
        public float middleHeightFactor = 3.0f;

        private Vector3 startPoint;
        private Transform endPointTransf = null;
        private Vector3 endPoint;
        private Vector3 calculatedMiddle;

        private float damage;
        private float currentInterpolation = 0.0f;

        private PlayerLogic playerScript;
        private EnemyBase enemyScript;

        private void Update()
        {
            if (UIManager.Instance.GamePaused)
                return;

            MoveBullet();

            if(enemyScript == null && currentInterpolation >= 1.0f)
            {
                DestroyImmediate(gameObject);
            }
        }

        public void Init(PlayerLogic _playerScript, EnemyBase enemy, Vector3 _startPoint, Transform _endPoint, float _damage)
        {
            playerScript = _playerScript;
            enemyScript = enemy;

            startPoint = _startPoint;
            endPointTransf = _endPoint;
            calculatedMiddle = (startPoint + endPointTransf.position) / 2.0f;
            damage = _damage;

            float yDiff = endPointTransf.position.y - startPoint.y;
            if (Mathf.Abs(yDiff) < 0.1f)
                yDiff = 0.5f;

            calculatedMiddle.y += yDiff * middleHeightFactor;
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
            if (endPointTransf)
                endPoint = endPointTransf.position;

            Vector3 calculatedPos = Vector3.Lerp(startPoint, endPoint, currentInterpolation);
            float yFact = -4.0f * middleHeightFactor * currentInterpolation * currentInterpolation + 4.0f * middleHeightFactor * currentInterpolation;
            calculatedPos.y = yFact + Mathf.Lerp(startPoint.y, endPoint.y, currentInterpolation);

            transform.position = calculatedPos;

            float interpolationFact = 1.0f / interpolationTime;
            currentInterpolation += interpolationFact * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (other.tag == "EnemyHitbox")
            {
                EnemyBase script = other.transform.root.GetComponent<EnemyBase>();
                if (script)
                {
                    bool enemyDied = script.TakeDamage(damage);

                    if (enemyDied)
                    {
                        playerScript.EnemyDied(enemyScript);
                    }

                    Destroy(this.gameObject);
                }
            }
        }
    }
}