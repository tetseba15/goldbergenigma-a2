using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    //                       roar duration
    public static event Action<float> OnEnemyRoaring;

    public enum AIState { Patrol, Chase, Idle, Investigate, Spotted, Fleeing }


    [Header("Enemy Data")]
    [SerializeField] private EnemyData _data;
    
    [Header("References")]
    [SerializeField] private Transform _lookAtTarget;
    [SerializeField] private EnemyAudioManager _audioManager; 
    [SerializeField] private EnemySpawnEnabler _spawnEnabler;

    [Header("Timers & Delays")]
    [SerializeField] private float _roarDuration = 2f;
    [SerializeField] private float _fleeDuration = 3f;

    [Space(20)]
    [SerializeField] private Transform[] _patrolWaypoints;

    [Space(10)]
    [SerializeField] private LayerMask _obstacleMask;

    private NavMeshAgent _agent;
    private Animator _animator;
    private AIState _currentState;
    private Vector3 _investigateTarget;
    private int _currentWaypointIndex;
    private bool _playerInSafeZone = false;
    private bool _isStunned = false;

    private float _fleeTimer = 0f;
    private float _timeSinceLastSeen = 0f;
    private Vector3 _lastKnownPosition;

    private bool _hasBeenBlindedByFlashlight = false;

    // Spawn / Teleport variables
    private int _noSpawnAreaMask;
    private SpawnZone[] _spawnZones;
    private float _appearTimer = 0f;
    private float _currentAppearDuration;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();

        if (_audioManager == null)
            Debug.LogWarning($"[EnemyAI] Missing EnemyAudioManager on {gameObject.name}");

        _currentState = AIState.Patrol;

        int noSpawnArea = NavMesh.GetAreaFromName("NoSpawn");
        _noSpawnAreaMask = NavMesh.AllAreas & ~(1 << noSpawnArea);
        _spawnZones = FindObjectsByType<SpawnZone>(FindObjectsSortMode.None);
    }

    private void OnEnable()
    {
        NoiseManager.OnNoiseEmitted += HearNoise;
        _appearTimer = 0f;
        _hasBeenBlindedByFlashlight = false; 

        if (_data != null)
            _currentAppearDuration = UnityEngine.Random.Range(_data.minAppearDuration, _data.maxAppearDuration);
    }

    private void OnDisable()
    {
        NoiseManager.OnNoiseEmitted -= HearNoise;
    }

    private void Start()
    {
        if (_patrolWaypoints.Length > 0)
        {
            _agent.SetDestination(_patrolWaypoints[0].position);
        }
    }

    private void Update()
    {
        if (_isStunned)
        {
            _animator.SetFloat("Speed", 0f);
            return;
        }

        if (PlayerTarget.Instance == null)
        {
            if (_currentState == AIState.Chase) ChangeState(AIState.Patrol);
            return;
        }

        if (HandleSpawnTimer()) return;

        HandleSpawnTimer();

        // Control de Tiempos
        if (_currentState == AIState.Fleeing)
        {
            _fleeTimer -= Time.deltaTime;
            if (_fleeTimer <= 0) ChangeState(AIState.Patrol);
        }

        if (_currentState != AIState.Spotted && _currentState != AIState.Fleeing)
        {
            CheckSensors();
            CheckFlashlight();
        }

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
            case AIState.Fleeing:
                // El NavMeshAgent ya tiene su destino de huida seteado en CheckFlashlight()
                break;
            case AIState.Spotted:
                break;
        }

        _animator.SetFloat("Speed", _agent.velocity.magnitude / _agent.speed);
        UpdateLookAt();
    }

    

    #region Combat & Reactions

    public void HolyWaterImpact()
    {
        if (_audioManager != null) _audioManager.PlayHurt();
        TeleportNearPlayer();
    }

    public void CrossImpact(float duration)
    {
        if (_isStunned) return;

        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        _isStunned = true;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        if (_audioManager != null) _audioManager.PlayHurt();

        yield return new WaitForSeconds(duration);

        _isStunned = false;
        _agent.isStopped = false;

        ChangeState(AIState.Patrol);
    }

    #endregion

    #region Handlers (Patrol, Chase, Investigate)

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

    private void HandleChase()
    {
        _agent.speed = _data.chaseSpeed;
        _agent.SetDestination(PlayerTarget.Instance.PlayerTransform.position);
    }

    private void HandleInvestigate()
    {
        _agent.speed = _data.patrolSpeed;
        _agent.SetDestination(_investigateTarget);

        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            ChangeState(AIState.Patrol);
        }
    }

    #endregion

    #region Senses (Vision, Hearing, Flashlight)

    private void CheckSensors()
    {
        if (_playerInSafeZone) return;

        Transform target = PlayerTarget.Instance.PlayerTransform;
        Vector3 directionToPlayer = target.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // VISUAL DETECTION
        if (distanceToPlayer <= _data.viewRadius)
        {
            if (Vector3.Angle(transform.forward, directionToPlayer) < _data.viewAngle / 2f)
            {
                if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, _obstacleMask))
                {
                    if (_currentState != AIState.Chase)
                    {
                        StartCoroutine(SpotPlayerRoutine());
                    }
                    else
                    {
                        _lastKnownPosition = target.position;
                        _timeSinceLastSeen = 0f;
                    }
                    return;
                }
            }
        }

        // LAST KNOWN LOCATION LOGIC
        if (_currentState == AIState.Chase)
        {
            _timeSinceLastSeen += Time.deltaTime;

            if (_timeSinceLastSeen > 1.5f)
            {
                _investigateTarget = _lastKnownPosition;
                ChangeState(AIState.Investigate);
            }
        }
    }

    private void HearNoise(Vector3 noisePosition, float noiseVolume)
    {
        // ignore noises if state is chase,spotter,fleeing or stunned
        if (_playerInSafeZone || _currentState == AIState.Chase || _currentState == AIState.Spotted || _currentState == AIState.Fleeing || _isStunned) return;

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
            if (!_hasBeenBlindedByFlashlight)
            {
                _hasBeenBlindedByFlashlight = true;

                StartCoroutine(EnragePlayerRoutine(4f));
                return;
            }

            Vector3 fleeDirection = (transform.position - target.position).normalized;
            Vector3 fleeTarget = transform.position + fleeDirection * _data.flashlightRepelDistance;

            if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit hit, _data.flashlightRepelDistance, NavMesh.AllAreas))
            {
                _agent.SetDestination(hit.position);
            }

            _fleeTimer = _fleeDuration;
            ChangeState(AIState.Fleeing);
        }
    }

    #endregion

    #region State Management & Coroutines

    private void ChangeState(AIState newState)
    {
        if (_currentState == newState) return;

        _currentState = newState;

        if (_audioManager != null)
        {
            switch (_currentState)
            {
                case AIState.Patrol:
                case AIState.Idle:
                case AIState.Investigate:
                    _audioManager.PlayIdleBreathing();
                    break;
                    // El rugido ahora se maneja en la Corrutina SpotPlayerRoutine
            }
        }
    }

    private IEnumerator EnragePlayerRoutine(float duration)
    {
        _currentState = AIState.Spotted;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        if (_audioManager != null) _audioManager.PlayEnraged();

        // Trigger the panic events on player (Camera shake, slow down, and long flicker)
        OnEnemyRoaring?.Invoke(duration);

        yield return new WaitForSeconds(duration);

        _agent.isStopped = false;
        ChangeState(AIState.Chase); 
    }

    private IEnumerator SpotPlayerRoutine()
    {
        _currentState = AIState.Spotted;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        // animation/sfx feedback
        if (_audioManager != null) _audioManager.PlayAttack();

        // Flashlight flick
        OnEnemyRoaring?.Invoke(_roarDuration);

        // invulnerability
        yield return new WaitForSeconds(_roarDuration);

        _agent.isStopped = false;
        ChangeState(AIState.Chase);
    }

    #endregion

    #region Spawning & Utilities

    private bool HandleSpawnTimer()
    {
        if (_spawnEnabler != null)
        {
            _appearTimer += Time.deltaTime;
            if (_appearTimer >= _currentAppearDuration)
            {
                _appearTimer = 0f;
                _spawnEnabler.DespawnEnemy();
                return true; 
            }
        }
        return false; 
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

    public void TeleportNow() => TeleportNearPlayer();

    public void ForcePatrol() => ChangeState(AIState.Patrol);

    public void SetPlayerInSafeZone(bool value) => _playerInSafeZone = value;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isStunned)
        {
            //GameManager.Instance.GameOver();
        }
    }

    #endregion

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