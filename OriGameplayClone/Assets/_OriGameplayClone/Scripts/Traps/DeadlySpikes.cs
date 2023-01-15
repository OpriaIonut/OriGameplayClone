using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class DeadlySpikes : MonoBehaviour
    {
        public float damage = 1000;

        private void OnTriggerEnter(Collider other)
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (other.tag == "PlayerHitbox")
            {
                PlayerLogic script = other.transform.root.GetComponent<PlayerLogic>();
                if (script)
                {
                    script.TakeDamage(damage, transform);
                }
            }
        }
    }
}