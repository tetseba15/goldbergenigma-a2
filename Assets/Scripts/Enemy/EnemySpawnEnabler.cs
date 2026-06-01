using UnityEngine;
using System.Collections;

public class EnemySpawnEnabler : MonoBehaviour
{
    [SerializeField] private float _delayAfterEntering = 30f;
    [SerializeField] private float _minReappearDelay = 60f;
    [SerializeField] private float _maxReappearDelay = 80f;
    [SerializeField] private EnemyAI _enemy;
    private bool _triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_triggered) return;
        if (other.CompareTag("Player"))
        {
            _triggered = true;
            StartCoroutine(EnableEnemyAfterDelay());
        }
    }

    private IEnumerator EnableEnemyAfterDelay()
    {
        yield return new WaitForSeconds(_delayAfterEntering);
        SpawnEnemy();
    }

    public void SpawnEnemy()
    {
        Debug.Log("SpawnEnemy llamado");
        _enemy.gameObject.SetActive(true);
        StartCoroutine(TeleportNextFrame());
    }

    private IEnumerator TeleportNextFrame()
    {
        yield return null;
        _enemy.TeleportNow();
    }

    public void DespawnEnemy()
    {
        _enemy.gameObject.SetActive(false);
        StartCoroutine(ReappearAfterDelay());
    }

    private IEnumerator ReappearAfterDelay()
    {
        float delay = Random.Range(_minReappearDelay, _maxReappearDelay);
        yield return new WaitForSeconds(delay);
        SpawnEnemy();
    }
}