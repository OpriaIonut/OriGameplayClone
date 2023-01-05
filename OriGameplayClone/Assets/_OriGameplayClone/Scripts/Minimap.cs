using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class Minimap : MonoBehaviour
    {
        public GameObject enemyMarkerPrefab;
        public RectTransform enemyMarkerParent;
        public Camera minimapCam;

        private float mapWidth = 250.0f;
        private float markerMaxRadius = 0;
        private EnemyColony[] enemyCells;
        private List<RectTransform> enemyMarkers;

        private void Start()
        {
            enemyCells = FindObjectsOfType<EnemyColony>();

            enemyMarkers = new List<RectTransform>();
            for(int index = 0; index < enemyCells.Length; index++)
            {
                GameObject clone = Instantiate(enemyMarkerPrefab, enemyMarkerParent);
                clone.name = enemyCells[index].name;
                enemyMarkers.Add(clone.GetComponent<RectTransform>());
            }
            markerMaxRadius = enemyMarkerParent.rect.width / 2.0f;
        }

        private void Update()
        {
            for(int index = 0; index < enemyCells.Length; index++)
            {
                Vector3 screenPos = minimapCam.WorldToScreenPoint(enemyCells[index].transform.position);
                screenPos.z = 0.0f;

                screenPos.x = Mathf.Clamp(screenPos.x, 0.0f, mapWidth);
                screenPos.y = Mathf.Clamp(screenPos.y, 0.0f, mapWidth);

                screenPos.x = (screenPos.x / mapWidth) * 2.0f - 1.0f;
                screenPos.y = (screenPos.y / mapWidth) * 2.0f - 1.0f;

                Vector3 markerPos = screenPos * markerMaxRadius;

                if (markerPos.magnitude > markerMaxRadius)
                    markerPos = screenPos.normalized * markerMaxRadius;
                
                enemyMarkers[index].localPosition = markerPos;
            }
        }
    }
}