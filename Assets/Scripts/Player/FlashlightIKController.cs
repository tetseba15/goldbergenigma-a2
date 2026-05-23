using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FlashlightIKController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private PlayerInputHandler _inputHandler;
    [SerializeField] private PlayerFlashlight _flashlight;
    [SerializeField] private Rig _rightArmRig; 
    
    [SerializeField, Tooltip("Shoulder or sway pivot")]
    private Transform _aimParent;

    [SerializeField] private Transform _mixamoHandBone;

    [SerializeField] private Transform _flashlightModel;

    [Header("Settings")]
    [SerializeField] private float _transitionSpeed = 5f;

    private float _targetWeight = 1f;



    private void Update()
    {
        bool shouldAim = _flashlight.IsOn() || _inputHandler.IsInspectingFlashlight; 

        _targetWeight = shouldAim ? 1f : 0f;

        
        _rightArmRig.weight = Mathf.Lerp(_rightArmRig.weight, _targetWeight, Time.deltaTime * _transitionSpeed);

        if (_rightArmRig.weight > 0.9f)
        {
            if (_flashlightModel.parent != _aimParent)
            {
                _flashlightModel.SetParent(_aimParent);
                _flashlightModel.localPosition = Vector3.Lerp(_flashlightModel.localPosition, Vector3.zero, Time.deltaTime * 10f);
            }
        }
        else if (_rightArmRig.weight < 0.1f)
        {
            if (_flashlightModel.parent != _mixamoHandBone)
            {
                _flashlightModel.SetParent(_mixamoHandBone);
            }
        }
    }
}