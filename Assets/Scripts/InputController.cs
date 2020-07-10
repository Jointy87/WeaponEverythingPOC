using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    //Cache
    PlayerMover mover;
    PlayerFighter fighter;
    
    //States
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool haveControl = true;

    private void Awake() 
    {
        mover = GetComponent<PlayerMover>();
        fighter = GetComponent<PlayerFighter>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!haveControl) return;
        
        horizontalMove = Input.GetAxisRaw("Horizontal") * mover.FetchMoveSpeed() * 
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
        mover.Move(horizontalMove, crouch, jump);
        jump = false;

        //Attack
        if(Input.GetButtonDown("Fire1"))
        {
            fighter.Attack();
        }

        //Dash
        if(Input.GetButtonDown("Fire2"))
        {
            fighter.StartDash(horizontalMove);
        }
    }

    public void HaveControl(bool value)
    {
        haveControl = value;
    }
}