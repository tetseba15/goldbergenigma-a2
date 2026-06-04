using UnityEngine;

public class DangerClose : MonoBehaviour
{
   
    public Transform jugador;
    public Transform enemigo;
    public float distanciaUmbral = 5f; 
    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {

        
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
