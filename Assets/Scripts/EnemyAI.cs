using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] private EnemyData _data;

    public enum AIState { Patrol, Chase, Idle, Investigate }

    private Vector3 _investigateTarget;

    [Header("References")]
    [SerializeField] private Transform _lookAtTarget;

    [Space(20)]
    [SerializeField] private Transform[] _patrolWaypoints;

    [Space(10)]
    [SerializeField] private LayerMask _obstacleMask;

    private NavMeshAgent _agent;
    private Animator _animator;
    private AIState _currentState;
    private int _currentWaypointIndex;
    private bool _playerInSafeZone = false;
    private int _noSpawnAreaMask;
    private SpawnZone[] _spawnZones;
    private float _appearTimer = 0f;
    private float _currentAppearDuration;

    [Header("Spawn Settings")]
    [SerializeField] private EnemySpawnEnabler _spawnEnabler;

    private void OnEnable()
    {
        NoiseManager.OnNoiseEmitted += HearNoise;
        _appearTimer = 0f;
        _currentAppearDuration = UnityEngine.Random.Range(_data.minAppearDuration, _data.maxAppearDuration);
    }

    private void OnDisable()
    {
        NoiseManager.OnNoiseEmitted -= HearNoise;
    }

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _currentState = AIState.Chase;

        int noSpawnArea = NavMesh.GetAreaFromName("NoSpawn");
        _noSpawnAreaMask = NavMesh.AllAreas & ~(1 << noSpawnArea);
        _spawnZones = FindObjectsByType<SpawnZone>(FindObjectsSortMode.None);
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

        if (_spawnEnabler != null)
        {
            _appearTimer += Time.deltaTime;
            if (_appearTimer >= _currentAppearDuration)
            {
                _appearTimer = 0f;
                _spawnEnabler.DespawnEnemy();
                return;
            }
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

    private void TeleportNearPlayer()
    {
        if (PlayerTarget.Instance == null) return;

        Transform player = PlayerTarget.Instance.PlayerTransform;

        for (int i = 0; i < 10; i++)
        {
            float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = UnityEngine.Random.Range(_data.minTeleportDistance, _data.maxTeleportDistance);

            Vector3 offset = new Vector3(Mathf.Sin(angle) * distance, 0f, Mathf.Cos(angle) * distance);
            Vector3 candidatePos = player.position + offset;

            if (NavMesh.SamplePosition(candidatePos, out NavMeshHit hit, 2f, _noSpawnAreaMask))
            {
                bool inSpawnZone = false;
                foreach (var zone in _spawnZones)
                {
                    if (zone.Contains(hit.position))
                    {
                        inSpawnZone = true;
                        break;
                    }
                }

                if (inSpawnZone)
                {
                    _agent.Warp(hit.position);
                    ChangeState(AIState.Chase);
                    return;
                }
            }
        }
    }

    public void TeleportNow()
    {
        TeleportNearPlayer();
    }

    private void CheckSensors()
    {
        if (_playerInSafeZone) return;

        Transform target = PlayerTarget.Instance.PlayerTransform;

        Vector3 directionToPlayer = target.position - transform.position;
        float sqrDistanceToPlayer = directionToPlayer.sqrMagnitude;

        if (sqrDistanceToPlayer <= (_data.viewRadius * _data.viewRadius))
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) < _data.viewAngle / 2f)
            {
                float distanceToPlayer = Mathf.Sqrt(sqrDistanceToPlayer);
                if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, _obstacleMask))
                {
                    ChangeState(AIState.Chase);
                    return;
                }
            }
        }
    }

    private void HandleInvestigate()
    {
        _agent.speed = _data.patrolSpeed;
        _agent.SetDestination(_investigateTarget);

        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            ChangeState(AIState.Chase);
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
        if (PlayerTarget.Instance.Flashlight == null || !PlayerTarget.Instance.Flashlight.IsOn()) return;

        Transform target = PlayerTarget.Instance.PlayerTransform;
        float sqrDistanceToPlayer = (transform.position - target.position).sqrMagnitude;

        if (sqrDistanceToPlayer > (_data.flashlightRepelDistance * _data.flashlightRepelDistance)) return;

        Vector3 directionToEnemy = (transform.position - target.position).normalized;
        float angle = Vector3.Angle(target.forward, directionToEnemy);

        if (angle < 30f)
        {
            Vector3 fleeDirection = (transform.position - target.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * _data.flashlightRepelDistance;

            if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, _data.flashlightRepelDistance, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }

            ChangeState(AIState.Investigate);
        }
    }

    private void ChangeState(AIState newState)
    {
        if (_currentState == newState) return;
        _currentState = newState;
    }

    private void HandleChase()
    {
        _agent.speed = _data.chaseSpeed;
        _agent.SetDestination(PlayerTarget.Instance.PlayerTransform.position);
    }

    private void HandlePatrol()
    {
        if (_patrolWaypoints.Length == 0) return;

        _agent.speed = _data.patrolSpeed;

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
        if (_data == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _data.viewRadius);

        Vector3 viewAngleLeft = DirFromAngle(transform.eulerAngles.y, -_data.viewAngle / 2f);
        Vector3 viewAngleRight = DirFromAngle(transform.eulerAngles.y, _data.viewAngle / 2f);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + viewAngleLeft * _data.viewAngle);
        Gizmos.DrawLine(transform.position, transform.position + viewAngleRight * _data.viewAngle);

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