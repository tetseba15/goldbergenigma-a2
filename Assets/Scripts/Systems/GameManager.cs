using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private bool isPaused = false;
    
    [Header("UI Referencias")]
    
    public GameObject menuPausaUI;
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
        // -------- DEBUGGING -----------

        //if (Keyboard.current.digit9Key.wasPressedThisFrame)
        //{
        //    SceneManager.LoadScene(2);

        //}
        //if (Keyboard.current.digit8Key.wasPressedThisFrame)
        //{
        //    SceneManager.LoadScene(3);
        //}

    }
    //public void Pause()
    //{
    //    Time.timeScale = 0f;          
    //    AudioListener.pause = true;   
    //    isPaused = true;
    //    if (menuPausaUI != null) menuPausaUI.SetActive(true);

    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = true;
    //}

    //public void Unpause()
    //{
    //    Time.timeScale = 1f;          
    //    AudioListener.pause = false;  
    //    isPaused = false;
    //    if (menuPausaUI != null) menuPausaUI.SetActive(false);

    //    Cursor.lockState = CursorLockMode.None;
    //    Cursor.visible = false;

    //}
    private void OnEnable()
    {
        
    }

    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void GameOver()
    {
        SceneManager.LoadScene(2);
    }

    public void Win()
    {
        SceneManager.LoadScene("Win");
    }
}