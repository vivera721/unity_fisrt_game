using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    [SerializeField]
    Vector3 MoveVector = Vector3.zero;
    [SerializeField]
    float Speed;
    [SerializeField]
    BoxCollider boxCollider;
    [SerializeField]
    Transform MainBGQuadTransform;

    [SerializeField]
    Transform FireTransformLeft;
    [SerializeField]
    Transform FireTransformRight;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1;

    private Transform tp;

    protected override void UpdateActor()
    {
        UpdateMove();
    }

    void UpdateMove()
    {
        if (MoveVector.sqrMagnitude == 0)
            return;

        MoveVector = AdjustMoveVector(MoveVector);

        transform.position += MoveVector;
    }

    public void ProcessInput(Vector3 moveDirection)
    {
        MoveVector = moveDirection * Speed * Time.deltaTime;
    }

    Vector3 AdjustMoveVector(Vector3 moveVector)
    {
        Vector3 result = Vector3.zero;

        result = boxCollider.transform.position + boxCollider.center + moveVector;

        if (result.x - boxCollider.size.x * 0.5f < -MainBGQuadTransform.localScale.x * 0.5f)
            moveVector.x = 0;
        if (result.x + boxCollider.size.x * 0.5f > MainBGQuadTransform.localScale.x * 0.5f)
            moveVector.x = 0;
        if (result.y - boxCollider.size.y * 0.5f < -MainBGQuadTransform.localScale.y * 0.5f)
            moveVector.y = 0;
        if (result.y + boxCollider.size.y * 0.5f > MainBGQuadTransform.localScale.y * 0.5f)
            moveVector.y = 0;
        return moveVector;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("other = " + other.name);
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
            if (!enemy.IsDead)
                enemy.OnCrash(this, CrashDamage);
    }

    public override void OnCrash(Actor attacker, int damage)
    {
        base.OnCrash(attacker, damage);
    }

    public void Fire()
    {

        GameObject leftgo = Instantiate(Bullet);
        GameObject rightgo = Instantiate(Bullet);
        Bullet leftbullet = leftgo.GetComponent<Bullet>();
        Bullet rightbullet = rightgo.GetComponent<Bullet>();
        leftbullet.Fire(this, FireTransformLeft.position, FireTransformLeft.up, BulletSpeed, Damage);
        rightbullet.Fire(this, FireTransformRight.position, FireTransformRight.up, BulletSpeed, Damage);

    }

}
