using UnityEngine;

public class TrailerMoveObject : MonoBehaviour
{
    [SerializeField] private float moveX;
    [SerializeField] private float moveY;
    [SerializeField] private float moveZ;

    [SerializeField] private float rotateX;
    [SerializeField] private float rotateY;
    [SerializeField] private float rotateZ;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(moveX * Time.deltaTime, moveY * Time.deltaTime, moveZ * Time.deltaTime);

        transform.Rotate(rotateX * Time.deltaTime, rotateY * Time.deltaTime, rotateZ * Time.deltaTime);
    }
}
