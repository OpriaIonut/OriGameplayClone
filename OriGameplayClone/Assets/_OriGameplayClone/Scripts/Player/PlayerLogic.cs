using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OriProject
{
    public class PlayerLogic : MonoBehaviour
    {
        public float maxHealth;
        public Image healthBar; 

        [Header("Attack")]
        public float damage;
        public float radius;
        public int shotsBeforeDelay = 5;
        public float attackRechargeDelay = 1.0f;
        public int maxNumOfEnemies = 3;
        public GameObject bulletPrefab;
        public Transform bulletSpawnPoint;

        [Header("ChargeAttack")]
        public float chargeAttackDamage;
        public float chargeTime = 1.0f;
        public ParticleSystem chargeReadyParticles;
        public ParticleSystem chargeAttackParticles;

        private float currentHealth;
        private int currentShots = 0;
        private float attackRechargeTime = 0.0f;

        private EnemyDetector enemyDetector;
        private CharacterMovement movementScript;

        private float chargeStartTime = 0.0f;
        private bool isCharging = false;
        private bool chargeReady = false;

        private List<EnemyBase> enemiesInRange = new List<EnemyBase>();
        private List<Tuple<int, float>> enemyDistance = new List<Tuple<int, float>>();

        private void Start()
        {
            currentHealth = maxHealth;
            enemyDetector = GetComponentInChildren<EnemyDetector>();
            movementScript = GetComponent<CharacterMovement>();
            enemyDetector.SetRadius(radius);
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
                isCharging = true;
                chargeStartTime = Time.time;

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
                        pos = transform.position + pos * radius * 0.5f;

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
                        script.Init(this, targetedEnemies[index], bulletSpawnPoint.position, targetedEnemies[index].transform, damage);
                    }
                }

                currentShots++;
                if (currentShots == shotsBeforeDelay)
                {
                    attackRechargeTime = Time.time;
                }
            }
            if(Input.GetMouseButtonUp(0))
            {
                if(chargeReady)
                {
                    chargeAttackParticles.Play();
                    for(int index = 0; index < enemiesInRange.Count; index++)
                    {
                        enemiesInRange[index].TakeDamage(chargeAttackDamage);
                    }
                }
                isCharging = false;
                chargeReady = false;
            }
            if(isCharging && !chargeReady && Time.time - chargeStartTime > chargeTime)
            {
                chargeReady = true;
                chargeReadyParticles.Play();
            }
        }

        public void TakeDamage(float damage, Transform attackerTransf)
        {
            currentHealth -= damage;
            if (currentHealth <= 0.0f)
                Die();
            else
            {
                float healthBarFill = currentHealth / maxHealth;
                healthBar.transform.localScale = new Vector3(healthBarFill, healthBar.transform.localScale.y, healthBar.transform.localScale.z);

                Vector3 direction = (transform.position - attackerTransf.position).normalized;
                direction.y = 0.0f;
                direction.z = 0.0f;
                movementScript.AddKnockback(direction);
            }
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