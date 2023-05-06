using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    public class ArcaneArrow : Projectile
    {
        protected override void OnCollisionEnter2D(Collision2D collision)
        {
            bool wasDamageable = DamageIfDamageable(collision);

            transform.parent = collision.transform;
            transform.localScale = new Vector3(1/transform.parent.localScale.x, 1/transform.parent.localScale.y, 1/transform.parent.localScale.z);
            Destroy(rb);
            coll.enabled = false;

            if (!wasDamageable)
            {
                StartCoroutine(GameManager.TimedDestruct(gameObject, 10f));
            }

            Destroy(this);
        }
    }
}
