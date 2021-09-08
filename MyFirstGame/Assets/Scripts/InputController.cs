using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInput();
    }

    void UpdateInput()
    {
        Vector3 moveDir = Vector3.zero;

        if(Input.GetKey(KeyCode.UpArrow))
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

}
