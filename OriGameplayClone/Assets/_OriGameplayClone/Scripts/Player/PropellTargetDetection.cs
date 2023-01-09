using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class PropellTargetDetection : MonoBehaviour
    {
        private CharacterMovement playerScript;
        private PlayerLogic playerLogic;

        private void Start()
        {
            playerScript = transform.root.GetComponent<CharacterMovement>();
            playerLogic = transform.root.GetComponent<PlayerLogic>();
        }

        private void OnTriggerEnter(Collider other)
        {
            PropellTarget target = other.gameObject.GetComponent<PropellTarget>();
            if (target == null)
                target = other.transform.root.GetComponent<PropellTarget>();

            if (target)
            {
                target.OnPlayerEnteredRange();
                playerScript.EnteredPropellTargetRange(target);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PropellTarget target = other.gameObject.GetComponent<PropellTarget>();
            if (target == null)
                target = other.transform.root.GetComponent<PropellTarget>();

            if (target)
            {
                target.OnPlayerExitedRange();
                playerScript.ExitPropellTargetRange(target);
            }
        }
    }
}