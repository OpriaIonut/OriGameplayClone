using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemyRhino : EnemyBase
    {
        public Transform wallDetector;
        public float detectionDistance = 0.15f;
        public LayerMask platformsLayer;

        public float maxMoveDistance = 25.0f;

        private float horizontalMoveDir = 1.0f;
        private Vector3 moveStartPos = Vector3.zero;

        private bool startedMovement = false;
        private float movementEndTime = 0.0f;

        private bool hitWall = false;
        private float wallDir = 0.0f;

        private void Start()
        {
            BaseStartCall();
        }

        private void Update()
        {
            if (UIManager.Instance.GamePaused)
                return;

            BaseUpdateCall();
        }

        protected override void Attack()
        {
            //Intentionally left empty
        }

        protected override void MovementLogic()
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (startedMovement && Time.time - movementEndTime > status.attackCooldown)
            {
                transform.position += Vector3.right * horizontalMoveDir * speed * Time.deltaTime;

                float rotAngle = horizontalMoveDir > 0.0f ? 0.0f : -180.0f;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotAngle, transform.rotation.eulerAngles.z);

                if (Vector3.Distance(transform.position, moveStartPos) > maxMoveDistance)
                {
                    startedMovement = false;
                    movementEndTime = Time.time;
                }
            }
        }

        protected override IEnumerator FindPlayerRange()
        {
            while (true)
            {
                if (UIManager.Instance.GamePaused)
                    yield break;

                if (Vector3.Distance(playerTransf.position, transform.position) < status.range)
                {
                    isPlayerInRange = true;
                    if (!startedMovement && Time.time - movementEndTime > status.attackCooldown)
                    {
                        startedMovement = true;
                        moveStartPos = transform.position;
                        horizontalMoveDir = playerTransf.position.x > transform.position.x ? 1.0f : -1.0f;

                        float rotAngle = horizontalMoveDir > 0.0f ? 0.0f : -180.0f;
                        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotAngle, transform.rotation.eulerAngles.z);

                        CheckPlatforms();
                    }
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

        private void CheckPlatforms()
        {
            if (UIManager.Instance.GamePaused)
                return;

            RaycastHit hitInfo;
            if (Physics.Raycast(wallDetector.position, wallDetector.forward, out hitInfo, detectionDistance, platformsLayer))
            {
                movementEndTime = Time.time;
                startedMovement = false;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (UIManager.Instance.GamePaused)
                return;

            OnTriggerEnterBase(other);

            if(other.tag == "ClimbableWall" || other.tag == "UnclimbableWall")
            {
                movementEndTime = Time.time;
                startedMovement = false;
            }
        }
    }
}