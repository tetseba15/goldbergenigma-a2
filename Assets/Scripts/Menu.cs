
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject exitButton;

    private void Start()
    {
    #if UNITY_WEBGL
        exitButton.SetActive(false);
    #endif
    }

    public void StarGame()
    {
        SceneManager.LoadScene("CasaTest1");// Escena del juego cuando le damos play
    }
    public void Exit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void BackTomenu()// option cuando el juego este en pausa
    {
        SceneManager.LoadScene("Menu");
    }

    public void Pause()
    {
        Time.timeScale = 0f;
    }
    public void Unpause()
    {
        Time.timeScale = 1f;
    }
    public void GameOver()
    {
        SceneManager.LoadScene("Game Over");

    }
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
}