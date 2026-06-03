using UnityEngine;

public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnemyAI enemy = FindFirstObjectByType<EnemyAI>();
            if (enemy != null)
            {
                enemy.SetPlayerInSafeZone(true);
                enemy.ForcePatrol();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EnemyAI enemy = FindFirstObjectByType<EnemyAI>();
            if (enemy != null)
            {
                enemy.SetPlayerInSafeZone(false);
            }
        }
    }
}