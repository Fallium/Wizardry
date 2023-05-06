using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            //Singleton that DOESNT persist across scenes
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(this);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public static IEnumerator TimedDestruct(GameObject objectToDestroy, float time)
        {
            yield return new WaitForSeconds(time);

            Destroy(objectToDestroy);
        }
    }
}
