using UnityEngine;

public class BloodBath : MonoBehaviour
{
    [Header("Blood")]
    [SerializeField] private GameObject _blood;
    [SerializeField] private ParticleSystem _bloodParticles;
    [SerializeField] private float _fillSpeed = 0.2f;

    [Header("Z position")]
    [SerializeField] private float _zmin;
    [SerializeField] private float _zmax;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _clip;

    [Header("Game Event Type")]
    [SerializeField] private GenericEventsTrigger.GameEventType _gameEventType;

    private float bloodHeight;
    private bool bobbinOpened;
    private Vector3 bloodPosition;

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += FillBath;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= FillBath;
    }

    private void Awake()
    {
        if (_blood != null)
        {
            bloodPosition = _blood.transform.localPosition;
        }

        if (_audioSource != null && _clip != null)
        {
            _audioSource.clip = _clip;
        }
    }

    private void Update()
    {
        if (bobbinOpened && bloodPosition.z < _zmax)
        {
            if (_bloodParticles.isStopped)
            {
                _bloodParticles.Play();
            }

            bloodHeight += _fillSpeed * Time.deltaTime;
            bloodHeight = Mathf.Clamp01(bloodHeight);

            bloodPosition.z = Mathf.Lerp(_zmin, _zmax, bloodHeight);

            _blood.transform.localPosition = bloodPosition;
        }

        if (bloodPosition.z >= _zmax)
        {
            _bloodParticles.Stop();
            _audioSource.Pause();
        }
    }

    private void FillBath(GenericEventsTrigger.GameEventType gameEventType)
    {
        if (gameEventType == _gameEventType)
        {
            bobbinOpened = true;
            _audioSource.Play();
        }
    }
}
