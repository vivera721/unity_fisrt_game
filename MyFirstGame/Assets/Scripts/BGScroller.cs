using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BGScrollData
{
    public Renderer RenderForScroll;
    public float Speed;
    public float OffsetY;
}

public class BGScroller : MonoBehaviour
{
    [SerializeField]
    BGScrollData[] ScrollDatas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScroll();
    }

    void UpdateScroll()
    {
        for (int i = 0; i < ScrollDatas.Length; i++)
        {
            SetTextureOffset(ScrollDatas[i]);
        }
    }

    void SetTextureOffset(BGScrollData scrollData)
    {
        scrollData.OffsetY += (float)(scrollData.Speed) * Time.deltaTime;

        if (scrollData.OffsetY > 1)
            scrollData.OffsetY = scrollData.OffsetY % 1.0f;

        Vector2 Offset = new Vector2(0,scrollData.OffsetY);

        scrollData.RenderForScroll.material.SetTextureOffset("_MainTex", Offset);
    }
}
