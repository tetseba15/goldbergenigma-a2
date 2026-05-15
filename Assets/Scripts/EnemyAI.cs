using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Speed settings")]
    [SerializeField, Tooltip("Patrol speed")]
    private float _patrolSpeed = 2f;

    [SerializeField, Tooltip("Chase speed")]
    private float _chaseSpeed = 5.5f;

    public enum AIState { Patrol, Chase, Idle, Investigate }

    private Vector3 _investigateTarget;

    [Header("References")]
    [SerializeField] private Transform _lookAtTarget;

    [Space(20)]
    [SerializeField] private Transform[] _patrolWaypoints;

    [Header("Vision Settings")]
    [SerializeField, Range(1f, 50f)] private float _viewRadious = 15f;
    [SerializeField, Range(1f, 360f)] private float _viewAngle = 90f;

    [Space(10)]
    [SerializeField] private LayerMask _obstacleMask;

    private NavMeshAgent _agent;
    private Animator _animator;
    private AIState _currentState;
    private int _currentWaypointIndex;
    private bool _playerInSafeZone = false;

    [Header("Flashlight Settings")]
    [SerializeField] private float _flashlightRepelDistance = 10f;

    //Esta ref no hace falta, está guardada en el PlayerTarget pero la dejo por las dudas y comento la que está sin usar momentaneamente
    //[SerializeField] private PlayerFlashlight _playerFlashlight;
    //[SerializeField] private float _flashlightRepelSpeed = 5f;

    private void OnEnable()
    {
        NoiseManager.OnNoiseEmitted += HearNoise;
    }

    private void OnDisable()
    {
        NoiseManager.OnNoiseEmitted -= HearNoise;
    }


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
        if (PlayerTarget.Instance == null)
        {
            if (_currentState == AIState.Chase) ChangeState(AIState.Patrol);
            return;
        }

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
            case AIState.Investigate: 
                HandleInvestigate(); 
                break;
        }

        _animator.SetFloat("Speed", _agent.velocity.magnitude / _agent.speed);
        UpdateLookAt();
    }

    private void CheckSensors()
    {
        if (_playerInSafeZone) return;

        Transform target = PlayerTarget.Instance.PlayerTransform;

        Vector3 directionToPlayer = target.position - transform.position;
        float sqrDistanceToPlayer = directionToPlayer.sqrMagnitude;

        if (sqrDistanceToPlayer <= (_viewRadious * _viewRadious))
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) < _viewAngle / 2f)
            {
                float distanceToPlayer = Mathf.Sqrt(sqrDistanceToPlayer);
                if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, _obstacleMask))
                {
                    ChangeState(AIState.Chase);
                    return;
                }
            }
        }

        if (_currentState == AIState.Chase && sqrDistanceToPlayer > (_viewRadious * 1.5f) * (_viewRadious * 1.5f))
        {
            ChangeState(AIState.Patrol);
        }
    }

    private void HandleInvestigate()
    {
        _agent.speed = _patrolSpeed; 
        _agent.SetDestination(_investigateTarget);

        
        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            ChangeState(AIState.Patrol);
        }
    }

    private void HearNoise(Vector3 noisePosition, float noiseVolume)
    {
        if (_playerInSafeZone || _currentState == AIState.Chase) return;

        float sqrDistanceToNoise = (noisePosition - transform.position).sqrMagnitude;

        if (sqrDistanceToNoise <= (noiseVolume * noiseVolume))
        {
            _investigateTarget = noisePosition;
            ChangeState(AIState.Investigate);
        }
    }

    private void CheckFlashlight()
    {
        //if (_playerFlashlight == null || !_playerFlashlight.IsOn()) return;

        //float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);
        //if (distanceToPlayer > _flashlightRepelDistance) return;

        //// Verificar si la linterna apunta hacia la nena
        //Vector3 directionToEnemy = (transform.position - _playerTransform.position).normalized;
        //float angle = Vector3.Angle(_playerTransform.forward, directionToEnemy);

        //if (angle < 30f)
        //{
        //    // La linterna apunta a la nena, alejarse
        //    Vector3 fleeDirection = (transform.position - _playerTransform.position).normalized;
        //    Vector3 fleeTarget = transform.position + fleeDirection * _flashlightRepelDistance;

        //    NavMeshHit hit;
        //    if (NavMesh.SamplePosition(fleeTarget, out hit, _flashlightRepelDistance, NavMesh.AllAreas))
        //    {
        //        _agent.SetDestination(hit.position);
        //    }

        //    ChangeState(AIState.Patrol);
        //}

        if (PlayerTarget.Instance.Flashlight == null || !PlayerTarget.Instance.Flashlight.IsOn()) return;

        Transform target = PlayerTarget.Instance.PlayerTransform;
        float sqrDistanceToPlayer = (transform.position - target.position).sqrMagnitude;

        if (sqrDistanceToPlayer > (_flashlightRepelDistance * _flashlightRepelDistance)) return;

        Vector3 directionToEnemy = (transform.position - target.position).normalized;
        float angle = Vector3.Angle(target.forward, directionToEnemy);

        if (angle < 30f)
        {
            Vector3 fleeDirection = (transform.position - target.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * _flashlightRepelDistance;

            if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, _flashlightRepelDistance, NavMesh.AllAreas))
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
        _agent.SetDestination(PlayerTarget.Instance.PlayerTransform.position);
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
                PlayerTarget.Instance.PlayerTransform.position + Vector3.up * 1.5f,
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

        if (PlayerTarget.Instance != null && PlayerTarget.Instance.PlayerTransform != null && _currentState == AIState.Chase)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, PlayerTarget.Instance.PlayerTransform.position);
        }
    }

    private Vector3 DirFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion
}