using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Speed settings")]
    [SerializeField, Tooltip("Patrol speed")] private float _patrolSpeed = 2f;
    [SerializeField, Tooltip("Chase speed")] private float _chaseSpeed = 5.5f;

    private enum AIState { Patrol, Chase, Idle }

    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _lookAtTarget;

    [Space(20)]
    [SerializeField] private Transform[] _patrolWaypoints;

    
    [Header("Enemy Spawns Points")]
    [SerializeField] private Transform[] _enemySpawnPoints;

    [Header("Vision Settings")]
    [SerializeField, Range(1f, 50f)] private float _viewRadious = 15f;
    [SerializeField, Range(1f, 360f)] private float _viewAngle = 90f;

    [Space(10)]
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;

    private NavMeshAgent _agent;
    private Animator _animator;
    private AIState _currentState;
    private int _currentWaypointIndex;
    private bool _playerInSafeZone = false;

    [Header("Flashlight Settings")]
    [SerializeField] private PlayerFlashlight _playerFlashlight;
    [SerializeField] private float _flashlightRepelDistance = 10f;
    [SerializeField] private float _flashlightRepelSpeed = 5f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _currentState = AIState.Patrol;
    }

    void Start()
    {
        if (_patrolWaypoints.Length > 0)
        {
            _agent.SetDestination(_patrolWaypoints[0].position);
        }
    }

    void Update()
    {
        CheckSensors();
        CheckFlashlight();

        switch (_currentState)
        {
            case AIState.Patrol:
                HandlePatrol();
                break;

            case AIState.Chase:
                HandleChase();
                break;
        }
        _animator.SetFloat("Speed", _agent.velocity.magnitude / _agent.speed);
        UpdateLookAt();
    }

    
    public void HolyWaterImpact()
    {
        
        if (_enemySpawnPoints != null && _enemySpawnPoints.Length > 0)
        {
            
            int indiceAleatorio = UnityEngine.Random.Range(0, _enemySpawnPoints.Length);
            _agent.Warp(_enemySpawnPoints[indiceAleatorio].position);
        }
        
        ChangeState(AIState.Patrol);
    }

    private void CheckSensors()
    {
        if (_playerInSafeZone) return;

        Vector3 directionToPlayer = (_playerTransform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

        if (distanceToPlayer <= _viewRadious)
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) < _viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, _obstacleMask))
                {
                    ChangeState(AIState.Chase);
                    return;
                }
            }
        }

        if (_currentState == AIState.Chase && distanceToPlayer > _viewRadious * 1.5f)
        {
            ChangeState(AIState.Patrol);
        }
    }

    private void CheckFlashlight()
    {
        if (_playerFlashlight == null || !_playerFlashlight.IsOn()) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        if (distanceToPlayer > _flashlightRepelDistance) return;

        // Verificar si la linterna apunta hacia la nena
        Vector3 directionToEnemy = (transform.position - _playerTransform.position).normalized;
        float angle = Vector3.Angle(_playerTransform.forward, directionToEnemy);

        if (angle < 30f)
        {
            // La linterna apunta a la nena, alejarse
            Vector3 fleeDirection = (transform.position - _playerTransform.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * _flashlightRepelDistance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(fleeTarget, out hit, _flashlightRepelDistance, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }

            ChangeState(AIState.Patrol);
        }
    }

    private void ChangeState(AIState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        if (_currentState == AIState.Patrol)
        {
            _agent.SetDestination(_patrolWaypoints[_currentWaypointIndex].position);
        }
    }

    private void HandleChase()
    {
        _agent.speed = _chaseSpeed;
        _agent.SetDestination(_playerTransform.position);
    }

    private void HandlePatrol()
    {
        if (_patrolWaypoints.Length == 0) return;

        _agent.speed = _patrolSpeed;

        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _patrolWaypoints.Length;
            _agent.SetDestination(_patrolWaypoints[_currentWaypointIndex].position);
        }
    }

    private void UpdateLookAt()
    {
        if (_lookAtTarget == null) return;

        if (_currentState == AIState.Chase)
        {
            _lookAtTarget.position = Vector3.Lerp(
                _lookAtTarget.position,
                _playerTransform.position + Vector3.up * 1.5f,
                Time.deltaTime * 5f
            );
        }
    }

    public void ForcePatrol()
    {
        ChangeState(AIState.Patrol);
    }

    public void SetPlayerInSafeZone(bool value)
    {
        _playerInSafeZone = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }

    #region Debug Gizmos
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _viewRadious);

        Vector3 viewAngleLeft = DirFromAngle(transform.eulerAngles.y, -_viewAngle / 2f);
        Vector3 viewAngleRight = DirFromAngle(transform.eulerAngles.y, _viewAngle / 2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleLeft * _viewAngle);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleRight * _viewAngle);

        if (_playerTransform != null && _currentState == AIState.Chase)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _playerTransform.position);
        }
    }

    private Vector3 DirFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion
}