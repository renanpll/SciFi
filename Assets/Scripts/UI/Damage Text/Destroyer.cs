using UnityEngine;

namespace SciFi.UI.DamageText
{
    public class Destroyer : MonoBehaviour
    {
        [SerializeField] GameObject _targetToDestroy;
        public void DestroyTarget()
        {
            Destroy(_targetToDestroy);
        }
    }

}
