using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerPhysicsInteraction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler _playerInput;


    [Header("Push Settings")]
    [SerializeField] private float _pushForce = 2f;
    [SerializeField] private float _sprintPushForce = 8f;


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;
        if (body == null || body.isKinematic || hit.moveDirection.y < -0.3f) return;

        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z).normalized;

        float currentForce = _playerInput.IsSprinting ? _sprintPushForce : _pushForce;

        body.AddForceAtPosition(pushDirection * currentForce, hit.point, ForceMode.Impulse);
    }
}