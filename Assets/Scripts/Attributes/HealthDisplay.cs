using System;
using UnityEngine;
using UnityEngine.UI;

namespace SciFi.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health _health;
        void Awake()
        {
            _health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}/{1:0}", _health.GetHealthPoints(), _health.GetMaxHealth());
        }
    }
}

