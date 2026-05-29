using UnityEngine;

public class ShaderWarmup : MonoBehaviour
{
    [SerializeField] private ShaderVariantCollection _gameShaders;

    private void Start()
    {
        if (_gameShaders != null)
        {
            _gameShaders.WarmUp();
            Debug.Log("Shaders pre-compilados");
        }
    }
}