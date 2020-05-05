using SciFi.Saving;
using System;
using UnityEngine;

namespace SciFi.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float _experiencePoints = 0;

        public event Action onExperienceGained;
        public void GainExperience(float experience)
        {
            _experiencePoints += experience;
            onExperienceGained();
        }

        public float GetExperiencePoints()
        {
            return _experiencePoints;
        }

        public object CaptureState()
        {
            return _experiencePoints;
        }

        public void RestoreState(object state)
        {
            _experiencePoints = (float)state;
        }
    }

}