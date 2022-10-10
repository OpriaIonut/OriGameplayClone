using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class MovingWalls : MonoBehaviour
    {
        public bool isRightMoving = true;
        public bool isLeftMoving = true;
        public bool isVerticalTrap = false;

        public Transform rightWall;
        public Transform rightHandle;
        public BoxCollider rightTrigger;

        public Transform leftWall;
        public Transform leftHandle;
        public BoxCollider leftTrigger;

        public float wallWidth = 1.0f;
        public float moveSpeed = 1.0f;
        public float closeTriggerActivateDist = 2.0f;
        public float endMovementWaitTime = 3.0f;

        private Vector3 initialLeftHandle;
        private Vector3 initialRightHandle;

        private float moveDirection = 1.0f; //1 means that it is closing, -1 means that it is opening
        private bool reachedEnd = false;
        private float reachEndTime = 0.0f;
        private float initialDistance;

        private void Start()
        {
            leftTrigger.enabled = false;
            rightTrigger.enabled = false;

            initialDistance = Vector3.Distance(rightWall.position, leftWall.position);

            initialLeftHandle = leftHandle.position;
            initialRightHandle = rightHandle.position;
        }

        private void Update()
        {
            if (reachedEnd && Time.time - reachEndTime < endMovementWaitTime)
                return;
            else
            {
                reachedEnd = false;
            }

            Vector3 moveDir = Vector3.right;
            if (isVerticalTrap)
                moveDir = Vector3.up;
            if (isRightMoving)
            {
                rightWall.Translate(-1.0f * moveDir * moveDirection * moveSpeed * Time.deltaTime);
            }
            if(isLeftMoving)
            {
                leftWall.Translate(moveDir * moveDirection * moveSpeed * Time.deltaTime);
            }

            float distance = Vector3.Distance(rightWall.position, leftWall.position);
            bool activateTriggers = distance < (closeTriggerActivateDist + wallWidth);

            leftTrigger.enabled = activateTriggers;
            rightTrigger.enabled = activateTriggers;

            float stretch = (initialDistance - distance) / 2.0f + 1.0f;
            if (isRightMoving)
            {
                if (!isLeftMoving)
                    stretch *= 2.0f;
                rightHandle.position = (rightWall.position + initialRightHandle + wallWidth * moveDir * 0.65f) / 2.0f;

                if(!isVerticalTrap)
                    rightHandle.localScale = new Vector3(stretch, rightHandle.localScale.y, rightHandle.localScale.z);
                else
                    rightHandle.localScale = new Vector3(rightHandle.localScale.x, stretch, rightHandle.localScale.z);
            }
            if (isLeftMoving)
            {
                if (!isRightMoving)
                    stretch *= 2.0f;
                leftHandle.position = (leftWall.position + initialLeftHandle - wallWidth * moveDir * 0.65f) / 2.0f;

                if(!isVerticalTrap)
                    leftHandle.localScale = new Vector3(stretch, leftHandle.localScale.y, leftHandle.localScale.z);
                else
                    leftHandle.localScale = new Vector3(leftHandle.localScale.x, stretch, leftHandle.localScale.z);
            }

            if(activateTriggers)
                Debug.DrawLine(leftWall.position, rightWall.position, Color.blue);

            if (moveDirection > 0.0f && distance < wallWidth)
            {
                reachedEnd = true;
                reachEndTime = Time.time;
                moveDirection *= -1.0f;
            }
            else if(moveDirection < 0.0f && distance > initialDistance)
            {
                reachedEnd = true;
                reachEndTime = Time.time;
                moveDirection *= -1.0f;
            }
        }

        public void OnPlayerEnter(PlayerLogic script)
        {
            script.TakeDamage(1000, transform);
        }
    }
}