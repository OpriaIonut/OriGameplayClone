using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemyCrow : EnemyBase
    {
        public float moveDelay = 5.0f;
        public float moveDistance = 25.0f;
        public float moveBackSpeed = 5.0f;

        private Vector3 startPos;

        private float moveAmount = 0.0f;
        private float lastMoveTime = 0.0f;

        private void Start()
        {
            BaseStartCall();
            startPos = transform.position;
        }

        private void Update()
        {
            BaseUpdateCall();
        }

        protected override void Attack()
        {
            //Intentionally left empty
        }

        protected override void MovementLogic()
        {
            if(isPlayerInRange)
            {
                if (Time.time - lastMoveTime > moveDelay)
                {
                    Vector3 moveDir = (playerTransf.position - transform.position).normalized;
                    rb.AddForce(moveDir * speed);
                    lastMoveTime = Time.time;
                }
            }
            else
            {
                if (Time.time - timePlayerExitedRange > 3.0f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, startPos, moveBackSpeed * Time.deltaTime);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            this.OnTriggerEnterBase(other);
        }
    }
}