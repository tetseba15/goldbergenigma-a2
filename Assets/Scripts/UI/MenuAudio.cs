using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class MenuAudio : MonoBehaviour
{
    public static MenuAudio instance;
    private AudioSource mAudioSource;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        //Para que lo reproduzca y no destruya
        else
        {
            Destroy(gameObject);
        }
        mAudioSource = GetComponent<AudioSource>();
    }
    public void PlayGame()
    {

    }
    public void QuitGame()
    {
        Application.Quit();
    }

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
