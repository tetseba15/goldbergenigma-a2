using System;
using UnityEngine;

public class UpstairsTrigger : MonoBehaviour
{
    public static event Action OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnTrigger?.Invoke();
        }
    }
}
