using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemyBase : MonoBehaviour
    {
        public EnemyScriptable status;
        public GameObject inPlayerRangeGfx;

        private float currentHealth;
        private float damage;

        private void Start()
        {
            currentHealth = status.health;
            damage = status.damage;
        }

        public void IsInPlayerRange(bool value)
        {
            inPlayerRangeGfx.SetActive(value);
        }

        public void TakeDamage(float damageAmount)
        {
            currentHealth -= damageAmount;
            if (currentHealth <= 0.0f)
                Die();
        }

        private void Die()
        {
            Destroy(this.gameObject);
        }
    }
}