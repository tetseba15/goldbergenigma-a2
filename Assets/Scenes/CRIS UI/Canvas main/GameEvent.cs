using System;
using UnityEngine;

public static class GameEvent
{
    public static Action <float,float> HolyWater;

    public static void holyWater(float currentWater, float maxWater)
    {
        HolyWater?. Invoke(currentWater, maxWater);//? Evita el null si esta vacio
    }
}

