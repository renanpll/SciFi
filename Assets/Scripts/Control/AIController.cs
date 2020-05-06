using UnityEngine;
using SciFi.Combat;
using SciFi.Movement;
using SciFi.Attributes;
using GameDevTV.Utils;

namespace SciFi.Control
{
    public class AIController : MonoBehaviour
    {
        [Range(0, 1)]
        [SerializeField] private float _patrolSpeedFraction = 0.3f;
        [Range(0, 1)]
        [SerializeField] private float _suspicionSpeedFraction = 0.5f;
        [SerializeField] private float _detectionRange = 8f;
        [SerializeField] private float _fovMaxAngle = 45f;
        [SerializeField] private float _suspicionTime = 10f;
        [SerializeField] private float _aggroCooldownTime = 5f;
        [SerializeField] private float _waypointTolerance = 1f;
        [SerializeField] private float _waypointDwellTime = 3f;

        [SerializeField] private PatrolPath _patrolPath = null;

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
        private Vector3 _lastPlayerPosition;

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

        private void Aggrevate()
        {
            _timeSinceAggrevated = 0;
            _timeSinceLastSawPlayer = 0;
            _lastPlayerPosition = _player.transform.position;
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
            _mover.StartMoveAction(_lastPlayerPosition, _suspicionSpeedFraction);
        }

        private void AttackBehavior()
        {
            _fighter.Attack(_player);

            if (!_hasShout)
            {
                AggrevateNearbyEnemies();
            }
        }

        private void AggrevateNearbyEnemies()
        {
            _hasShout = true;

            RaycastHit[] hits = Physics.SphereCastAll(transform.position, _detectionRange, Vector3.up, 0);

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
            return HasSight() || _timeSinceAggrevated < _aggroCooldownTime;
        }

        private bool HasSight()
        {
            if (!GetIsInRange()) return false;

            Vector3 playerDirection = (_player.transform.position - transform.position).normalized;
            playerDirection.y *= 0;

            float angle = Vector3.Angle(transform.forward, playerDirection);
            if (angle > _fovMaxAngle) return false;

            Ray ray = new Ray((transform.position + Vector3.up), playerDirection);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, _detectionRange))
            {
                if (!hit.transform.CompareTag("Player")) return false;
            }
            else return false;

            _timeSinceLastSawPlayer = 0;
            _lastPlayerPosition = _player.transform.position;

            return true;
        }

        private bool GetIsInRange()
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);

            return distance < _detectionRange;
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRange);

            Vector3 fovLine1 = Quaternion.AngleAxis(_fovMaxAngle, transform.up) * transform.forward * _detectionRange;
            Vector3 fovLine2 = Quaternion.AngleAxis(-_fovMaxAngle, transform.up) * transform.forward * _detectionRange;
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, fovLine1);
            Gizmos.DrawRay(transform.position, fovLine2);
        }
    }
}

