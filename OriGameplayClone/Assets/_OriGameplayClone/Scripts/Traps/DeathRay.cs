using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class DeathRay : MonoBehaviour
    {
        public Transform firePoint;
        public Transform obstacleCheck;
        public Transform laserRay;
        public LayerMask laserLayer;
        public LayerMask obstacleLayer;

        public bool horizontalLaser;
        public float fireDuration = 5.0f;
        public float fireCooldown = 15.0f;
        public float startDelay = 0.0f;

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
                if (startDelay > 0.0f)
                    startFireTime += startDelay;
                else
                    startFireTime += Random.Range(0.0f, fireCooldown);

                laserRay.gameObject.SetActive(true);
                while (Time.time - startFireTime <= fireDuration)
                {
                    RaycastHit hitInfo;
                    RaycastHit hitInfo2;

                    bool obstaclesInFront = Physics.Raycast(new Ray(obstacleCheck.position, firePoint.forward), out hitInfo2, float.PositiveInfinity, obstacleLayer);

                    if (Physics.Raycast(new Ray(firePoint.position, firePoint.forward), out hitInfo, float.PositiveInfinity, laserLayer))
                    {
                        if(obstaclesInFront && hitInfo.collider.name != hitInfo2.collider.name)
                        {
                            laserRay.gameObject.SetActive(false);
                        }
                        else
                        {
                            desiredScale = Vector3.Distance(firePoint.position, hitInfo.point) / 2.0f + 0.25f;

                            laserRay.position = (firePoint.position + hitInfo.point) / 2.0f;
                            if (horizontalLaser)
                                laserRay.localScale = new Vector3(1.0f, desiredScale, 1.0f);
                            else
                                laserRay.localScale = new Vector3(1.0f, desiredScale, 1.0f);
                        }
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