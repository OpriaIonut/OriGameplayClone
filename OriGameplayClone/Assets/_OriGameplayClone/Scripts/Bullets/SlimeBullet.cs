using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class SlimeBullet : MonoBehaviour
    {
        private float damage;
        private Rigidbody rb;

        public void Init(float _damage, Vector3 propellForce)
        {
            damage = _damage;
            rb = GetComponent<Rigidbody>();
            rb.AddForce(propellForce);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "PlayerHitbox")
            {
                PlayerLogic playerScript = other.transform.root.GetComponent<PlayerLogic>();
                if(playerScript)
                {
                    playerScript.TakeDamage(damage, transform);
                }
            }
            else if(other.tag == "GroundPlatform" || other.tag == "ClimbableWall" || other.tag == "BreakablePlatform")
            {
                rb.velocity = Vector3.zero;
                rb.isKinematic = true;
            }
        }
    }
}