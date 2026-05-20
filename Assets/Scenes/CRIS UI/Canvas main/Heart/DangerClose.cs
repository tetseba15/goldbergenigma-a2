using UnityEngine;

public class DangerClose : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform jugador;
    public Transform enemigo;
    public float distanciaUmbral = 5f; // A qué distancia empieza a latir
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {

        // Calculamos la distancia entre el jugador y el enemigo
        float distancia = Vector3.Distance(jugador.position, enemigo.position);

        if (distancia < distanciaUmbral)
        {
            animator.SetBool("Danger", true);
        }
        else
        {
            animator.SetBool("Danger", false);
        }

    }
}
