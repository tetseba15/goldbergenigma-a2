using UnityEngine;

public class laughtChildren : MonoBehaviour
{
    [Header("Configuración del Sonido")]
    
    [SerializeField] private AudioSource AudioSource;
    
    [SerializeField] private AudioClip clipLaught;

    
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Player"))
        {

            if (AudioSource != null && clipLaught != null)
            {
                AudioSource.PlayOneShot(clipLaught);
                Debug.Log("¡La risa de los niños ha sonado!");
            }
            else
            {

                Destroy(gameObject);
            }
        }
    }
}