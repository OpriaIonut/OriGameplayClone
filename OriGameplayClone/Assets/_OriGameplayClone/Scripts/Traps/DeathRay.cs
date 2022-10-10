using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class DeathRay : MonoBehaviour
    {
        public Transform firePoint;
        public Transform laserRay;
        public LayerMask laserLayer;

        public bool horizontalLaser;
        public float fireDuration = 5.0f;
        public float fireCooldown = 15.0f;

        private float desiredScale = 0.0f;

        private void Start()
        {
            StartCoroutine(FireLaser());
        }

        public IEnumerator FireLaser()
        {
            while (true)
            {
                float startFireTime = Time.time;
                laserRay.gameObject.SetActive(true);
                while (Time.time - startFireTime <= fireDuration)
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(new Ray(firePoint.position, firePoint.forward), out hitInfo, float.PositiveInfinity, laserLayer))
                    {
                        desiredScale = Vector3.Distance(firePoint.position, hitInfo.point) / 2.0f + 0.25f;

                        laserRay.position = (firePoint.position + hitInfo.point) / 2.0f;
                        if (horizontalLaser)
                            laserRay.localScale = new Vector3(1.0f, desiredScale, 1.0f);
                        else
                            laserRay.localScale = new Vector3(1.0f, desiredScale, 1.0f);
                    }
                    else
                        Debug.LogWarning("Raycast failed for: " + gameObject.name);

                    yield return new WaitForSeconds(0.1f);
                }
                laserRay.gameObject.SetActive(false);

                yield return new WaitForSeconds(fireCooldown);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.tag == "PlayerHitbox")
            {
                PlayerLogic script = other.transform.root.GetComponent<PlayerLogic>();
                if(script)
                {
                    script.TakeDamage(1000, transform);
                }
            }
        }
    }
}