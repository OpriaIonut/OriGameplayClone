using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class JumpingFrog : EnemyBase
    {
        public Transform floorDetector;
        public float detectionDistance = 0.15f;
        public LayerMask platformsLayer;

        public float delayBetweenJumps = 1.0f;
        public float upwardsPropell = 100.0f;
        public float horizontalPropellLimit = 100.0f;

        private bool isGrounded = false;
        private float reachedGroundTime = 0.0f;

        private void Start()
        {
            BaseStartCall();
            StartCoroutine(CheckPlatforms());
        }

        private void Update()
        {
            if (UIManager.Instance.GamePaused)
                return;

            BaseUpdateCall();
        }

        protected override void MovementLogic()
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (isPlayerInRange && isGrounded && Time.time - reachedGroundTime > delayBetweenJumps)
            {
                isGrounded = false;
                damagedPlayer = false;

                float horizontalAmount = playerTransf.position.x - transform.position.x;
                horizontalAmount *= speed;
                if (Mathf.Abs(horizontalAmount) > horizontalPropellLimit)
                {
                    if (horizontalAmount > 0.0f)
                        horizontalAmount = horizontalPropellLimit;
                    else
                        horizontalAmount = -horizontalPropellLimit;
                }


                float rotAngle = horizontalAmount > 0.0f ? 0.0f : 180.0f;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotAngle, transform.rotation.eulerAngles.z);

                Vector3 propelDir = new Vector3(horizontalAmount, upwardsPropell, 0.0f);
                rb.AddForce(propelDir);
            }
        }

        protected override void Attack()
        {
            //Intentionally left empty
        }

        private IEnumerator CheckPlatforms()
        {
            WaitForSeconds wait = new WaitForSeconds(0.25f);
            while (true)
            {
                if (UIManager.Instance.GamePaused)
                    yield break;

                yield return wait;

                RaycastHit hitInfo;
                if (Physics.Raycast(floorDetector.position, Vector3.down, out hitInfo, detectionDistance, platformsLayer))
                {
                    if (!isGrounded)
                    {
                        isGrounded = true;
                        reachedGroundTime = Time.time;
                    }
                }
                else
                {
                    isGrounded = false;
                }
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (UIManager.Instance.GamePaused)
                return;

            this.OnTriggerEnterBase(other);
        }
    }
}