using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    [SerializeField]
    Transform SelfTransform;

    [SerializeField]
    Vector3 RotateAngle = new Vector3(0.0f, 0.5f, 0.0f);

    [SerializeField]
    string filePath;

    public string FilePath
    {
        get
        {
            return filePath;
        }
        set
        {
            filePath = value;
        }
    }

    Color chColor = Color.clear;

    bool ischColor = false;

    float duration = 5;
    float smoothness = 0.02f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateRotate();
        UpdateColor();
    }

    void UpdateRotate()
    {
        Vector3 eulerAngles = SelfTransform.localRotation.eulerAngles;
        eulerAngles += RotateAngle;
        SelfTransform.Rotate(RotateAngle, Space.Self);
    }

    void UpdateColor()
    {
        float progress = 0;
        float increment = smoothness / duration;
        if (!ischColor)
        {
            while (progress < 1)
            {
                chColor = Color.Lerp(Color.white, Color.gray, progress);
                progress += increment;
            }
            ischColor = true;
        }
        else
        {
            while (progress < 1)
            {
                chColor = Color.Lerp(Color.white, Color.gray, progress);
                progress += increment;
            }
            ischColor = false;
        }

    }

}
