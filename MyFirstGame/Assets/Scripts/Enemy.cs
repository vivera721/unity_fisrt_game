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
    State CurrentState = State.None;

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
    float BulletSpeed = 1;

    float LastActionUpdateTime = 0.0f;

    [SerializeField]
    int FireRemainCount = 1;

    [SerializeField]
    int GamePoint = 10;

    [SerializeField]
    float ItemDropRate;     // 아이템 생성 확률

    [SerializeField]
    int ItemDropID;         // 아이템 생성시 참조할 ItemDrop 테이블의 인덱스

    public string FilePath
    {
        get;
        set;
    }


    Vector3 AppearPoint;      // 입장시 도착 위치
    Vector3 DisappearPoint;      // 퇴장시 목표 위치

    // Update is called once per frame
    protected override void UpdateActor()
    {

        switch (CurrentState)
        {
            case State.None:
                break;
            case State.Ready:
                UpdateReady();
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
        if (CurrentState == State.Appear)
        {
            CurrentState = State.Battle;
            LastActionUpdateTime = Time.time;
        }
        else
        {
            CurrentState = State.None;
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);
        }
    }
    public void Reset(SquadronMemberStruct data)
    {
        EnemyStruct enemyStruct = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID);

        CurrentHP = MaxHP = enemyStruct.MaxHP;             // CurrentHP까지 다시 입력
        Damage = enemyStruct.Damage;                       // 총알 데미지
        crashDamage = enemyStruct.CrashDamage;             // 충돌 데미지
        BulletSpeed = enemyStruct.BulletSpeed;             // 총알 속도
        FireRemainCount = enemyStruct.FireRemainCount;     // 발사할 총알 갯수
        GamePoint = enemyStruct.GamePoint;                 // 파괴시 얻을 점수

        AppearPoint = new Vector3(data.AppearPointX, data.AppearPointY, 0);             // 입장시 도착 위치 
        DisappearPoint = new Vector3(data.DisappearPointX, data.DisappearPointY, 0);    // 퇴장시 목표 위치
        ItemDropRate = enemyStruct.ItemDropRate;    // 아이템 생성 확률
        ItemDropID = enemyStruct.ItemDropID;        // 아이템 Drop 테이블 참조 인덱스
        CurrentState = State.Ready;
        LastActionUpdateTime = Time.time;
    }
    public void Appear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = MaxSpeed;

        CurrentState = State.Appear;
        MoveStartTime = Time.time;
    }
    void Disappear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = 0;

        CurrentState = State.Disappear;
        MoveStartTime = Time.time;
    }
    void UpdateReady()
    {
        if (Time.time - LastActionUpdateTime > 1.0f)
        {
            Appear(AppearPoint);
        }
    }
    void UpdateBattle()
    {
        if (Time.time - LastActionUpdateTime > 1.0f)
        {
            if (FireRemainCount > 0)
            {
                Fire();
                FireRemainCount--;
            }
            else
            {
                Disappear(DisappearPoint);
            }

            LastActionUpdateTime = Time.time;
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
    public override void OnCrash(Actor attacker, int damage)
    {
        base.OnCrash(attacker, damage);
    }
    public void Fire()
    {
        
        Vector3 player1 = new Vector3(SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Hero.transform.position.x, SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Hero.transform.position.y - 3.5f, 0);
        
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.EnemyBulletIndex);
        bullet.Fire(this, FireTransform.position, player1, BulletSpeed,Damage);
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);
        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        inGameSceneMain.GamePointAccumulator.Accumulator(GamePoint);
        inGameSceneMain.EnemyManager.RemoveEnemy(this);
        GenerateItem();

        CurrentState = State.Dead;
    }
    void GenerateItem()
    {
        // 아이템 생성 확율을 검사
        float ItemGen = Random.Range(0.0f, 100.0f);
        if (ItemGen > ItemDropRate)
            return;

        ItemDropTable itemDropTable = SystemManager.Instance.ItemDropTable;
        ItemDropStruct dropStruct = itemDropTable.GetDropData(ItemDropID);

        // 어느 아이템을 생성할 것인지 확율 검사
        ItemGen = Random.Range(0, dropStruct.Rate1 + dropStruct.Rate2 + dropStruct.Rate3);
        int ItemIndex = -1;

        if (ItemGen <= dropStruct.Rate1)     // 1번 아이템 비율보다 작은 경우
            ItemIndex = dropStruct.ItemID1;
        else if (ItemGen <= (dropStruct.Rate1 + dropStruct.Rate2))   // 2번 아이템 비율보다 작은 경우
            ItemIndex = dropStruct.ItemID2;
        else //if (ItemGen <= (dropStruct.Rate1 + dropStruct.Rate2 + dropStruct.Rate3)) // 3번 아이템 비율인 경우
            ItemIndex = dropStruct.ItemID3;

        Debug.Log("GenerateItem ItemIndex = " + ItemIndex);

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        inGameSceneMain.ItemBoxManager.Generate(ItemIndex, transform.position);
    }
}
