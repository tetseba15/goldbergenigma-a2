using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FlashlightIKController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFlashlight _flashlight;
    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private Rig _rightArmRig;

    [Header("Hierarchy")]
    [SerializeField, Tooltip("Pivot or shoulder sway")]
    private Transform _aimParent;
    [SerializeField, Tooltip("Real hand bone")]
    private Transform _mixamoHandBone;
    [SerializeField, Tooltip("Flashlight main object")]
    private Transform _flashlightModel;

    [Header("Settings")]
    [SerializeField] private float _transitionSpeed = 5f;
    [SerializeField, Tooltip("Hand rotation while walking")]
    private Vector3 _handRotationOffset;

    private float _targetWeight = 1f;

    private void LateUpdate() 
    {
        bool shouldAim = _flashlight.IsOn() || _inputHandler.IsInspectingFlashlight;

        _targetWeight = shouldAim ? 1f : 0f;
        _rightArmRig.weight = Mathf.Lerp(_rightArmRig.weight, _targetWeight, Time.deltaTime * _transitionSpeed);

        if (!shouldAim)
        {
            _flashlightModel.position = _mixamoHandBone.position;

            _flashlightModel.rotation = _mixamoHandBone.rotation * Quaternion.Euler(_handRotationOffset);
        }
    }
}