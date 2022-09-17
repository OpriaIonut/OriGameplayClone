using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class JumpingFrog : EnemyBase
    {
        public float delayBetweenJumps = 1.0f;
        public float upwardsPropell = 100.0f;
        public float horizontalPropellLimit = 100.0f;

        private bool isGrounded = false;
        private float reachedGroundTime = 0.0f;

        protected override void MoveNoTarget()
        {
            //Intentionally left empty
        }

        protected override void MoveTowardsPlayer()
        {
            if (isGrounded && Time.time - reachedGroundTime > delayBetweenJumps)
            {
                isGrounded = false;
                damagedPlayer = false;

                float horizontalAmount = playerTransf.position.x - transform.position.x;
                horizontalAmount *= status.speed;
                if (Mathf.Abs(horizontalAmount) > horizontalPropellLimit)
                {
                    if (horizontalAmount > 0.0f)
                        horizontalAmount = horizontalPropellLimit;
                    else
                        horizontalAmount = -horizontalPropellLimit;
                }

                Vector3 propelDir = new Vector3(horizontalAmount, upwardsPropell, 0.0f);
                rb.AddForce(propelDir);
            }
        }

        protected override void Attack()
        {
            //Intentionally left empty
        }


        private void OnTriggerEnter(Collider other)
        {
            this.OnTriggerEnterBase(other);
            if (other.tag == "GroundPlatform")
            {
                isGrounded = true;
                reachedGroundTime = Time.time;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "GroundPlatform")
            {
                isGrounded = false;
            }
        }
    }
}