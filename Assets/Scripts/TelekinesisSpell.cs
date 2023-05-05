using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    public class TelekinesisSpell : Spell
    {
        [SerializeField]
        private float baseForce;
        [SerializeField]
        private float PLForceMultiplier;
        [SerializeField]
        private float dampingCoeff;
        [SerializeField]
        private float baseManaCost;
        [SerializeField]
        private float PLManaCostMultiplier;

        private Rigidbody2D targetRB;
        private Creature targetCreature;

        public override void SpellUpdate()
        {
            if (targetRB == null && Input.GetMouseButton(0)) //when mousedown and no target, find target
            {
                Collider2D potentialTargetColl = Physics2D.OverlapPoint(wizard.mouse.transform.position);
                if(potentialTargetColl != null && potentialTargetColl.attachedRigidbody != null)
                {
                    targetRB = potentialTargetColl.attachedRigidbody;
                    targetCreature = targetRB.GetComponent<Creature>();
                }
            }
            else if(targetRB != null && Input.GetMouseButton(0)) //when have target and mouse is still down, apply force to target
            {
                if(targetCreature != null)
                {
                    targetCreature.Ragdoll();
                }
                Vector2 normalizedForceVector = (wizard.mouse.transform.position - targetRB.transform.position).normalized;
                Vector2 force = baseForce * PLForceMultiplier * wizard.GetPowerLevel() * normalizedForceVector;
                Vector2 damping = -targetRB.velocity * dampingCoeff;
                targetRB.AddForce(force + damping);
            }
            else if(targetRB != null && !Input.GetMouseButton(0)) //release object
            {
                targetRB = null;
            }
        }
    }
}
