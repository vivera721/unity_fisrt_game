using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    protected int MaxHP = 100;

    [SerializeField]
    protected int CurrentHP;

    [SerializeField]
    protected int Damage = 1;

    [SerializeField]
    protected int crashDamage = 100;

    [SerializeField]
    bool isDead = false;

    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    [SerializeField]
    int UsableItemCount = 0;

    public int ItemCount
    {
        get
        {
            return UsableItemCount;
        }
    }
    protected int CrashDamage
    {
        get
        {
            return crashDamage;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActor();
    }

    protected virtual void Initialize()
    {
        CurrentHP = MaxHP;
    }

    protected virtual void UpdateActor()
    {

    }

    public virtual void OnBulletHited(Actor attacker, int damage)
    {
        Debug.Log("총알맞음! 데미지 = " + damage);
        DecreaseHP(attacker,damage);
    }

    public virtual void OnCrash(Actor attacker, int damage)
    {
        Debug.Log("충돌! 어태커 = " + attacker.name + "데미지 = " + damage);
        DecreaseHP(attacker,damage);
    }

    protected virtual void DecreaseHP(Actor attacker,int value)
    {
        if (isDead)
            return;

        CurrentHP -= value;

        if (CurrentHP < 0)
            CurrentHP = 0;

        if(CurrentHP == 0)
        {
            OnDead(attacker);
        }
    }
    protected virtual void InternalIncreaseHP(int value)
    {
        if (isDead)
            return;

        CurrentHP += value;

        if (CurrentHP > MaxHP)
            CurrentHP = MaxHP;
    }

    public virtual void IncreaseHP(int value)
    {
        if (isDead)
            return;

    }

    public void CmdIncreaseHP(int value)
    {
        InternalIncreaseHP(value);
    }

    public virtual void IncreaseUsableItem(int value = 1)
    {
        if (isDead)
            return;
        //
        CmdIncreaseUsableItem(value);
    }

    public void CmdIncreaseUsableItem(int value)
    {
        UsableItemCount += value;
    }

    protected virtual void OnDead(Actor killer)
    {
        Debug.Log(name + " 죽음!");
        isDead = true;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectManager.ActorDeadFxIndex, transform.position);
    }


}
