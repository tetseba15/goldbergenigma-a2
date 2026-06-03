using Unity.VisualScripting;
using UnityEngine;

public class SpawnMannequin : MonoBehaviour
{
    [SerializeField] private GameObject mannequinPrefab;

    public void ActivateMannequin()
    {
        mannequinPrefab.SetActive(true);
    }
}
