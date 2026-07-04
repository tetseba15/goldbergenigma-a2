using UnityEngine;

public class ChangingPicture : MonoBehaviour
{
    [Header("Texture Settings")]
    [SerializeField] private Texture2D _texture;
    [SerializeField] private Vector2 _textureOffset;
    [SerializeField]
    [Tooltip("Cantidad de tiempo que mantiene la nueva textura (de ser 0 no vuelve a la textura original)")] private float _changedTime;

    [Header("Game Event Type")]
    [SerializeField] private GenericEventsTrigger.GameEventType _gameEventType;

    private Renderer _renderer;
    private Texture2D _originTexture;
    private Texture2D _actualTexture;

    private Vector2 _originOffset;

    private float _time;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _originTexture = GetComponent<Renderer>().material.mainTexture as Texture2D;
        _actualTexture = _originTexture;

        _originOffset = _renderer.material.GetTextureOffset("_BaseMap");
    }

    private void Update()
    {
        if (_changedTime == 0) return;

        if (_actualTexture != _originTexture)
        {
            _time += Time.deltaTime;

            if (_time >= _changedTime)
            {
                ChangePicture(_originTexture);
                _renderer.material.SetTextureOffset("_BaseMap", _originOffset);
            }
        }
    }

    private void OnEnable()
    {
        GenericEventsTrigger.OnTriggerEvent += TriggerChange;
    }

    private void OnDisable()
    {
        GenericEventsTrigger.OnTriggerEvent -= TriggerChange;
    }

    private void TriggerChange(GenericEventsTrigger.GameEventType gameEventType)
    {
        if (gameEventType == _gameEventType)
        {
            ChangePicture(_texture);
            _renderer.material.SetTextureOffset("_BaseMap", new(_textureOffset.x, _textureOffset.y));
        }
    }

    private void ChangePicture(Texture2D texture)
    {
        if (texture != null)
        {
            _renderer.material.SetTexture("_BaseMap", texture);
            _actualTexture = texture;
        }
    }
}
