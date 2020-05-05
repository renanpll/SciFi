using GameDevTV.Utils;
using SciFi.Core;
using SciFi.Saving;
using SciFi.Stats;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace SciFi.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] TakeDamageEvent takeDamage;
        [SerializeField] UnityEvent onDie;

        private LazyValue<float> _healthPoints;
        private bool _isDead = false;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        { }

        private void Awake()
        {
            _healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start()
        {
            _healthPoints.ForceInit();
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenHealth;
        }

        public float GetHealthPercentage()
        {
            return (_healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health));
        }

        public bool IsDead()
        {
            return _isDead;
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            takeDamage.Invoke(damage);

            _healthPoints.value = Mathf.Max(_healthPoints.value - damage, 0);

            if (_healthPoints.value == 0)
            {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
        }

        public float GetHealthPoints()
        {
            return _healthPoints.value;
        }

        public float GetMaxHealth()
        {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void AwardExperience(GameObject instigator)
        {
            Experience experience = instigator.GetComponent<Experience>();
            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void Die()
        {
            if (_isDead) return;

            _isDead = true;
            GetComponent<Animator>().SetTrigger("die");

            GetComponent<ActionScheduler>().StartAction(null);
            
        }

        private void RegenHealth()
        {
            _healthPoints.value = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public object CaptureState()
        {
            return _healthPoints.value;
        }

        public void RestoreState(object state)
        {
            _healthPoints.value = (float)state;
            if (_healthPoints.value <= 0) Die();
        }


    }
}