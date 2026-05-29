using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Cinemachine; 

public class PlayerPanicReaction : MonoBehaviour
{
    [Header("References")]
    [SerializeField, Tooltip("Reference to the player's movement script or speed variable")]
    private PlayerMovement _playerMovementScript; 

    [Header("Visual & Camera Effects")]
    [SerializeField, Tooltip("A local URP Volume dedicated to panic effects")]
    private Volume _panicVolume;
    [SerializeField, Tooltip("Cinemachine Impulse Source to shake the camera")]
    private CinemachineImpulseSource _impulseSource;

    [Header("Settings")]
    [SerializeField, Range(0f, 1f), Tooltip("How much to slow down the player (0.2 = 20% speed)")]
    private float _speedMultiplier = 0.2f;
    [SerializeField, Tooltip("How fast the blur fades in and out")]
    private float _visualFadeSpeed = 5f;

    private Coroutine _panicCoroutine;

    private void OnEnable()
    {
        EnemyAI.OnEnemyRoaring += TriggerPanic;
    }

    private void OnDisable()
    {
        EnemyAI.OnEnemyRoaring -= TriggerPanic;
    }

    private void TriggerPanic(float roarDuration, float invulnerabilityDuration)
    {
        if (_panicCoroutine != null)
        {
            StopCoroutine(_panicCoroutine);
        }

        float stunDuration = roarDuration / 2;
        _panicCoroutine = StartCoroutine(PanicRoutine(stunDuration));
    }

    private IEnumerator PanicRoutine(float stunDuration)
    {

        _playerMovementScript.SpeedMultiplier *= _speedMultiplier;

        if (_impulseSource != null)
        {
            _impulseSource.GenerateImpulse();
        }

        // 2. FADE IN VISUAL DISTORTION (URP Volume)
        if (_panicVolume != null)
        {
            while (_panicVolume.weight < 1f)
            {
                _panicVolume.weight = Mathf.MoveTowards(_panicVolume.weight, 1f, Time.deltaTime * _visualFadeSpeed);
                yield return null;
            }
        }

        yield return new WaitForSeconds(stunDuration);


        _playerMovementScript.SpeedMultiplier = 1.0f;

        if (_panicVolume != null)
        {
            while (_panicVolume.weight > 0f)
            {
                _panicVolume.weight = Mathf.MoveTowards(_panicVolume.weight, 0f, Time.deltaTime * (_visualFadeSpeed / 2f)); // Fades out slower for effect
                yield return null;
            }
        }
    }
}