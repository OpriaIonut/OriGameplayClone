using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    [RequireComponent(typeof(Rigidbody))]
    public class PropellTarget : MonoBehaviour
    {
        public float propellForce = 100.0f;
        public Transform targetTransf;

        public virtual void LaunchTarget(Vector3 direction)
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (propellForce > 0.0f && Vector3.Distance(direction, Vector3.zero) > 0.01)
            {
                Rigidbody rigidbody = targetTransf.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.velocity = Vector3.zero;
                rigidbody.AddForce(direction * propellForce);
            }
        }

        public virtual void OnPlayerEnteredRange()
        {

        }

        public virtual void OnPlayerExitedRange()
        {

        }
    }
}