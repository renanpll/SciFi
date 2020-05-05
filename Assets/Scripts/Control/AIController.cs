using UnityEngine;
using SciFi.Combat;
using SciFi.Core;
using SciFi.Movement;
using SciFi.Attributes;
using GameDevTV.Utils;
using System;

namespace SciFi.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float _patrolSpeedFraction = 0.3f;
        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 4f;
        [SerializeField] private float _shoutDistance = 5f;
        [SerializeField] private float _aggroCooldownTime = 3f;
        [SerializeField] private float _waypointTolerance = 1f;
        [SerializeField] private float _waypointDwellTime = 3f;
        [SerializeField] private PatrolPath _patrolPath = null;
        [Range(0, 1)]

        private GameObject _player;
        private Fighter _fighter;
        private Mover _mover;
        private Health _health;

        private LazyValue<Vector3> _guardPosition;
        private int currentWaypointIndex = 0;

        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private float _timeSinceAggrevated = Mathf.Infinity;

        private bool _hasShout = false;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _fighter = GetComponent<Fighter>();
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();

            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }


        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (_health.IsDead()) return;

            if (IsAggrevated() && _fighter.CanAttack(_player))
            {
                AttackBehavior();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                _hasShout = false;
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimers();
        }

        public void Aggrevate()
        {
            _timeSinceAggrevated = 0;
        }

        private void UpdateTimers()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
            _timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = _guardPosition.value;

            if (_patrolPath != null)
            {
                if (AtWaypoint())
                {
                    _timeSinceArrivedAtWaypoint = 0;
                    CycleWaypoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedAtWaypoint > _waypointDwellTime)
            {
                _mover.StartMoveAction(nextPosition, _patrolSpeedFraction);
            }
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < _waypointTolerance;
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = _patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().StartAction(null);
        }

        private void AttackBehavior()
        {

            _timeSinceLastSawPlayer = 0;
            _fighter.Attack(_player);

            if (!_hasShout)
            {
                AggrevateNearbyEnemies();
            }
        }

        private void AggrevateNearbyEnemies()
        {
            _hasShout = true;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, _shoutDistance, Vector3.up, 0);

            foreach (RaycastHit hit in hits)
            {
                AIController aIController = hit.transform.GetComponent<AIController>();

                if (aIController == null) continue;
                if (aIController == this) continue;
                if (aIController.IsAggrevated()) continue;

                aIController.Aggrevate();
            }
        }

        private bool IsAggrevated()
        {

            float distance = Vector3.Distance(transform.position, _player.transform.position);

            return distance < _chaseDistance || _timeSinceAggrevated < _aggroCooldownTime;
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}

