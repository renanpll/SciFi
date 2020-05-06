using UnityEngine;
using SciFi.Movement;
using SciFi.Core;
using SciFi.Saving;
using SciFi.Attributes;
using SciFi.Stats;
using GameDevTV.Utils;
using GameDevTV.Inventories;

namespace SciFi.Combat
{

    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float _attackSpeed = 1f;
        [SerializeField] private Transform _rightHandTransform = null;
        [SerializeField] private Transform _leftHandTransform = null;
        [SerializeField] private WeaponConfig _defaultWeapon = null;

        private Health _target;
        private Equipment _equipment;
        private float _timeSinceLastAttack = Mathf.Infinity;
        private WeaponConfig _currentWeaponConfig;
        private LazyValue<Weapon> _currentWeapon;

        private void Awake()
        {
            _currentWeaponConfig = _defaultWeapon;
            _currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);

            _equipment = GetComponent<Equipment>();
            if (_equipment)
            {
                _equipment.equipmentUpdated += UpdateWeapon;
            }
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(_defaultWeapon);
        }

        private void Start()
        {
            _currentWeapon.ForceInit();
        }

        private void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            if (_target != null && !_target.IsDead())
            {
                if (!GetIsInRange(_target.transform))
                {
                    GetComponent<Mover>().MoveTo(_target.transform.position, 1f);
                }
                else
                {
                    GetComponent<Mover>().Cancel();
                    AttackBehavior();
                }
            }
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            _currentWeaponConfig = weapon;
            _currentWeapon.value = AttachWeapon(weapon);
        }

        private void UpdateWeapon()
        {
            var weapon = _equipment.GetItemInSlot(EquipLocation.Weapon) as WeaponConfig;

            if (weapon == null)
            {
                EquipWeapon(_defaultWeapon);
            }
            else
            {
                EquipWeapon(weapon);
            }
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator anim = GetComponent<Animator>();
            return weapon.Spawn(_rightHandTransform, _leftHandTransform, anim);
        }

        public bool CanAttack(GameObject target)
        {
            if (!GetComponent<Mover>().CanMoveTo(target.transform.position) && !GetIsInRange(target.transform)) return false;

            return target != null && !target.GetComponent<Health>().IsDead();
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            _target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            _target = null;
            GetComponent<Mover>().Cancel();
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        public Health GetTarget()
        {
            return _target;
        }


        private void AttackBehavior()
        {
            if (_timeSinceLastAttack > _attackSpeed)
            {
                transform.LookAt(_target.transform.position);
                //This will trigger the Hit() event
                TriggerAttack();
                _timeSinceLastAttack = 0;
            }
        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        //Animation Event
        private void Hit()
        {
            if (_target == null) return;

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (_currentWeapon.value != null)
            {
                _currentWeapon.value.OnHit();
            }
            
            if (_currentWeaponConfig.HasProjectile())
            {
                _currentWeaponConfig.LaunchProjectile(_rightHandTransform, _leftHandTransform, _target, gameObject, damage);
            }
            else
            {
                _target.TakeDamage(gameObject, damage);
            }
        }
        //Animation Event
        private void Shoot()
        {
            Hit();
        }

        private bool GetIsInRange(Transform targetTransform)
        {
            return Vector3.Distance(transform.position, targetTransform.position) <= _currentWeaponConfig.GetRange();
        }

        //ISaveable implementation
        public object CaptureState()
        {
            return _currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string weaponName = (string)state;
            WeaponConfig weapon = UnityEngine.Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }
    }

}
