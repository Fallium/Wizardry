using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wizardry
{
    public class MouseObject : MonoBehaviour
    {
        private void Update()
        {
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }
}
