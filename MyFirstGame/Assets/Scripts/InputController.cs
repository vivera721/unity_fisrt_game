using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    int DelayTime;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
        UpdateZKey();
    }

    void UpdateInput()
    {
        Vector3 moveDir = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDir.y = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDir.y = -1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDir.x = -1;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDir.x = 1;
        }

        SystemManager.Instance.Hero.ProcessInput(moveDir);
    }

    void UpdateZKey()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            // 총알 연속 발사 및 딜레이 주기
            DelayTime++;
            if(DelayTime % 50 == 1)
            {
                SystemManager.Instance.Hero.Fire();
            }
        }
    }

}
