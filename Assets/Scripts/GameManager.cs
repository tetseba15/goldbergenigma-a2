using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(2);

        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            SceneManager.LoadScene(3);

        }
    }

    private void OnEnable()
    {
        
    }

    public void GameOver()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}