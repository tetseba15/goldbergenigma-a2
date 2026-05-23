using UnityEngine;

public class Mannequin : MonoBehaviour
{
    [SerializeField] private float _disappearDistance = 3f;
    private Transform _player;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (_player == null) return;

        float distance = Vector3.Distance(transform.position, _player.position);

        if (distance <= _disappearDistance)
        {
            gameObject.SetActive(false);
        }
    }
}