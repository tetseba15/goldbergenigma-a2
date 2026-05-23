using UnityEngine;
using UnityEngine.Audio;

public class DangerClose : AudioHeart
{
    
    [Header("Referencias")]
    public Transform jugador;
    public Transform enemigo;
    public AudioSource audioHeart; // Asegúrate de arrastrar el AudioSource aquí en el Inspector
    public Animator anim;
    [Header("Configuración")]
    public float detectionRange = 10f;

    private Animator animator;
    private bool isPlaying = false; // Faltaba declarar esta variable

    
    void Start()
    {
        animator = GetComponent<Animator>();

        // Validación por si olvidas asignar el AudioSource en el Inspector
        if (audioHeart == null)
            audioHeart = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Usamos una sola variable para la distancia
        float distancia = Vector3.Distance(jugador.position, enemigo.position);

        if (distancia <= detectionRange)
        {
            // Control de Animación
            animator.SetBool("CorazonLatido", true);

            // Control de Sonido: Solo si no se está reproduciendo ya
            if (!isPlaying)
            {
                audioHeart.Play();
                isPlaying = true;
            }
        }
        else
        {
            // Control de Animación
            animator.SetBool("CorazonLatido", false);

            // Control de Sonido: Detener si se aleja
            if (isPlaying)
            {
                audioHeart.Stop();
                isPlaying = false;
            }
        }
    }
}

