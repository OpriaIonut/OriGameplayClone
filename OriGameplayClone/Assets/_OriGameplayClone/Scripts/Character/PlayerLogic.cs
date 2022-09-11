using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class PlayerLogic : MonoBehaviour
    {
        public float maxHealth;
        public float damage;

        [Header("Attack")]
        public int shotsBeforeDelay = 5;
        public float attackRechargeDelay = 1.0f;
        public int maxNumOfEnemies = 3;
        public GameObject bulletPrefab;
        public Transform bulletSpawnPoint;

        private float currentHealth;
        private int currentShots = 0;
        private float attackRechargeTime = 0.0f;

        private EnemyDetector enemyDetector;

        private List<EnemyBase> enemiesInRange = new List<EnemyBase>();
        private List<Tuple<int, float>> enemyDistance = new List<Tuple<int, float>>();

        private void Start()
        {
            currentHealth = maxHealth;
            enemyDetector = GetComponentInChildren<EnemyDetector>();
        }

        private void Update()
        {
            AttackLogic();
        }

        private void AttackLogic()
        {
            if (currentShots == shotsBeforeDelay)
            {
                if (Time.time - attackRechargeTime > attackRechargeDelay)
                {
                    currentShots = 0;
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                List<EnemyBase> targetedEnemies = new List<EnemyBase>(enemiesInRange);
                if (enemiesInRange.Count > maxNumOfEnemies)
                {
                    enemyDistance.Clear();
                    for (int index = 0; index < enemiesInRange.Count; index++)
                    {
                        float dist = Vector3.Distance(transform.position, enemiesInRange[index].transform.position);
                        enemyDistance.Add(new Tuple<int, float>(index, dist));
                    }
                    enemyDistance.Sort((x, y) => y.Item2.CompareTo(x.Item2));
                    enemyDistance.Reverse();

                    targetedEnemies.Clear();
                    for(int index = 0; index < maxNumOfEnemies; index++)
                    {
                        int enemyID = enemyDistance[index].Item1;
                        targetedEnemies.Add(enemiesInRange[enemyID]);
                    }
                }


                int count = Math.Min(targetedEnemies.Count, maxNumOfEnemies);
                if (count == 0)
                {
                    for (int index = 0; index < maxNumOfEnemies; index++)
                    {
                        float randAngle = UnityEngine.Random.Range(0.0f, 2 * (float)Math.PI);
                        Vector3 pos = new Vector3(Mathf.Cos(randAngle), Mathf.Sin(randAngle), 0.0f);
                        pos = transform.position + pos * enemyDetector.radius * 0.5f;

                        GameObject clone = Instantiate(bulletPrefab);
                        clone.transform.position = bulletSpawnPoint.position;
                        Destroy(clone, 10.0f);

                        PlayerBullet script = clone.GetComponent<PlayerBullet>();
                        script.Init(this, null, bulletSpawnPoint.position, pos, damage);
                    }
                }
                else
                {
                    for (int index = 0; index < count; index++)
                    {
                        GameObject clone = Instantiate(bulletPrefab);
                        clone.transform.position = bulletSpawnPoint.position;
                        Destroy(clone, 10.0f);

                        PlayerBullet script = clone.GetComponent<PlayerBullet>();
                        script.Init(this, targetedEnemies[index], bulletSpawnPoint.position, targetedEnemies[index].transform.position, damage);
                    }
                }

                currentShots++;
                if (currentShots == shotsBeforeDelay)
                {
                    attackRechargeTime = Time.time;
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

        public void EnemyDied(EnemyBase enemy)
        {
            RemoveEnemyInRange(enemy);
        }

        public void AddEnemyInRange(EnemyBase enemy)
        {
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                enemy.IsInPlayerRange(true);
            }
        }

        public void RemoveEnemyInRange(EnemyBase enemy)
        {
            if (enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
                enemy.IsInPlayerRange(false);
            }
        }
    }
}