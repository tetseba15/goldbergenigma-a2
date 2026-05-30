using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyPhysicsInteraction : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField] private float _enemyPushForce = 15f;

    private NavMeshAgent _agent;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody body = collision.rigidbody;

        if (body == null || body.isKinematic) return;

        Vector3 pushDirection = _agent.velocity.normalized;
        pushDirection.y = 0f;

        body.AddForceAtPosition(pushDirection * _enemyPushForce, collision.GetContact(0).point, ForceMode.Force);
    }
}