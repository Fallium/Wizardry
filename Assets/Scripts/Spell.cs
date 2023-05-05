using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    public abstract class Spell : MonoBehaviour
    {
        protected Wizard wizard;

        protected virtual void Awake()
        {
            wizard = transform.parent.parent.GetComponent<Wizard>();
        }

        public abstract void SpellUpdate();
    }
}
