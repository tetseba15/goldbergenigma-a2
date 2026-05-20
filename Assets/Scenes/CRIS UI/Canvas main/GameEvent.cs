using System;
using UnityEngine;

public static class GameEvent
{
    public static Action <float,float> OnHolyWater;

    public static void holyWater(float currentWater, float maxWater)
    {
        OnHolyWater?. Invoke(currentWater, maxWater);//? Evita el null si esta vacio
    }
}

