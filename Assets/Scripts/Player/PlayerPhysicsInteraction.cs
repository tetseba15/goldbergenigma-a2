using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPhysicsInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler _playerInput;


    [Header("Push Settings")]
    [SerializeField, Tooltip("Fuerza base con la que el jugador empuja objetos")]
    private float _pushForce = 2f;


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        if (body == null || body.isKinematic || hit.moveDirection.y < -0.3f) return;

        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z).normalized;
        

        if (_playerInput.IsSprinting)
        {
            body.AddForceAtPosition(pushDirection * _pushForce * 3, hit.point, ForceMode.Impulse);
        }
        else
        {
            body.AddForceAtPosition(pushDirection * _pushForce, hit.point, ForceMode.Impulse);
        }
    }
}