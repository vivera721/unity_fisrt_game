using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OwnerSide : int
{
    Player = 0,
    Enemy
}

public class Bullet : MonoBehaviour
{
    const float LifeTime = 15.0f;

    OwnerSide ownerSide = OwnerSide.Player;
    [SerializeField]
    Vector3 moveDirection = Vector3.zero;

    [SerializeField]
    float Speed = 0.0f;

    bool hited = false;

    float FireTime;

    bool NeedMove = false;
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

    public void Fire(OwnerSide FireOwner, Vector3 firePosition, Vector3 direction, float speed)
    {
        ownerSide = FireOwner;
        transform.position = firePosition;
        moveDirection = direction;
        Speed = speed;

        FireTime = Time.time;
        NeedMove = true;
    }

    Vector3 AdjustMove(Vector3 moveVector)
    {
        RaycastHit hitInfo;

        // 라인 캐스트는 시작점 끝점이 있는 선
        // 그 선에 뭐가 걸리는지 체크
        if(Physics.Linecast(transform.position,transform.position + moveVector, out hitInfo))
        {
            moveVector = hitInfo.point - transform.position;
            OnBulletCollision(hitInfo.collider);
        }
        return moveVector;
    }

    void OnBulletCollision(Collider collider)
    {
        if (hited)
            return;

        Collider myCollider = GetComponentInChildren<Collider>();
        myCollider.enabled = false;

        hited = true;
        NeedMove = false;

        if (ownerSide == OwnerSide.Player)
        {
            Enemy enemy = collider.GetComponentInParent<Enemy>();
        }
        else
        {
            Player player = collider.GetComponentInParent<Player>();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        OnBulletCollision(other);
    }

    bool ProcessDisappearCondition()
    {
        if(transform.position.x > 6.0f || transform.position.x < -6.0f 
            || transform.position.y > 6.0f || transform.position.y < -6.0f)
        {
            Disappear();
            return true;
        }
        else if(Time.time - FireTime > LifeTime)
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
