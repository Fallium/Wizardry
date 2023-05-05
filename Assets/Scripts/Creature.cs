using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(Animator))]

    public abstract class Creature : MonoBehaviour, Damageable
    {
        protected enum resistanceIndex
        {
            Physical,
            Arcane,
            Fire,
            Electric,
            Impact,
            Force
        }

        protected delegate void MoveFunction();

        protected Rigidbody2D rb;
        protected Collider2D coll;
        protected Animator anim;

        [SerializeField]
        private float maxHealth;
        private float curHealth;

        [SerializeField]
        private float[] resistances;

        protected bool ragdoll;
        
        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            anim = GetComponent<Animator>();
        }

        private void Start()
        {
            curHealth = maxHealth;
            if(resistances == null)
            {
                resistances = new float[6];
                for (int i = 0; i < resistances.Length; i++)
                {
                    resistances[i] = 1f;
                }
            }
        }

        protected virtual void Update()
        {
            HandleRagdoll();
        }

        protected void ControlledMovement(MoveFunction move)
        {
            if (!ragdoll)
            {
                move();
            }
        }

        private void HandleRagdoll()
        {
            if (ragdoll)
            {
                ragdoll = rb.velocity.magnitude > 1f;
            }
        }

        public void Ragdoll()
        {
            ragdoll = true;
        }

        private void Knockback(Vector2 kb)
        {
            ragdoll = true;
            rb.AddForce(kb, ForceMode2D.Impulse);
        }
        
        public void TakeAttack(Attack a)
        {
            float damageToTake = resistances[(int)a.damageType] * a.damage;
            if(damageToTake > 0)
            {
                curHealth -= damageToTake;
                Knockback(a.incomingDirection * a.knockback);
            }
            
            if(curHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}
