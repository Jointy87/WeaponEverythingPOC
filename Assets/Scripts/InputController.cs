using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    public CharacterController2D controller;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    // Update is called once per frame
    void Update()
    {

        horizontalMove = Input.GetAxisRaw("Horizontal") * controller.FetchMoveSpeed() * Time.fixedDeltaTime;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("Crouch"))
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

    }

    void FixedUpdate()
    {
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
            controller.Dash(horizontalMove);
        }
    }
}