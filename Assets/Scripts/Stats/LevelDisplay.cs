using System;
using UnityEngine;
using UnityEngine.UI;

namespace SciFi.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats _baseStats;
        void Awake()
        {
            _baseStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        }

        void Update()
        {
            GetComponent<Text>().text = String.Format("{0:0}", _baseStats.GetLevel());
        }
    }

}
