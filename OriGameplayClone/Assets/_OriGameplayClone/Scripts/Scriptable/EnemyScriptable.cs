using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OriProject
{
    [CreateAssetMenu(fileName = "EnemyStatus", menuName = "ScriptableObjects/EnemyStatus", order = 1)]
    public class EnemyScriptable : ScriptableObject
    {
        public float health;
        public float damage;
    }
}