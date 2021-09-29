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
    float ItemDropRate;     // ������ ���� Ȯ��

    [SerializeField]
    int ItemDropID;         // ������ ������ ������ ItemDrop ���̺��� �ε���

    public string FilePath
    {
        get;
        set;
    }


    Vector3 AppearPoint;      // ����� ���� ��ġ
    Vector3 DisappearPoint;      // ����� ��ǥ ��ġ

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
                Debug.LogError("���°� ���ǵ��� �ʾҽ��ϴ�!");
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

        CurrentHP = MaxHP = enemyStruct.MaxHP;             // CurrentHP���� �ٽ� �Է�
        Damage = enemyStruct.Damage;                       // �Ѿ� ������
        crashDamage = enemyStruct.CrashDamage;             // �浹 ������
        BulletSpeed = enemyStruct.BulletSpeed;             // �Ѿ� �ӵ�
        FireRemainCount = enemyStruct.FireRemainCount;     // �߻��� �Ѿ� ����
        GamePoint = enemyStruct.GamePoint;                 // �ı��� ���� ����

        AppearPoint = new Vector3(data.AppearPointX, data.AppearPointY, 0);             // ����� ���� ��ġ 
        DisappearPoint = new Vector3(data.DisappearPointX, data.DisappearPointY, 0);    // ����� ��ǥ ��ġ
        ItemDropRate = enemyStruct.ItemDropRate;    // ������ ���� Ȯ��
        ItemDropID = enemyStruct.ItemDropID;        // ������ Drop ���̺� ���� �ε���
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
        // ������ ���� Ȯ���� �˻�
        float ItemGen = Random.Range(0.0f, 100.0f);
        if (ItemGen > ItemDropRate)
            return;

        ItemDropTable itemDropTable = SystemManager.Instance.ItemDropTable;
        ItemDropStruct dropStruct = itemDropTable.GetDropData(ItemDropID);

        // ��� �������� ������ ������ Ȯ�� �˻�
        ItemGen = Random.Range(0, dropStruct.Rate1 + dropStruct.Rate2 + dropStruct.Rate3);
        int ItemIndex = -1;

        if (ItemGen <= dropStruct.Rate1)     // 1�� ������ �������� ���� ���
            ItemIndex = dropStruct.ItemID1;
        else if (ItemGen <= (dropStruct.Rate1 + dropStruct.Rate2))   // 2�� ������ �������� ���� ���
            ItemIndex = dropStruct.ItemID2;
        else //if (ItemGen <= (dropStruct.Rate1 + dropStruct.Rate2 + dropStruct.Rate3)) // 3�� ������ ������ ���
            ItemIndex = dropStruct.ItemID3;

        Debug.Log("GenerateItem ItemIndex = " + ItemIndex);

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        inGameSceneMain.ItemBoxManager.Generate(ItemIndex, transform.position);
    }
}
