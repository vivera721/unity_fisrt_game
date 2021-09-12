using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    public enum State
    {
        None = -1,
        Ready = 0,
        Appear,
        Battle,
        Dead,
        Disappear,
    }

    [SerializeField]
    State CurrenState = State.None;

    const float MaxSpeed = 10.0f;
    const float MaxSpeedTime = 0.5f;

    [SerializeField]
    Vector3 TargetPosition;
    [SerializeField]
    float CurrentSpeed;

    Vector3 CurrentVelocity;

    float MoveStartTime = 0.0f;


    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1;

    float LastBattleUpdateTime = 0.0f;

    [SerializeField]
    int FireRemainCount = 1;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected override void UpdateActor()
    {

        switch (CurrenState)
        {
            case State.None:
            case State.Ready:
                break;
            case State.Dead:
                break;
            case State.Appear:
            case State.Disappear:
                UpdateSpeed();
                UpdateMove();
                break;
            case State.Battle:
                UpdateBattle();
                break;
            default:
                Debug.LogError("상태가 정의되지 않았습니다!");
                break;
        }

    }

    void UpdateSpeed()
    {
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, (Time.time - MoveStartTime) / MaxSpeedTime);
    }
    void UpdateMove()
    {
        float distance = Vector3.Distance(TargetPosition, transform.position);
        if (distance == 0)
        {
            Arrived();
            return;
        }

        CurrentVelocity = (TargetPosition - transform.position).normalized * CurrentSpeed;

        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref CurrentVelocity, distance / CurrentSpeed, MaxSpeed);

    }
    void Arrived()
    {
        CurrentSpeed = 0.0f;
        if (CurrenState == State.Appear)
        {
            CurrenState = State.Battle;
            LastBattleUpdateTime = Time.time;
        }
        else
        {
            CurrenState = State.None;
        }
    }
    public void Appear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = MaxSpeed;

        CurrenState = State.Appear;
        MoveStartTime = Time.time;
    }
    void Disappear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = 0;

        CurrenState = State.Disappear;
        MoveStartTime = Time.time;
    }
    void UpdateBattle()
    {
        if (Time.time - LastBattleUpdateTime > 1.0f)
        {
            if (FireRemainCount > 0)
            {
                Fire();
                FireRemainCount--;
            }
            else
            {
                Disappear(new Vector3(transform.position.x, -6.0f, transform.position.z));
            }

            LastBattleUpdateTime = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other = " + other.name);

        Player player = other.GetComponentInParent<Player>();
        if (player)
            if(!player.IsDead)
                player.OnCrash(this, CrashDamage);
    }
    public void OnCrash(Player player, int damage)
    {
        Debug.Log("oncrash player = " + player);
        OnCrash(damage);
    }
    public void Fire()
    {
        Vector3 player1 = new Vector3(0, 0, 0);
        player1 = SystemManager.Instance.Hero.transform.position;

        GameObject go = Instantiate(Bullet);
        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Fire(OwnerSide.Enemy, FireTransform.position, player1, BulletSpeed,Damage);

    }

    protected override void OnDead()
    {
        base.OnDead();

        CurrenState = State.Dead;
    }
}
