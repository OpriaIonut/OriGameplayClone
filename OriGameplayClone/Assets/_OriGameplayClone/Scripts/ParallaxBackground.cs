using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class ParallaxBackground : MonoBehaviour
    {
        private List<Transform> parallaxItems = new List<Transform>();
        private Transform cameraTransf;

        private Vector3 previousCamPos;

        private void Start()
        {
            foreach(Transform transf in transform)
            {
                parallaxItems.Add(transf);
            }
            cameraTransf = Camera.main.transform;
            previousCamPos = cameraTransf.position;
        }

        private void Update()
        {
            if (UIManager.Instance.GamePaused)
                return;

            float effectPerItem = 1.0f / (parallaxItems.Count - 1);

            for (int index = 0; index < parallaxItems.Count; index++)
            {
                float moveAmount = index * effectPerItem; 
                float parallax = (previousCamPos.x - cameraTransf.position.x) * moveAmount * 2.5f;

                float bgTargetPosX = parallaxItems[index].position.x + parallax;
                Vector3 bgTargetPos = new Vector3(bgTargetPosX, parallaxItems[index].position.y, parallaxItems[index].position.z);

                parallaxItems[index].position = Vector3.Lerp(parallaxItems[index].position, bgTargetPos, 10.0f * Time.deltaTime);
            }
            previousCamPos = cameraTransf.position;
        }
    }
}