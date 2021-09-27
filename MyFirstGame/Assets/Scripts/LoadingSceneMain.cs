using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneMain : BaseSceneMain
{
    const float TextUpdateIntaval = 0.15f;
    float LastUpdateTime;

    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if (currentTime - LastUpdateTime > TextUpdateIntaval)
        {
            LastUpdateTime = currentTime;
        }

        //
    }
}