using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemyArachne : EnemyBase
    {
        public Transform gfx;
        public GameObject bulletPrefab;

        private LineRenderer rope;

        private void Start()
        {
            BaseStartCall();

            rope = GetComponent<LineRenderer>();
            Vector3 diff = transform.position - gfx.position;
            gfx.GetComponent<HingeJoint>().anchor = new Vector3(diff.x, 0.0f, diff.y);
        }

        private void Update()
        {
            BaseUpdateCall();

            rope.SetPosition(0, transform.position);
            rope.SetPosition(1, gfx.position);
        }

        protected override IEnumerator FindPlayerRange()
        {
            while (true)
            {
                if (Vector3.Distance(playerTransf.position, gfx.position) < status.range)
                {
                    isPlayerInRange = true;
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

        protected override void Attack()
        {
            GameObject clone = Instantiate(bulletPrefab);
            clone.transform.position = gfx.transform.position;

            ArachneBullet script = clone.GetComponent<ArachneBullet>();
            script.Init(playerTransf.position, status.damage);

            Destroy(clone, 15.0f);
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterBase(other);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(gfx.position, status.range);
        }
    }
}