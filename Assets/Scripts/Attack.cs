using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Wizardry
{
    [Serializable]
    public class Attack
    {
        public enum DamageType
        {
            Physical,
            Arcane,
            Fire,
            Electric,
            Impact,
        }

        public float damage;
        public DamageType damageType;
        public Vector2 incomingDirection;
        public float knockback;

        public Attack(float _damage, DamageType _damageType, Vector2 _incomingDirection, float _knockback)
        {
            damage = _damage;
            damageType = _damageType;
            incomingDirection = _incomingDirection.normalized;
            knockback = _knockback;
        }
    }
}
