using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    public class ArcaneBowSpell : Spell
    {
        [SerializeField]
        private GameObject ArcaneBowPrefab;
        private float bowRadius;
        [SerializeField]
        private Projectile arrowPrefab;
        [SerializeField]
        private LineRenderer drawLinePrefab;

        [SerializeField]
        private float fullDrawTime;
        [SerializeField]
        private float maxArrowSpeed;
        [SerializeField]
        private float maxArrowDamage;
        [SerializeField]
        private float maxArrowKnockback;
        [SerializeField]
        private float bowRotationSpeed; //in degrees per second
        [SerializeField]
        private float mouseMaxDrawDistance;

        private float drawPercent = 0f;

        private List<GameObject> currentSwipeBows = new List<GameObject>(); //bows spawned in currentmousedown

        private List<GameObject> activeBows = new List<GameObject>();
        private List<Animator> activeBowsAnim = new List<Animator>();

        [SerializeField]
        private int fireMode = 0;

        protected override void Awake()
        {
            base.Awake();
            bowRadius = ArcaneBowPrefab.transform.GetChild(0).GetComponent<CircleCollider2D>().radius;
        }
        public override void SpellUpdate()
        {
            HandleBowSpawning();
            HandleBowDestroying();
            switch (fireMode)
            {
                case 0:
                    FireModeConcentrated();
                    break;
                case 1:
                    FireModeVolley();
                    break;
            }
            UpdateBowsAnimator();
        }

        private void BowsPointTowardsMouse()
        {
            foreach(GameObject bow in activeBows) //update bows rotation
            {
                if ((wizard.mouse.transform.position - bow.transform.position).sqrMagnitude > Mathf.Pow(bowRadius, 2))
                {
                    //bow.transform.parent.right = wizard.mouse.transform.position - bow.transform.parent.position;
                    bow.transform.parent.rotation = Quaternion.RotateTowards(bow.transform.parent.rotation, Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.right, wizard.mouse.transform.position - bow.transform.parent.position), Vector3.forward), bowRotationSpeed * Time.deltaTime);
                }
            }
        }

        private void UpdateBowsAnimator()
        {
            foreach (Animator bowAnim in activeBowsAnim) //update bows animator
            {
                bowAnim.SetFloat("drawPercent", drawPercent);
            }

        }

        private void SpawnBow(Vector3 position)
        {
            GameObject newbow = Instantiate(ArcaneBowPrefab, position, Quaternion.LookRotation(Vector3.forward, Vector3.right)).transform.GetChild(0).gameObject;
            activeBows.Add(newbow);
            activeBowsAnim.Add(newbow.transform.GetComponent<Animator>());

            currentSwipeBows.Add(newbow);
        }

        private void DestroyBow(GameObject bow)
        {
            if (activeBows.Contains(bow))
            {
                activeBows.Remove(bow);
                activeBowsAnim.Remove(bow.GetComponent<Animator>());
                Destroy(bow.transform.parent.gameObject);
            }
        }

        private void FireAll()
        {
            if (drawPercent > 0.33f)
            {
                float arrowSpeed = Mathf.Lerp(maxArrowSpeed * 0.5f, maxArrowSpeed, drawPercent);
                float arrowDamage = Mathf.Lerp(maxArrowDamage * 0.5f, maxArrowDamage, drawPercent);
                float arrowKnockback = Mathf.Lerp(maxArrowKnockback * 0.5f, maxArrowKnockback, drawPercent);
                foreach (GameObject bow in activeBows)
                {
                    Projectile newArrow = Instantiate(arrowPrefab, bow.transform.position, bow.transform.rotation);
                    newArrow.SetInitialValues(bow.transform.right * arrowSpeed, arrowDamage, Attack.DamageType.Arcane, arrowKnockback, 0, LayerMask.NameToLayer("Enemy"), 5f, true);
                }
            }

            drawPercent = 0f;
        }

        private void HandleBowSpawning()
        {
            if (Input.GetMouseButtonDown(1))
            {
                currentSwipeBows.Clear();
            }

            if (Input.GetMouseButton(1))
            {
                Collider2D[] collsNearMouse = Physics2D.OverlapCircleAll(wizard.mouse.transform.position, bowRadius);

                if(collsNearMouse.Length == 0) //if space is empty, spawn a bow
                {
                    SpawnBow(wizard.mouse.transform.position);
                }
            }
            if (Input.GetMouseButtonUp(1))
            {
                currentSwipeBows.Clear();
            }
        }

        private void HandleBowDestroying()
        {
            if (Input.GetMouseButtonDown(2))
            {
                foreach(GameObject bow in activeBows.ToArray())
                {
                    DestroyBow(bow);
                }
            }
        }

        private void FireModeConcentrated()
        {
            BowsPointTowardsMouse();
            if (Input.GetMouseButton(0))
            {
                drawPercent += (1 / fullDrawTime) * Time.deltaTime;
                drawPercent = Mathf.Clamp01(drawPercent);
            }
            else if (Input.GetMouseButtonUp(0)) //fire
            {
                FireAll();
            }
        }

        private Vector2[] drawPoints = new Vector2[2];
        private LineRenderer lr;
        private void FireModeVolley()
        {
            if (Input.GetMouseButtonDown(0))
            {
                drawPoints[0] = wizard.mouse.transform.position;
                lr = Instantiate(drawLinePrefab);
                lr.positionCount = 1;
                lr.SetPosition(0, drawPoints[0]);
            }
            else if (Input.GetMouseButton(0))
            {
                drawPoints[1] = wizard.mouse.transform.position;
                Vector2 drawVector = drawPoints[0] - drawPoints[1];
                float drawnDrawPercent = Mathf.Clamp01(drawVector.magnitude / mouseMaxDrawDistance);
                float maxDrawDelta = (1 / fullDrawTime) * Time.deltaTime;
                drawPercent = Mathf.Clamp01(drawPercent + Mathf.Clamp(drawnDrawPercent - drawPercent, -1f, maxDrawDelta));

                //update line renderer
                int numLrPoints = 1000;
                lr.positionCount = numLrPoints;
                for(int i = 1; i < numLrPoints-1; i++)
                {
                    lr.SetPosition(i, new Vector3(Mathf.Lerp(drawPoints[0].x, drawPoints[1].x, (float)i / numLrPoints), Mathf.Lerp(drawPoints[0].y, drawPoints[1].y, (float)i / numLrPoints), 0f));
                }
                lr.SetPosition(numLrPoints-1, drawPoints[1]);
                lr.colorGradient = SetGradientTime(lr.colorGradient, drawPercent);

                foreach (GameObject bow in activeBows)
                {
                    bow.transform.parent.rotation = Quaternion.RotateTowards(bow.transform.parent.rotation, Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.right, drawVector), Vector3.forward), bowRotationSpeed * Time.deltaTime);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                FireAll();
                Destroy(lr.gameObject);
                lr = null;
            }

            Gradient SetGradientTime(Gradient old, float time)
            {
                GradientColorKey[] colorKeys = old.colorKeys;

                colorKeys[0].time = Mathf.Clamp01(time - 0.00001f);
                colorKeys[1].time = Mathf.Clamp01(time + 0.00001f);

                Gradient newGrad = new Gradient();
                newGrad.SetKeys(colorKeys, old.alphaKeys);
                return newGrad;
            }
        }
    }
}
