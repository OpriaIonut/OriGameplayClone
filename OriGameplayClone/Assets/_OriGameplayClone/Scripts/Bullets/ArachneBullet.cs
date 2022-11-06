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

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "PlayerHitbox")
            {
                PlayerLogic playerScript = other.transform.root.GetComponent<PlayerLogic>();
                if (playerScript)
                {
                    playerScript.TakeDamage(damage, transform);
                    Destroy(gameObject);
                }
            }
        }
    }
}