using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemySlime : EnemyBase
    {
        public Transform waypointTransf1;
        public Transform waypointTransf2;

        public GameObject bulletPrefab;
        public int numOfBullets = 3;
        public Vector3 shootDir = Vector3.up;
        public float shootUpForce = 100.0f;
        public float shootHorizontalForce = 100.0f;
        public float randomHorizontalStride = 25.0f;
        public float randomVerticalStride = 25.0f;

        private Vector3 waypoint1 = Vector3.zero;
        private Vector3 waypoint2 = Vector3.zero;
        private Vector3 targetWaypoint;
        private bool pickedWaypoint1;

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
            BaseUpdateCall();
        }

        protected override void Attack()
        {
            for(int index = 0; index < numOfBullets; index++)
            {
                GameObject clone = Instantiate(bulletPrefab);
                clone.transform.position = transform.position;

                SlimeBullet script = clone.GetComponent<SlimeBullet>();

                Vector3 playerDir = (playerTransf.position - transform.position).normalized;
                float horizontalStride = Random.Range(-randomHorizontalStride, randomHorizontalStride);
                float verticalStride = Random.Range(-randomVerticalStride, randomVerticalStride);
                Vector3 propellForce = shootDir * (shootUpForce + verticalStride) + playerDir * (shootHorizontalForce + horizontalStride);

                script.Init(status.damage, propellForce);
                Destroy(clone, 5.0f);
            }
        }

        protected override void MovementLogic()
        {
            if(Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
            {
                pickedWaypoint1 = !pickedWaypoint1;
                targetWaypoint = pickedWaypoint1 ? waypoint1 : waypoint2;
            }

            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterBase(other);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, status.range);

            if (waypoint1 == Vector3.zero)
            {
                waypoint1 = waypointTransf1.position;
                waypoint2 = waypointTransf2.position;
            }

            Gizmos.color = new Color(1.0f, 0.75f, 0.0f);
            Gizmos.DrawSphere(waypoint1, 0.75f);
            Gizmos.DrawSphere(waypoint2, 0.75f);

            Gizmos.color = new Color(0.5f, 0.0f, 0.5f);
            Gizmos.DrawSphere(transform.position + shootDir * 3.0f, 0.5f);
        }
    }
}