using UnityEngine;

public class SoundsCar : MonoBehaviour
{
    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource miReproductorCentral;

    [Header ("Fuentes de audio")]
    [SerializeField] private AudioSource openDoorSource;
    [SerializeField] private AudioSource closeDoorSource;
    [SerializeField] private AudioSource radioSource;
    [SerializeField] private AudioSource valizasSource;

    private bool openDoor = false;
    private bool onRadio= false;
    private bool onValizas = false;

    public void OpenDoorCar()
    {
        if (!openDoor)
        {
            if (openDoorSource != null) openDoorSource.Play();
            openDoor = true;
        }
        else
        {
            if (closeDoorSource != null) closeDoorSource.Play();
            openDoor = false;
            
        }
    }
}
