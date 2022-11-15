using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemyWorm : EnemyBase
    {
        public Transform firePoint;
        public GameObject bulletPrefab;

        private void Start()
        {
            BaseStartCall();
        }

        private void Update()
        {
            BaseUpdateCall();
        }

        protected override void Attack()
        {
            //float xPlayerDir = playerTransf.position.x - transform.position.x;
            //float rotAngle = xPlayerDir > 0.0f ? 0.0f : -180.0f;
            //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotAngle, transform.rotation.eulerAngles.z);

            GameObject clone = Instantiate(bulletPrefab);
            clone.transform.position = firePoint.position;

            PukingFrogBullet bulletScript = clone.GetComponent<PukingFrogBullet>();
            bulletScript.Init(clone.transform.position, playerTransf.position, status.damage);

            Destroy(clone, 5.0f);
        }

        protected override void MovementLogic()
        {
            //Intentionally left empty
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterBase(other);
        }
    }
}