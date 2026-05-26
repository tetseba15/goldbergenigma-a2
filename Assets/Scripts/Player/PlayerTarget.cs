using UnityEngine;

public class PlayerTarget : MonoBehaviour
{
    public static PlayerTarget Instance { get; private set; }

    public Transform PlayerTransform { get; private set; }
    public PlayerFlashlight Flashlight { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            PlayerTransform = transform;
            Flashlight = GetComponent<PlayerFlashlight>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}