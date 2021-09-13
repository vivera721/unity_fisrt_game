using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePointAccumulator
{
    int gamePoint = 0;

    public int GamePoint
    {
        get
        {
            return gamePoint;
        }
    }

    public void Accumulator(int value)
    {
        gamePoint += value;
    }

    public void Reset()
    {
        gamePoint = 0;
    }

}
