using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    const int HPRecoveryValue = 20;
    const int ScoreAddValue = 100;

    public enum ItemEffect : int
    {
        HPRecovery = 0,
        ScoreAdd,
        UsableItemAdd,
    }

    [SerializeField]
    ItemEffect itemEffect = ItemEffect.HPRecovery;

    [SerializeField]
    Transform SelfTransform;

    [SerializeField]
    Vector3 RotateAngle = new Vector3(0.0f, 0.5f, 0.0f);

    [SerializeField]
    string filePath;

    [SerializeField]
    Vector3 MoveVector = Vector3.zero;

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
        UpdateMove();
    }

    void UpdateMove()
    {
        SelfTransform.position += MoveVector * Time.deltaTime;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
            return;

        OnItemCollision(other);
    }

    void OnItemCollision(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null)
            return;

        if (player.IsDead)
            return; 

        if (player)
        {

            switch (itemEffect)
            {
                case ItemEffect.HPRecovery:
                    player.IncreaseHP(HPRecoveryValue);
                    break;
                case ItemEffect.ScoreAdd:
                    InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
                    inGameSceneMain.GamePointAccumulator.Accumulator(ScoreAddValue);
                    break;
                case ItemEffect.UsableItemAdd:
                    player.IncreaseUsableItem();
                    break;
            }
        }

        Disappear();
    }
    void Disappear()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemBoxManager.Remove(this);
    }
}
