using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SciFi.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] RectTransform _foreground = null;

        void Update()
        {
            if (GetComponentInParent<Health>().IsDead())
            {
                GetComponentInChildren<Canvas>().enabled = false;
                return;
            }

            GetComponentInChildren<Canvas>().enabled = true;
            float healthPercentage = GetComponentInParent<Health>().GetHealthPercentage();
            Vector3 healthScale = new Vector3(healthPercentage, 1, 1);
            _foreground.localScale = healthScale;
        }
    }
}

