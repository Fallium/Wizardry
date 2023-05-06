using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

namespace Wizardry
{
    [Serializable]
    public class Attack
    {
        [Serializable]
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

        [Serializable]
        public class ResistanceDictionary : ISerializationCallbackReceiver
        {
            [SerializeField]
            private List<SerializableKeyValuePair> keyValuePairs = new List<SerializableKeyValuePair>();

            private Dictionary<Attack.DamageType, float> dictionary = new Dictionary<Attack.DamageType, float>();

            public float this[Attack.DamageType key]
            {
                get => dictionary[key];
                set => dictionary[key] = value;
            }
            public bool TryAdd(Attack.DamageType key, float value)
            {
                return dictionary.TryAdd(key, value);
            }

            public bool TryGetValue(Attack.DamageType key, out float value)
            {
                return dictionary.TryGetValue(key, out value);
            }

            public void OnBeforeSerialize()
            {
                keyValuePairs.Clear();
                foreach (var pair in dictionary)
                {
                    keyValuePairs.Add(new SerializableKeyValuePair(pair.Key, pair.Value));
                }
            }

            public void OnAfterDeserialize()
            {
                dictionary = new Dictionary<Attack.DamageType, float>();
                foreach (var pair in keyValuePairs)
                {
                    dictionary.Add(pair.Key, pair.Value);
                }
            }

            [Serializable]
            private class SerializableKeyValuePair
            {
                [SerializeField]
                private Attack.DamageType key;
                [SerializeField]
                private float value;

                public SerializableKeyValuePair(Attack.DamageType key, float value)
                {
                    this.key = key;
                    this.value = value;
                }

                public Attack.DamageType Key => key;
                public float Value => value;
            }
        }

    }
}
