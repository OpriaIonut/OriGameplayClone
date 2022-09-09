using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class PropellTarget: MonoBehaviour
{
    public float propellForce = 100.0f;

    public virtual void LaunchTarget(Vector3 direction)
    {
        GetComponent<Rigidbody>().AddForce(direction * propellForce);
    }

    protected virtual void OnPlayerEnteredRange()
    {

    }

    protected virtual void OnPlayerExitedRange()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            CharacterMovement playerMovement = other.transform.root.GetComponent<CharacterMovement>();
            playerMovement.EnteredPropellTargetRange(this);
            OnPlayerEnteredRange();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.tag == "Player")
        {
            CharacterMovement playerMovement = other.transform.root.GetComponent<CharacterMovement>();
            playerMovement.ExitPropellTargetRange();
            OnPlayerExitedRange();
        }
    }
}