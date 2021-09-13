using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    const float LifeTime = 10.0f;

    [SerializeField]
    Vector3 moveDirection = Vector3.zero;

    [SerializeField]
    float Speed = 0.0f;

    bool hited = false;

    float FireTime;

    bool NeedMove = false;

    [SerializeField]
    int Damage = 1;

    Actor Owner;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (ProcessDisappearCondition())
            return;

        UpdateMove();
    }

    void UpdateMove()
    {
        if (!NeedMove)
            return;

        Vector3 moveVector = moveDirection.normalized * Speed * Time.deltaTime;
        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;
    }

    public void Fire(Actor owner, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        Owner = owner;
        transform.position = firePosition;
        moveDirection = direction;
        Speed = speed;
        Damage = damage;

        FireTime = Time.time;
        NeedMove = true;
    }

    Vector3 AdjustMove(Vector3 moveVector)
    {
        RaycastHit hitInfo;

        // 라인 캐스트는 시작점 끝점이 있는 선
        // 그 선에 뭐가 걸리는지 체크
        if (Physics.Linecast(transform.position, transform.position + moveVector, out hitInfo))
        {
            Actor actor = hitInfo.collider.GetComponentInParent<Actor>();
            if (actor && actor.IsDead)
                return moveVector;

            moveVector = hitInfo.point - transform.position;
            OnBulletCollision(hitInfo.collider);
        }
        return moveVector;
    }

    void OnBulletCollision(Collider collider)
    {
        if (hited)
            return;
        if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet")
           || collider.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            return;
        }

        Actor actor = collider.GetComponentInParent<Actor>();
        if (actor && actor.IsDead)
            return;

        actor.OnBulletHited(Owner, Damage);

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        hited = true;
        NeedMove = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        OnBulletCollision(other);
    }

    bool ProcessDisappearCondition()
    {
        if (hited)
        {
            Disappear();
            return true;
        }


        if (transform.position.x > 6.0f || transform.position.x < -6.0f
            || transform.position.y > 6.0f || transform.position.y < -6.0f)
        {
            Disappear();
            return true;
        }
        else if (Time.time - FireTime > LifeTime)
        {
            Disappear();
            return true;
        }
        return false;
    }

    void Disappear()
    {
        Destroy(gameObject);
    }

}
