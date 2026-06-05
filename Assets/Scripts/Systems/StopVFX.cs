using UnityEngine;

public class StopVFX : MonoBehaviour
{
    [SerializeField] private float timer = 7f;
   

    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
