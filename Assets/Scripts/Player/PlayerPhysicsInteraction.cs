using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInputHandler))]
public class PlayerPhysicsInteraction : MonoBehaviour
{
    private PlayerInputHandler _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInputHandler>();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        InteractableDoor door = hit.collider.GetComponentInParent<InteractableDoor>();

        if (door != null)
        {
            if (_playerInput.MoveInput.magnitude > 0.1f)
            {
                door.PhysicalPush(transform.position, _playerInput.IsSprinting);
            }
        }
    }
}