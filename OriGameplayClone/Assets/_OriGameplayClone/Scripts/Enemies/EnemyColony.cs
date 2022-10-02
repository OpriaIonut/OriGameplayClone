using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class EnemyColony : EnemyBase
    {
        public bool jumpMovement = true;
        public bool spawnChildOnDeath = true;
        public GameObject child;
        public float delayBetweenMovements = 1.0f;

        private bool isGrounded = false;
        private float groundHitTime = 0.0f;

        private float onStartRightForce = 1.0f;

        private void Start()
        {
            BaseStartCall();
        }

        private void Update()
        {
            BaseUpdateCall();
        }

        public void OnSpawned(bool propellRight)
        {
            onStartRightForce = propellRight ? 1.0f : -1.0f;
            Invoke("StartupForce", 0.1f);
        }

        public void StartupForce()
        {
            float upRandom = Random.Range(0.9f, 1.1f);
            float rightRandom = Random.Range(0.9f, 1.1f);
            rb.AddForce(Vector3.up * 350f * upRandom + onStartRightForce * Vector3.right * 100f * rightRandom);
        }

        protected override void Die()
        {
            if(spawnChildOnDeath)
            {
                GameObject clone1 = Instantiate(child);
                clone1.transform.position = transform.position;
                EnemyColony script = clone1.GetComponent<EnemyColony>();
                script.OnSpawned(true);

                GameObject clone2 = Instantiate(child);
                clone2.transform.position = transform.position;
                EnemyColony script2 = clone2.GetComponent<EnemyColony>();
                script2.OnSpawned(false);
            }

            Destroy(gameObject);
        }

        protected override void MovementLogic()
        {
            if (isPlayerInRange && isGrounded)
            {
                if (jumpMovement)
                {
                    if (Time.time - groundHitTime > delayBetweenMovements)
                    {
                        isGrounded = false;
                        float playerDir = (playerTransf.position.x - transform.position.x) / status.range;
                        float factor = Mathf.Abs(playerDir) + Random.Range(-0.1f, 0.1f);
                        factor = Mathf.Clamp01(factor);
                        playerDir = Mathf.Lerp(playerDir * speed, playerDir * speed * 2f, 1.0f - factor);
                        rb.AddForce(Vector3.up * 350f + playerDir * Vector3.right);
                    }
                }
                else
                {
                    float playerDir = playerTransf.position.x - transform.position.x > 0 ? 1.0f : -1.0f;
                    transform.Translate(Vector3.right * playerDir * speed * 0.015f * Time.deltaTime);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterBase(other);

            if (other.tag == "GroundPlatform" || other.tag == "BreakablePlatform")
            {
                isGrounded = true;
                groundHitTime = Time.time;
                rb.velocity = Vector3.zero;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if(other.tag == "GroundPlatform" || other.tag == "BreakablePlatform")
            {
                isGrounded = false;
            }
        }
    }
}