using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class ArachneBullet : PropellTarget
    {
        public float speed = 10.0f;

        private Vector3 direction;
        private bool isInitialized = false;
        private float damage;
        private bool hitPlayer = true;

        private void Update()
        {
            if(isInitialized)
                transform.Translate(direction * speed * Time.deltaTime);
        }

        public void Init(Vector3 _target, float _damage)
        {
            direction = (_target - transform.position).normalized;
            damage = _damage;
            isInitialized = true;
        }

        public override void LaunchTarget(Vector3 direction)
        {
            base.LaunchTarget(direction);
            hitPlayer = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hitPlayer && other.tag == "PlayerHitbox")
            {
                PlayerLogic playerScript = other.transform.root.GetComponent<PlayerLogic>();
                if (playerScript)
                {
                    playerScript.TakeDamage(damage, transform);
                    Destroy(gameObject);
                }
            }
            else if(!hitPlayer && other.tag == "EnemyHitbox")
            {
                EnemyBase enemy = other.transform.root.GetComponent<EnemyBase>();
                if(enemy)
                {
                    enemy.TakeDamage(damage * 5.0f);
                    Destroy(gameObject);
                }
            }
        }
    }
}