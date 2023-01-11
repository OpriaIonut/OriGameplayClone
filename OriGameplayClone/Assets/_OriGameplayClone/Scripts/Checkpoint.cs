using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class Checkpoint : MonoBehaviour
    {
        public ParticleSystem normalParticle;
        public ParticleSystem savedParticle;

        public string name = "";
        private static int counter = 0;

        private bool changedParticles = false;
        public bool wasUsed { get { return changedParticles; } }

        private void Start()
        {
            name = "Checkpoint" + counter;
            counter++;
        }

        public void ChangeParticleDisplay()
        {
            normalParticle.gameObject.SetActive(false);
            savedParticle.gameObject.SetActive(true);
            savedParticle.Play();

            changedParticles = true;
            FindObjectOfType<PlayerLogic>().CheckpointInRange(false, null);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (changedParticles == false && other.name == "GFX")
            {
                PlayerLogic target = other.transform.root.GetComponent<PlayerLogic>();
                if (target)
                {
                    target.CheckpointInRange(true, this);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (changedParticles == false && other.name == "GFX")
            {
                PlayerLogic target = other.transform.root.GetComponent<PlayerLogic>();
                if (target)
                {
                    target.CheckpointInRange(false, null);
                }
            }
        }
    }
}