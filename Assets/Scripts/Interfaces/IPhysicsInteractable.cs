using UnityEngine;

public interface IPhysicsInteractable
{
    string GetInteractPrompt(GameObject interactor);
    
    void OnGrabStart(GameObject interactor);
    
  
    void OnGrabUpdate(Vector2 mouseDelta); 
    
    void OnGrabEnd();
}
