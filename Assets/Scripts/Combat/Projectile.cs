using SciFi.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace SciFi.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _speed = 20f;
        [SerializeField] private bool _isHoming = false;
        [SerializeField] UnityEvent onHit;

        private float _damage;
        Health _target = null;
        GameObject _instigator = null;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }
        void Update()
        {
            if (_isHoming && !_target.IsDead()) transform.LookAt(GetAimLocation());

            transform.Translate(Vector3.forward * Time.deltaTime * _speed);
        }

        private void OnTriggerEnter(Collider other)
        {
            Health targetHealth = other.GetComponent<Health>();

            if (targetHealth == null) return;

            if (targetHealth.IsDead()) return;

            _speed = 0;

            onHit.Invoke();
            targetHealth.TakeDamage(_instigator, _damage);
            Destroy(gameObject, 1f);
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            _target = target;
            _damage = damage;
            _instigator = instigator;
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCollider = _target.GetComponent<CapsuleCollider>();

            if (targetCollider == null) return _target.transform.position;

            return _target.transform.position + (Vector3.up * targetCollider.height / 2);
        }
    }

}
