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
            if (UIManager.Instance.GamePaused)
                return;

            rope.SetPosition(0, transform.parent.position);
            rope.SetPosition(1, transform.position);
        }

        public override void OnPlayerEnteredRange()
        {
            if (UIManager.Instance.GamePaused)
                return;

            base.OnPlayerEnteredRange();
            lanternMat.color = Color.cyan;
            lanternMat.SetColor("_EmissionColor", Color.cyan);
        }

        public override void OnPlayerExitedRange()
        {
            if (UIManager.Instance.GamePaused)
                return;

            base.OnPlayerExitedRange();
            lanternMat.color = initColor;
            lanternMat.SetColor("_EmissionColor", initEmissionColor);
        }
    }
}