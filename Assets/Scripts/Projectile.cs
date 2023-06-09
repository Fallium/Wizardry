using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{

    [RequireComponent(typeof(Collider2D))]
    public class Projectile : MonoBehaviour
    {
        protected Rigidbody2D rb;
        protected Collider2D coll;

        protected float damage;
        protected Attack.DamageType damageType;
        protected float knockback;

        protected int pierce;

        protected LayerMask collisionMask;
        protected float lifetime;

        protected bool velocityAligned;

        protected Vector2 lastFrameVelocity;

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
        }

        public void SetInitialValues(Vector2 initialVelocity, float _damage, Attack.DamageType _damageType, float _knockback, int _pierce, LayerMask _collisionMask, float _lifetime, bool _velocityAligned)
        {
            rb.velocity = initialVelocity;
            damage = _damage;
            damageType = _damageType;
            knockback = _knockback;
            pierce = _pierce;
            collisionMask = _collisionMask;
            lifetime = _lifetime;
            velocityAligned = _velocityAligned;
        }

        protected virtual void Update()
        {
            HandleLifetime();
            HandleVelocityAlignment();

            lastFrameVelocity = rb.velocity;
        }

        protected virtual void HandleLifetime()
        {
            lifetime -= Time.deltaTime;
            if(lifetime <= 0)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void HandleVelocityAlignment()
        {
            if (velocityAligned)
            {
                transform.right = rb.velocity;
            }
        }

        protected virtual bool DamageIfDamageable(Collision2D collision)
        {
            Damageable damaged = collision.gameObject.GetComponent<Damageable>();
            if (damaged != null) //if target is damageable
            {
                damaged.TakeAttack(new Attack(damage, damageType, lastFrameVelocity, knockback));
                return true;
            }
            return false;
        }

        protected virtual void OnCollisionEnter2D(Collision2D collision)
        {
            DamageIfDamageable(collision);

            Destroy(gameObject);
        }
    }
}
