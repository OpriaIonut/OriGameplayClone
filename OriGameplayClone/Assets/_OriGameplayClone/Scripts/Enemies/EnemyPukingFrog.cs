using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemyPukingFrog : EnemyBase
    {
        public float moveDelay = 3.0f;
        public Transform waypointTransf1;
        public Transform waypointTransf2;
        public Transform firePoint;
        public GameObject bulletPrefab;
        public bool canMove = true;

        private Vector3 waypoint1 = Vector3.zero;
        private Vector3 waypoint2 = Vector3.zero;
        private Vector3 targetWaypoint;
        private bool pickedWaypoint1;
        private float pickTargetTime = 0.0f;

        private void Start()
        {
            BaseStartCall();

            waypoint1 = waypointTransf1.position;
            waypoint2 = waypointTransf2.position;

            pickedWaypoint1 = Random.Range(0.0f, 1.0f) < 0.5f;
            targetWaypoint = pickedWaypoint1 ? waypoint1 : waypoint2;
        }

        private void Update()
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (isPlayerInRange && Time.time - lastAttackTime > status.attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }

        private void FixedUpdate()
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (canMove)
                MovementLogic();
        }

        protected override void MovementLogic()
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (Time.time - pickTargetTime < moveDelay)
                return;

            if(isPlayerInRange)
            {
                rb.velocity = new Vector3(0.0f, rb.velocity.y, rb.velocity.z);
                return;
            }

            if (Vector3.Distance(transform.position, targetWaypoint) < 1.0f)
            {
                pickedWaypoint1 = !pickedWaypoint1;
                targetWaypoint = pickedWaypoint1 ? waypoint1 : waypoint2;
                pickTargetTime = Time.time;
                rb.velocity = new Vector3(0.0f, rb.velocity.y, rb.velocity.z);
                return;
            }

            float xVel = targetWaypoint.x - transform.position.x;
            xVel = xVel > 0.0f ? 1.0f : -1.0f;
            rb.velocity = new Vector3(xVel * speed * Time.fixedDeltaTime, rb.velocity.y, rb.velocity.z);

            float rotAngle = rb.velocity.x > 0.0f ? 0.0f : -180.0f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotAngle, transform.rotation.eulerAngles.z);
        }

        protected override void Attack()
        {
            float xPlayerDir = playerTransf.position.x - transform.position.x;
            float rotAngle = xPlayerDir > 0.0f ? 0.0f : -180.0f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotAngle, transform.rotation.eulerAngles.z);

            GameObject clone = Instantiate(bulletPrefab);
            clone.transform.position = firePoint.position;

            PukingFrogBullet bulletScript = clone.GetComponent<PukingFrogBullet>();
            bulletScript.Init(transform.position, playerTransf.position, status.damage);

            Destroy(clone, 5.0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (UIManager.Instance.GamePaused)
                return;

            OnTriggerEnterBase(other);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, status.range);

            if (!canMove)
                return;

            if (waypoint1 == Vector3.zero)
            {
                waypoint1 = waypointTransf1.position;
                waypoint2 = waypointTransf2.position;
            }

            Gizmos.color = new Color(1.0f, 0.75f, 0.0f);
            Gizmos.DrawSphere(waypoint1, 0.75f);
            Gizmos.DrawSphere(waypoint2, 0.75f);
        }
    }
}