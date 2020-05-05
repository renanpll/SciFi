using System;
using UnityEngine;
using UnityEngine.UI;

namespace SciFi.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        private Experience _experience;
        void Awake()
        {
            _experience = GameObject.FindGameObjectWithTag("Player").GetComponent<Experience>();
        }

        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", _experience.GetExperiencePoints());
        }
    }
}

