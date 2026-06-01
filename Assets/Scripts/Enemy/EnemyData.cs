using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("Speed Settings")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 5.5f;

    [Header("Vision Settings")]
    [Range(1f, 50f)] public float viewRadius = 15f;
    [Range(1f, 360f)] public float viewAngle = 90f;

    [Header("Flashlight Settings")]
    public float flashlightRepelDistance = 10f;

    [Header("Teleport Settings")]
    public float minTeleportDistance = 8f;
    public float maxTeleportDistance = 15f;

    [Header("Appear Settings")]
    public float minAppearDuration = 20f;
    public float maxAppearDuration = 40f;
}