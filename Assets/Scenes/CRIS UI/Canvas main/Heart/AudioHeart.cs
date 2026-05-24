using UnityEngine;

public class AudioHeart : MonoBehaviour
{
    private AudioSource miAudio;
    private Animator miAnimator;

    public void PlayLatido() // Asegúrate de que diga "public"
    {
        if (miAudio != null)
        {
            miAudio.Play();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        miAudio = GetComponent<AudioSource>();
        miAnimator = GetComponent<Animator>();
    }
    // Esta función la puedes llamar desde otro script para activar/desactivar el latido
    public void SetPeligro(bool danger)
    {
        if (miAnimator != null)
        {
            miAnimator.SetBool("Danger", danger);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
