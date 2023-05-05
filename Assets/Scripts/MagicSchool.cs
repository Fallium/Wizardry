using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    public class MagicSchool : MonoBehaviour
    {
        public string schoolName { get; private set; }
        public Spell[] spells; //{ get; private set; }

        public ParticleSystem castParticles;

        private void Awake()
        {
            spells = GetComponents<Spell>();
            schoolName = name.Replace("School", "");
        }
    }
}
