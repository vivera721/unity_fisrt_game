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

    public virtual void OnBulletHited(int damage)
    {
        Debug.Log("총알맞음! 데미지 = " + damage);
        DecreaseHP(damage);
    }

    public virtual void OnCrash(int damage)
    {
        Debug.Log("충돌! 데미지 = " + damage);
        DecreaseHP(damage);
    }

    void DecreaseHP(int value)
    {
        if (isDead)
            return;

        CurrentHP -= value;

        if (CurrentHP < 0)
            CurrentHP = 0;

        if(CurrentHP == 0)
        {
            OnDead();
        }
    }

    protected virtual void OnDead()
    {
        Debug.Log(name + " 죽음!");
        isDead = true;
    }


}
