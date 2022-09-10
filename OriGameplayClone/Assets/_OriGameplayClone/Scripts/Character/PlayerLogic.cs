using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class PlayerLogic : MonoBehaviour
    {
        public float maxHealth;
        public float damage;

        public GameObject bulletPrefab;
        public Transform bulletSpawnPoint;

        private float currentHealth;

        private List<EnemyBase> enemiesInRange = new List<EnemyBase>();

        private void Start()
        {
            currentHealth = maxHealth;
        }

        private void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                if(enemiesInRange.Count > 0)
                {
                    GameObject clone = Instantiate(bulletPrefab);
                    clone.transform.position = bulletSpawnPoint.position;

                    PlayerBullet script = clone.GetComponent<PlayerBullet>();
                    script.Init(bulletSpawnPoint.position, enemiesInRange[0].transform.position, damage);
                }
            }
        }

        public void TakeDamage(float health)
        {
            currentHealth -= maxHealth;
            if (currentHealth <= 0.0f)
                Die();
        }

        private void Die()
        {
            Debug.Log("Game Over!");
            gameObject.SetActive(false);
        }

        public void AddEnemyInRange(EnemyBase enemy)
        {
            enemiesInRange.Add(enemy);
            enemy.IsInPlayerRange(true);
        }

        public void RemoveEnemyInRange(EnemyBase enemy)
        {
            enemiesInRange.Remove(enemy);
            enemy.IsInPlayerRange(false);
        }
    }
}