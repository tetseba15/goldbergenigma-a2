using System;
using UnityEngine;

public static class NoiseManager
{
    //                     position and volume (radious)
    public static event Action<Vector3, float> OnNoiseEmitted;

    // Use this method to emit "noises" to the enemy 
    public static void EmitNoise(Vector3 position, float volume)
    {
        OnNoiseEmitted?.Invoke(position, volume);
    }
}