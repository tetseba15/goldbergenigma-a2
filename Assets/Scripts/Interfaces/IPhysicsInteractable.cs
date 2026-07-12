using UnityEngine;

public interface IPhysicsInteractable
{
    string GetInteractPrompt(GameObject interactor);
    
    void OnGrabStart(GameObject interactor, Vector3 grabPoint/*, Camera playerCamera*/);
    
  
    void OnGrabUpdate(Vector2 mouseDelta); 
    
    void OnGrabEnd();
}
