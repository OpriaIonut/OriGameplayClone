using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class Checkpoint : MonoBehaviour
    {
        public ParticleSystem normalParticle;
        public ParticleSystem savedParticle;

        private Transform playerTransf;

        private void Start()
        {
            playerTransf = FindObjectOfType<PlayerLogic>().transform;
            StartCoroutine(CustomUpdate());
        }

        private IEnumerator CustomUpdate()
        {
            while(true)
            {
                if(Vector3.Distance(playerTransf.position, transform.position) < 1.5f)
                {
                    yield return new WaitForSeconds(3.0f);

                    if(Vector3.Distance(playerTransf.position, transform.position) < 1.5f)
                    {
                        SaveGame();
                        normalParticle.gameObject.SetActive(false);
                        savedParticle.gameObject.SetActive(true);
                        savedParticle.Play();
                        break;
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void SaveGame()
        {

        }
    }
}