using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    public class MovingWallCollision : MonoBehaviour
    {
        private MovingWalls script;

        private void Start()
        {
            script = transform.root.GetComponent<MovingWalls>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (UIManager.Instance.GamePaused)
                return;

            if (other.tag == "PlayerHitbox")
            {
                PlayerLogic logic = other.transform.root.GetComponent<PlayerLogic>();
                if (logic)
                {
                    script.OnPlayerEnter(logic);
                }
            }
        }
    }
}