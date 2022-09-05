using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : MonoBehaviour
{
    private Material lanternMat;
    private Color initColor;
    private Color initEmissionColor;
    private LineRenderer rope;

    private void Start()
    {
        rope = transform.parent.GetComponent<LineRenderer>();
        lanternMat = GetComponent<MeshRenderer>().material;
        initColor = lanternMat.color;
        initEmissionColor = lanternMat.GetColor("_EmissionColor");
    }

    private void Update()
    {
        rope.SetPosition(0, transform.parent.position);
        rope.SetPosition(1, transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.root.tag == "Player")
        {
            CharacterMovement playerMovement = other.transform.root.GetComponent<CharacterMovement>();
            playerMovement.EnteredLanternRange(transform);
            lanternMat.color = Color.cyan;
            lanternMat.SetColor("_EmissionColor", Color.cyan);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.transform.root.tag == "Player")
        {
            CharacterMovement playerMovement = other.transform.root.GetComponent<CharacterMovement>();
            playerMovement.ExitLanternRange();
            lanternMat.color = initColor;
            lanternMat.SetColor("_EmissionColor", initEmissionColor);
        }
    }
}
