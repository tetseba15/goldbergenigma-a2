using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FlashlightIKController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerFlashlight _flashlight;
    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private Rig _rightArmRig;
    [SerializeField] private Transform _ikTarget;

    [Header("Transforms")]
    [SerializeField, Tooltip("Pivot or shoulder sway")]
    private Transform _aimPivot;
    [SerializeField, Tooltip("Real hand bone")]
    private Transform _mixamoHandBone;
    [SerializeField, Tooltip("Flashlight main object")]
    private Transform _flashlightModel;

    [Header("Aming Settings")]
    [SerializeField] private float _transitionSpeed = 5f;
    [SerializeField, Tooltip("Precise local Position of the Target relative to Flashlight when ON/Inspecting")]
    private Vector3 _aimIKOffsetPosition; 
    [SerializeField, Tooltip("Precise local Rotation (Euler) of the Target relative to Flashlight when ON/Inspecting")]
    private Vector3 _aimIKOffsetRotation; 

    [Header("Resting Settings")]
    [SerializeField, Tooltip("Rotation offset to fit the flashlight in the relaxed hand bone")]
    private Vector3 _handVisualRotationOffset;

    private float _targetWeight = 1f;

    private void LateUpdate() 
    {
        // 1. Check if we should hold the flashlight up (ON or Inspecting)
        bool shouldAim = _flashlight.IsOn() || _inputHandler.IsInspectingFlashlight;

        // 2. Set target weight for the IK
        _targetWeight = shouldAim ? 1f : 0f;

        // 3. Smoothly transition the IK weight
        _rightArmRig.weight = Mathf.Lerp(_rightArmRig.weight, _targetWeight, Time.deltaTime * _transitionSpeed);

        if (shouldAim)
        {

            if (_ikTarget.parent != _flashlightModel)
            {
                //force precise local offsets
                _ikTarget.SetParent(_flashlightModel, false);
            }

            
            _ikTarget.localPosition = _aimIKOffsetPosition;
            _ikTarget.localRotation = Quaternion.Euler(_aimIKOffsetRotation);
        }
        else
        {
           
            if (_ikTarget.parent != _aimPivot)
            {
                _ikTarget.SetParent(_aimPivot, false); 
                _ikTarget.localPosition = Vector3.zero;
                _ikTarget.localRotation = Quaternion.identity;
            }

            // Lerp the flashlight position seamlessly from the Aim Pivot down to the Mixamo Hand Bone
            _flashlightModel.position = Vector3.Lerp(_mixamoHandBone.position, _aimPivot.position, _rightArmRig.weight);

            // Calculate the resting rotation with the offset applied to the hand bone
            Quaternion restingVisualRotation = _mixamoHandBone.rotation * Quaternion.Euler(_handVisualRotationOffset);

            // Slerp (Spherical Lerp) for smooth rotation blending
            _flashlightModel.rotation = Quaternion.Slerp(restingVisualRotation, _aimPivot.rotation, _rightArmRig.weight);
        }
    }
}