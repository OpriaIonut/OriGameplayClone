using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private List<EnemyColony> enemyCells;
        private List<RectTransform> enemyMarkers;

        #region Singleton

        private static Minimap instance;
        public static Minimap Instance { get { return instance; } }

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogWarning("Multiple instances for Minimap. Removing from " + gameObject.name);
                Destroy(this);
            }
            else
                instance = this;
        }

        #endregion

        private void Start()
        {
            enemyCells = FindObjectsOfType<EnemyColony>().ToList();

            enemyMarkers = new List<RectTransform>();
            for(int index = 0; index < enemyCells.Count; index++)
            {
                GameObject clone = Instantiate(enemyMarkerPrefab, enemyMarkerParent);
                clone.name = enemyCells[index].name;
                enemyMarkers.Add(clone.GetComponent<RectTransform>());
            }
            markerMaxRadius = enemyMarkerParent.rect.width / 2.0f;
        }

        private void Update()
        {
            for(int index = 0; index < enemyCells.Count; index++)
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

        public void ColonyDeath(EnemyColony obj, EnemyColony spawnedChild1, EnemyColony spawnedChild2)
        {
            for(int index = 0; index < enemyCells.Count; index++)
            {
                if(enemyCells[index] == obj)
                {
                    enemyCells.RemoveAt(index);
                    Destroy(enemyMarkers[index].gameObject);
                    enemyMarkers.RemoveAt(index);
                    break;
                }
            }
            if(spawnedChild1 != null)
            {
                enemyCells.Add(spawnedChild1);
                GameObject clone = Instantiate(enemyMarkerPrefab, enemyMarkerParent);
                clone.name = spawnedChild1.name;
                enemyMarkers.Add(clone.GetComponent<RectTransform>());
            }
            if (spawnedChild2 != null)
            {
                enemyCells.Add(spawnedChild2);
                GameObject clone = Instantiate(enemyMarkerPrefab, enemyMarkerParent);
                clone.name = spawnedChild2.name;
                enemyMarkers.Add(clone.GetComponent<RectTransform>());
            }
        }
    }
}