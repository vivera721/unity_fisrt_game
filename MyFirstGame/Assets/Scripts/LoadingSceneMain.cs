using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneMain : BaseSceneMain
{
    const float NextSceneIntaval = 3.0f;
    const float TextUpdateIntaval = 0.15f;
    float LastUpdateTime;
    float SceneStartTime;
    bool NextSceneCall = false;

    protected override void OnStart()
    {
        SceneStartTime = Time.time;
    }
    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if (currentTime - LastUpdateTime > TextUpdateIntaval)
        {
            LastUpdateTime = currentTime;
        }
        if (currentTime - SceneStartTime > NextSceneIntaval)
        {
            if (!NextSceneCall)
                GotoNextScene();
        }
    }
    void GotoNextScene()
    {
        SceneController.Instance.LoadScene(SceneNameConstants.InGame);
        NextSceneCall = true;
    }
}