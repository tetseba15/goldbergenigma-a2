using UnityEngine;

public class ChangingPicture : MonoBehaviour
{
    [Header("Texture")]
    [SerializeField] private Texture2D _texture;
    [SerializeField] private Vector2 _textureOffset;

    [Header("Game Event Type")]
    [SerializeField] private GenericEventsTrigger.GameEventType _gameEventType;

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += ChangePicture;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= ChangePicture;
    }

    private void ChangePicture(GenericEventsTrigger.GameEventType gameEventType)
    {
        if (gameEventType == _gameEventType)
        {
            if (_texture != null)
            {
                Renderer renderer = GetComponent<Renderer>();
                renderer.material.SetTexture("_BaseMap", _texture);
                renderer.material.SetTextureOffset("_BaseMap", new(_textureOffset.x, _textureOffset.y));
            }
               
        }
    }
}
