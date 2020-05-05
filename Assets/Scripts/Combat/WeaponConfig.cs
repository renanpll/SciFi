using GameDevTV.Inventories;
using SciFi.Attributes;
using SciFi.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace SciFi.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "Weapons/Make new Weapon", order = 0)]
    public class WeaponConfig : EquipableItem, IModifierProvider
    {
        [SerializeField] private Weapon _equippedPrefab = null;
        [SerializeField] private AnimatorOverrideController _animatorOverride = null;
        [SerializeField] private float _weaponrange = 2f;
        [SerializeField] private float _weaponDamage = 5f;
        [SerializeField] private float _percentageBonus = 0;
        [SerializeField] private bool _isRightHanded = true;
        [SerializeField] private bool _hasMuzzle = false;
        [SerializeField] private Projectile _projectile = null;

        private Weapon _weaponInstance = null;

        private const string _weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            if (_equippedPrefab != null)
            {
                Transform handTransform = GetHandTransform(rightHand, leftHand);

                _weaponInstance = Instantiate(_equippedPrefab, handTransform);
                _weaponInstance.gameObject.name = _weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;

            if (_animatorOverride != null)
            {
                animator.runtimeAnimatorController = _animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return _weaponInstance;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float calculatedDamage)
        {
            Vector3 launchPosition;
            if (_weaponInstance.GetMuzler() != null)
            {
                launchPosition = _weaponInstance.GetMuzler().position;
            }
            else
            {
                launchPosition = GetHandTransform(rightHand, leftHand).position;
            }
            Projectile projectileInstance = Instantiate(_projectile, launchPosition, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, calculatedDamage);
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(_weaponName);

            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(_weaponName);
            }

            if (oldWeapon == null) return;

            oldWeapon.name = "Old Weapon";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetHandTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;

            if (_isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public float GetDamage()
        {
            return _weaponDamage;
        }

        public float GetPercentageBonus()
        {
            return _percentageBonus;
        }

        public float GetRange()
        {
            return _weaponrange;
        }

        public bool HasProjectile()
        {
            return _projectile != null;
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _weaponDamage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return _percentageBonus;
            }
        }
    }

}