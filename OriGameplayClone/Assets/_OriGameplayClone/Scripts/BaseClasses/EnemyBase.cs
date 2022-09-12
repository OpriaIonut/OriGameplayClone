using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OriProject
{
    public class EnemyBase : MonoBehaviour
    {
        public EnemyScriptable status;
        public GameObject inPlayerRangeGfx;
        public Image healthBar;

        protected Transform playerTransf;
        protected bool isPlayerInRange = false;

        private float currentHealth;
        private float lastAttackTime = 0.0f;

        private void Start()
        {
            currentHealth = status.health;

            healthBar.transform.parent.gameObject.SetActive(false);
            playerTransf = FindObjectOfType<PlayerLogic>().transform;

            StartCoroutine("FindPlayerRange");
        }

        private void Update()
        {
            if (isPlayerInRange)
                MoveTowardsPlayer();
            else
                MoveNoTarget();

            if(isPlayerInRange && Time.time - lastAttackTime > status.attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        public void IsInPlayerRange(bool value)
        {
            inPlayerRangeGfx.SetActive(value);
        }

        public bool TakeDamage(float damageAmount)
        {
            currentHealth -= damageAmount;
            if (currentHealth <= 0.0f)
            {
                Die();
                return true;
            }

            float healthScale = currentHealth / status.health;
            healthBar.transform.parent.gameObject.SetActive(true);
            healthBar.transform.localScale = new Vector3(healthScale, healthBar.transform.localScale.y, healthBar.transform.localScale.z);

            return false;
        }

        protected virtual void Die()
        {
            Destroy(this.gameObject);
        }

        protected virtual void MoveTowardsPlayer()
        {
            //Intentionally left empty
        }

        protected virtual void MoveNoTarget()
        {
            //Intentionally left empty
        }

        protected virtual void Attack()
        {
            //Intentionally left empty
        }

        private IEnumerator FindPlayerRange()
        {
            while(true)
            {
                isPlayerInRange = false;
                if(Vector3.Distance(playerTransf.position, transform.position) < status.range)
                {
                    isPlayerInRange = true;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                PlayerLogic playerLogic = collision.gameObject.GetComponent<PlayerLogic>();
                if (playerLogic)
                {
                    playerLogic.TakeDamage(status.damage);
                }
            }
        }
    }
}