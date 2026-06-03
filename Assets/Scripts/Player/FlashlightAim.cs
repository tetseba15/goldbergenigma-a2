using UnityEngine;

public class FlashlightAim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraRoot;

    private void LateUpdate()
    {
        if (_cameraRoot == null) return;

        // X inclination
        float cameraPitch = _cameraRoot.localEulerAngles.x;

        transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }
}