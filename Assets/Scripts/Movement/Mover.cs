﻿using UnityEngine;
using UnityEngine.AI;
using SciFi.Core;
using SciFi.Saving;
using SciFi.Attributes;
using System;

namespace SciFi.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private float _maxSpeed = 5.66f;
        [SerializeField] private float _sneakySpeed = 1.55f;
        [SerializeField] private float _maxNavPathLength = 30f;

        [SerializeField] private AudioSource _footStepAudioSource;

        public event Action onNoiseMaking;

        private NavMeshAgent _agent;
        private Animator _anim;
        private Health _health;

        private bool _isSneaky = false;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _anim = GetComponent<Animator>();
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            _agent.enabled = !_health.IsDead();

            UpdateAnimator();

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                if (gameObject.CompareTag("Player"))
                    _isSneaky = !_isSneaky;
            }
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFraction);
        }

        public bool CanMoveTo(Vector3 target)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, target, NavMesh.AllAreas, path);

            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLenght(path) > _maxNavPathLength) return false;

            return true;
        }

        public void MoveTo(Vector3 destination, float speedFraction)
        {
            float speed = _maxSpeed;
            if (_isSneaky) speed = _sneakySpeed;

            _agent.speed = speed * Mathf.Clamp01(speedFraction);
            _agent.destination = destination;
            _agent.isStopped = false;
        }

        public void Cancel()
        {
            _agent.isStopped = true;
        }

        private float GetPathLenght(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }
            return total;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = transform.InverseTransformDirection(_agent.velocity);
            _anim.SetFloat("forwardSpeed", velocity.z);
        }

        //Triggered by animation event
        private void Step()
        {
            if (gameObject.CompareTag("Player"))
                onNoiseMaking();

            _footStepAudioSource.Play();
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = position.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
            GetComponent<ActionScheduler>().StartAction(null);
        }
    }

}
