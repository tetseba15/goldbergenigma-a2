using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPhysicsInteraction : MonoBehaviour
{
    [Header("Push Settings")]
    [SerializeField, Tooltip("Fuerza base con la que el jugador empuja objetos")]
    private float _pushForce = 2f;

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic) return;

        if (hit.moveDirection.y < -0.3f) return;

        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        body.AddForceAtPosition(pushDirection * _pushForce, hit.point, ForceMode.Impulse);
    }
}