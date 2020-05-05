using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SciFi.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text _damageText = null;

        public void SetValue(float amount)
        {
            _damageText.text = String.Format("{0:0}", amount);
        }
    }
}
