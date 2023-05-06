using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Wizardry
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Prop : MonoBehaviour, Damageable
    {
        [SerializeField]
        private float maxHealth;

        [SerializeField]
        public Attack.ResistanceDictionary resistances;

        [SerializeField]
        private List<Sprite> debrisSprites;

        [SerializeField]
        private float debrisLifetime = 10f;

        private float curHealth;

        private Rigidbody2D rb;
        private Collider2D coll;
        private SpriteRenderer sr;


        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            coll = GetComponent<Collider2D>();
            sr = GetComponent<SpriteRenderer>();
        }

        protected virtual void Start()
        {
            curHealth = maxHealth;

            foreach (Attack.DamageType type in Enum.GetValues((typeof(Attack.DamageType)))) //fill in resistances not filled out in inspector, default to 1 (no resistance)
            {
                resistances.TryAdd(type, 1f);
            }
        }

        public void TakeAttack(Attack a)
        {
            float damageToTake = resistances[a.damageType] * a.damage;
            if (damageToTake > 0)
            {
                curHealth -= damageToTake;
                Knockback(a.incomingDirection * a.knockback);
            }

            if (curHealth <= 0)
            {
                Destruct(a);
            }
        }

        private void Knockback(Vector2 kb)
        {
            rb.AddForce(kb, ForceMode2D.Impulse);
        }

        private void Destruct(Attack a)
        {
            StartCoroutine(Debris(a));

            rb.isKinematic = true;
            coll.enabled = false;
            sr.enabled = false;
        }

        private IEnumerator Debris(Attack a)
        {
            GameObject[] debris = new GameObject[debrisSprites.Count];

            for(int i = 0; i < debrisSprites.Count; i++)
            {
                debris[i] = new GameObject(gameObject.name + " Debris " + i);
                debris[i].layer = 10; //sets to debris layer

                //debris inherit's destroyed object's transform
                debris[i].transform.position = gameObject.transform.position;
                debris[i].transform.rotation = gameObject.transform.rotation;
                debris[i].transform.localScale = gameObject.transform.localScale;

                SpriteRenderer newSR = debris[i].AddComponent<SpriteRenderer>();
                newSR.sprite = debrisSprites[i];

                Rigidbody2D newRB = debris[i].AddComponent<Rigidbody2D>();
                Collider2D newColl = debris[i].AddComponent<PolygonCollider2D>();

                newRB.velocity = rb.velocity;
                newRB.angularVelocity = rb.angularVelocity;


                //explode debris pieces radially
                float angleDelta = 360 / debrisSprites.Count;
                Vector2 radialForce = new Vector2(Mathf.Sin(angleDelta * i), Mathf.Cos(angleDelta * i)) * a.knockback;
                newRB.AddForce(radialForce, ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(debrisLifetime);

            foreach(GameObject d in debris)
            {
                Destroy(d);
            }

            Destroy(gameObject);
        }

    }
}
