using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemyDetector : MonoBehaviour
    {
        public float radius = 10.0f;
        private PlayerLogic playerScript;

        private void Start()
        {
            playerScript = transform.root.GetComponent<PlayerLogic>();
            GetComponent<SphereCollider>().radius = radius;
        }

        private void OnTriggerEnter(Collider other)
        {
            EnemyBase enemyScript = other.transform.root.GetComponent<EnemyBase>();
            if(enemyScript)
            {
                playerScript.AddEnemyInRange(enemyScript);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            EnemyBase enemyScript = other.transform.root.GetComponent<EnemyBase>();
            if (enemyScript)
            {
                playerScript.RemoveEnemyInRange(enemyScript);
            }
        }
    }
}