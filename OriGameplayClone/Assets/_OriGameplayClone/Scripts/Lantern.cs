using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class Lantern : PropellTarget
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

        protected override void OnPlayerEnteredRange()
        {
            base.OnPlayerEnteredRange();
            lanternMat.color = Color.cyan;
            lanternMat.SetColor("_EmissionColor", Color.cyan);
        }

        protected override void OnPlayerExitedRange()
        {
            base.OnPlayerExitedRange();
            lanternMat.color = initColor;
            lanternMat.SetColor("_EmissionColor", initEmissionColor);
        }
    }
}