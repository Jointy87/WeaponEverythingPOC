using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    //Cache
    PlayerMover mover;
    PlayerFighter fighter;
    PlayerThrower thrower;
    
    //States
    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;
    bool haveControl = true;
    Vector2 aimDirection;

    private void Awake() 
    {
        mover = GetComponent<PlayerMover>();
        fighter = GetComponent<PlayerFighter>();
        thrower = GetComponent<PlayerThrower>();
    }

    void Update()
    {
        if (!haveControl || !fighter.IsAlive()) return;

        horizontalMove = Input.GetAxisRaw("Horizontal") * mover.FetchMoveSpeed() *
            Time.fixedDeltaTime;

        aimDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxis("Vertical"));

        if (IsThrowing())
        {
            thrower.Aim(aimDirection);

            if (Input.GetButtonDown("Fire1"))
            {
                thrower.Throw(aimDirection);
            }
        }
        else
        {
            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
            }

            mover.Move(horizontalMove, jump);
            jump = false;

            if (Input.GetButtonDown("Fire1"))
            {
                fighter.Attack();
            }

            if (Input.GetButtonDown("Fire2"))
            {
                fighter.StartDash(horizontalMove);
            }
        }
    }

    private bool IsThrowing()
    {
        if (Input.GetAxisRaw("Triggers") > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void HaveControl(bool value)
    {
        haveControl = value;
    }
}