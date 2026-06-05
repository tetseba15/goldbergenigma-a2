
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [SerializeField] private GameObject exitButton;
    private bool isPaused = false;
    private void Start()
    {
    #if UNITY_WEBGL
       exitButton.SetActive(false);
    #endif
    }
   
    public void StarGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(1);
    }
    public void Exit()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void BackTomenu()
    {
       
        SceneManager.LoadScene("Menu");
    }

   
    public void GameOver()
    {
        SceneManager.LoadScene("Game Over");

    }
    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void Win()
    {
        SceneManager.LoadScene("Win");
    }
}