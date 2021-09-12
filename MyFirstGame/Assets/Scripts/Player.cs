using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
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

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
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
            enemy.OnCrash(this);
    }

    public void OnCrash(Enemy enemy)
    {
        Debug.Log("oncrash enemy = " + enemy);
    }

    public void Fire()
    {

        GameObject leftgo = Instantiate(Bullet);
        GameObject rightgo = Instantiate(Bullet);
        Bullet leftbullet = leftgo.GetComponent<Bullet>();
        Bullet rightbullet = rightgo.GetComponent<Bullet>();
        leftbullet.Fire(OwnerSide.Player, FireTransformLeft.position, FireTransformLeft.up, BulletSpeed);
        rightbullet.Fire(OwnerSide.Player, FireTransformRight.position, FireTransformRight.up, BulletSpeed);

    }

}
