using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    public CharacterController2D controller;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool haveControl = true;

    // Update is called once per frame
    void Update()
    {
        if(!haveControl) return;
        
        horizontalMove = Input.GetAxisRaw("Horizontal") * controller.FetchMoveSpeed() * 
            Time.fixedDeltaTime;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    void FixedUpdate()
    {
        if(!haveControl) return;
        
        // Move our character
        controller.Move(horizontalMove, crouch, jump);
        jump = false;

        //Attack
        if(Input.GetButtonDown("Fire1"))
        {
            controller.Attack();
        }

        //Dash
        if(Input.GetButtonDown("Fire2"))
        {
            controller.StartDash(horizontalMove);
        }
    }

    public void HaveControl(bool value)
    {
        haveControl = value;
    }
}