using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    [RequireComponent(typeof(Creature))]
    public abstract class StatusEffect : MonoBehaviour
    {
        private Creature creature;

        private void Awake()
        {
            creature = GetComponent<Creature>();
        }

        public abstract void Clear();
    }
}
