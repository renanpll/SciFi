using GameDevTV.Inventories;
using SciFi.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SciFi.Inventories
{
    [CreateAssetMenu(menuName = ("SciFi/Inventory/Equipable Item"))]
    public class StatsEquipableItem : EquipableItem, IModifierProvider
    {
        [SerializeField] Modifier[] _additiveModifiers;
        [SerializeField] Modifier[] _percentageModifiers;

        [System.Serializable]
        struct Modifier
        {
            public Stat stat;
            public float value;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            foreach (var modifier in _additiveModifiers)
            {
                if (modifier.stat != stat) continue;

                yield return modifier.value;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            foreach (var modifier in _percentageModifiers)
            {
                if (modifier.stat != stat) continue;

                yield return modifier.value;
            }
        }
    }
}

