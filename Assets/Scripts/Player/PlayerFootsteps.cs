using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource _footstepSource;

    [Header("Audio Clips by Surface")]
    [SerializeField] private AudioClip[] _interiorClips; // Default
    [SerializeField] private AudioClip[] _exteriorClips;
    [SerializeField] private AudioClip[] _echoClips;

    [Header("Surface Detection")]
    [SerializeField, Tooltip("Layer del suelo para que el Raycast no choque con el jugador")]
    private LayerMask _groundLayer;
    [SerializeField] private string _exteriorTag = "SurfaceExterior";
    [SerializeField] private string _echoTag = "SurfaceEcho";

    [SerializeField, Tooltip("Longitud del rayo hacia abajo")]
    private float _raycastDistance = 1.5f;

    public void PlayStep()
    {
        // Default clips
        AudioClip[] selectedClips = _interiorClips;

        // Type of floor logic
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _raycastDistance, _groundLayer))
        {
            if (hit.collider.CompareTag(_exteriorTag))
            {
                selectedClips = _exteriorClips;
            }
            else if (hit.collider.CompareTag(_echoTag))
            {
                selectedClips = _echoClips;
            }
        }

        if (selectedClips == null || selectedClips.Length == 0) return;

        _footstepSource.pitch = Random.Range(0.9f, 1.1f);
        _footstepSource.volume = Random.Range(0.15f, 0.25f);

        AudioClip randomStep = selectedClips[Random.Range(0, selectedClips.Length)];
        _footstepSource.PlayOneShot(randomStep);
    }
}