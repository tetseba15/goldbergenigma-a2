using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private float timer = 4f;

    private float originalTimer;

    private void Start()
    {
        originalTimer = timer; 
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = originalTimer;

            SceneManager.LoadScene(1);
        }
    }
}
