using UnityEngine;

namespace SciFi.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText _damageTextPrefab = null;

        public void Spawn(float damage)
        {
            DamageText instance = Instantiate<DamageText>(_damageTextPrefab, transform);
            instance.SetValue(damage);
        }
    }
}
