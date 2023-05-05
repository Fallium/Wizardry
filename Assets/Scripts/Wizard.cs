using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    public class Wizard : Creature
    {
        public static Wizard player;
        public MouseObject mouse;

        [SerializeField]
        private float maxMana;
        private float curMana;
        [SerializeField]
        private float manaRegen;
        [SerializeField]
        private int maxPL;
        private int curPL = 1;
        [SerializeField]
        private float moveSpeed;
        [SerializeField]
        private float jumpSpeed;
        
        [SerializeField]
        private Spell currentSpell;

        private bool isGrounded;

        private float horizontalMoveInput;

        private Transform[] hands = new Transform[3];
        private Transform handMiddlePoint;

        [SerializeField]
        private List<MagicSchool> schools = new List<MagicSchool>();

        protected override void Awake()
        {
            //Singleton that DOESNT persist across scenes
            if (player == null)
            {
                player = this;
            }
            else if (player != this)
            {
                Destroy(gameObject);
            }
            base.Awake();

            //create mouse object
            GameObject newMouseObject = new GameObject("MouseObject");
            mouse = newMouseObject.AddComponent<MouseObject>();

            //find hands
            for(int i = 0; i < hands.Length; i++)
            {
                hands[i] = transform.Find("Hands").GetChild(i);
            }
            foreach(Transform hand in hands)
            {
                hand.gameObject.SetActive(false);
            }

            //find hand middle point, the middle point between the wizard's hands
            handMiddlePoint = hands[0].GetChild(0);

            //find spells
            Transform spellbook = transform.Find("Spellbook");
            for(int i = 0; i < spellbook.childCount; i++)
            {
                schools.Add(spellbook.GetChild(i).GetComponent<MagicSchool>());
            }

            foreach(MagicSchool skool in schools)
            {
                if (skool.spells != null)
                {
                    foreach (Spell s in skool.spells)
                    {
                        if (s.GetType() == typeof(ArcaneBowSpell))  //TEMPORARY, CHANGE ONCE SPELL SELECTING IS IN PLACE
                        {
                            currentSpell = s;
                            break;
                        }
                        
                    }
                }
            }
        }

        protected override void Update()
        {
            base.Update();

            horizontalMoveInput = Input.GetAxisRaw("Horizontal");

            ControlledMovement(HandleJumping);

            HandleSpriteFlip();

            UpdateAnim();

            HandleHands();

            //current spell check
            if(currentSpell != null)
            {
                currentSpell.SpellUpdate();
            }
        }

        private void FixedUpdate()
        {
            ControlledMovement(HandleHorizontalMovement);


            CheckIfGrounded();
        }

        private void HandleHorizontalMovement()
        {
            rb.AddForce(Vector2.right * (horizontalMoveInput * moveSpeed - rb.velocity.x), ForceMode2D.Impulse);
        }

        private void HandleJumping()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rb.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
            }
        }

        private void UpdateAnim()
        {
            anim.SetFloat("AbsoluteHorizontalSpeed", Mathf.Abs(rb.velocity.x));
            anim.SetFloat("VerticalSpeed", rb.velocity.y);
            anim.SetBool("isGrounded", isGrounded);
        }

        private void HandleSpriteFlip()
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                if (Camera.main.ScreenToWorldPoint(Input.mousePosition).x > transform.position.x)
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                if(horizontalMoveInput > 0.1)
                {
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
                }
                else if(horizontalMoveInput < -0.1)
                {
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
                }
            }
        }

        private void HandleHands()
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                foreach (Transform hand in hands)
                {
                    hand.gameObject.SetActive(true);
                    hand.up = (Vector2)mouse.transform.position - (Vector2)hand.transform.position;
                }
            }
            if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                foreach (Transform hand in hands)
                {
                    hand.gameObject.SetActive(false);
                }
            }
        }

        private void CheckIfGrounded()
        {
            RaycastHit2D[] hits = new RaycastHit2D[1];
            ContactFilter2D filter = new ContactFilter2D();
            //filter.SetLayerMask(1 << LayerMask.NameToLayer("Environment"));
            filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
            int numHits = rb.Cast(Vector2.down, filter, hits, 0.1f);
            isGrounded = numHits > 0;
        }

        public Transform GetWizardCastPoint()
        {
            return handMiddlePoint;
        }

        public int GetPowerLevel()
        {
            return curPL;
        }

        public bool HasEnoughMana(float manaDrain)
        {
            if(curMana- manaDrain < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void DrainMana(float manaDrain)
        {
            if (HasEnoughMana(manaDrain))
            {
                curMana -= manaDrain;
            }
        }
    }
}
