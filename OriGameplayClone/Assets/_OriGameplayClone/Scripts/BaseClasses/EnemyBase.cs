using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OriProject
{
    public class EnemyBase : PropellTarget
    {
        public float speed;
        public EnemyScriptable status;
        public GameObject inPlayerRangeGfx;
        public Image healthBar;

        protected Transform playerTransf;
        protected Rigidbody rb;
        protected bool isPlayerInRange = false;
        protected bool damagedPlayer = false;
        protected float timePlayerExitedRange = 0.0f;

        private float currentHealth;
        private float lastAttackTime = 0.0f;

        protected virtual void BaseStartCall()
        {
            currentHealth = status.health;

            healthBar.transform.parent.gameObject.SetActive(false);
            playerTransf = FindObjectOfType<PlayerLogic>().transform;

            rb = GetComponent<Rigidbody>();

            StartCoroutine("FindPlayerRange");
        }

        protected virtual void  BaseUpdateCall()
        {
            MovementLogic();

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

        protected virtual void MovementLogic()
        {
            //Intentionally left empty
        }

        protected virtual void Attack()
        {
            damagedPlayer = false;
        }

        protected virtual IEnumerator FindPlayerRange()
        {
            while(true)
            {
                if(Vector3.Distance(playerTransf.position, transform.position) < status.range)
                {
                    isPlayerInRange = true;
                }
                else
                {
                    if (isPlayerInRange)
                        timePlayerExitedRange = Time.time;
                    isPlayerInRange = false;
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        protected virtual void OnTriggerEnterBase(Collider other)
        {
            if(other.tag == "PlayerHitbox" && !damagedPlayer)
            {
                PlayerLogic playerLogic = other.transform.root.GetComponent<PlayerLogic>();
                if (playerLogic)
                {
                    playerLogic.TakeDamage(status.damage, transform);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, status.range);
        }
    }
}