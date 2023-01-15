using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemyDetector : MonoBehaviour
    {
        private float radius = 10.0f;
        private PlayerLogic playerScript;

        private void Start()
        {
            playerScript = transform.root.GetComponent<PlayerLogic>();
        }

        public void SetRadius(float _radius)
        {
            radius = _radius;
            GetComponent<SphereCollider>().radius = radius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (UIManager.Instance.GamePaused)
                return;

            EnemyBase enemyScript = other.transform.root.GetComponent<EnemyBase>();
            if(enemyScript)
            {
                playerScript.AddEnemyInRange(enemyScript);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (UIManager.Instance.GamePaused)
                return;

            EnemyBase enemyScript = other.transform.root.GetComponent<EnemyBase>();
            if (enemyScript)
            {
                playerScript.RemoveEnemyInRange(enemyScript);
            }
        }
    }
}