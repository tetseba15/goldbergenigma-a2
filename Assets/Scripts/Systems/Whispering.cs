using System;
using UnityEngine;

public class Whispering : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip _clip;

    [Header("Stun duration")]
    [SerializeField] private float _stunDuration = 5f;

    [Header("Whisper distortion duration")]
    [SerializeField] private float _whisperDistortionDuration = 20f;

    [Header("Game event type")]
    [SerializeField] private GenericEventsTrigger.GameEventType _gameEventType;

    public static Action<float> OnWhispering;

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += StartWhispers;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= StartWhispers;
    }

    private void StartWhispers(GenericEventsTrigger.GameEventType gameEventType)
    {
        if (gameEventType == _gameEventType)
        {
            AudioManager.Instance.PlaySFX(_clip, 1f);
            EnemyAI.TriggerRoar(_stunDuration, _stunDuration);

            OnWhispering?.Invoke(_whisperDistortionDuration);
        }
    }
}
