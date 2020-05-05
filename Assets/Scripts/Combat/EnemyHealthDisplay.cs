using SciFi.Attributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SciFi.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter _playerFighter;

        void Awake()
        {
            _playerFighter = GameObject.FindGameObjectWithTag("Player").GetComponent<Fighter>();
        }

        void Update()
        {
            Health targetHealth = _playerFighter.GetTarget();
            if (targetHealth != null)
            {
                GetComponent<Text>().text = String.Format("{0:0}/{1:0}", targetHealth.GetHealthPoints(), targetHealth.GetMaxHealth());
            }
            else
            {
                GetComponent<Text>().text = "N/A";
            }
        }
    }
}