using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SciFi.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] Transform _muzler = null;
        [SerializeField] UnityEvent onHit;
        public void OnHit()
        {
            onHit.Invoke();
        }

        public Transform GetMuzler()
        {
            if (_muzler == null) return null;

            return _muzler;
        }
    }
}
