using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    public class MagicMissile : Spell
    {
        [SerializeField]
        private float fireRate;

        [SerializeField]
        private Projectile missileProjectile;
        public override void SpellUpdate()
        {

        }

        private void Fire()
        {
            Wizard.player.GetWizardCastPoint();
        }
    }
}
