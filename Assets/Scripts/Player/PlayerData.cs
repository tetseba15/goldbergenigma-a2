using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;

    [Header("Camera")]
    public float lookSensitivity = 15f;
}