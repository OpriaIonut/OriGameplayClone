using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class PropellTarget : MonoBehaviour
    {
        public float propellForce = 100.0f;
        public Transform targetTransf;

        public virtual void LaunchTarget(Vector3 direction)
        {
            Rigidbody rigidbody = targetTransf.GetComponent<Rigidbody>();
            rigidbody.isKinematic = false;
            rigidbody.AddForce(direction * propellForce);
        }

        public virtual void OnPlayerEnteredRange()
        {

        }

        public virtual void OnPlayerExitedRange()
        {

        }
    }
}