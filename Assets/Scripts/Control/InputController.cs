using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Combat;
using WeaponEverything.Movement;

namespace WeaponEverything.Control
{
	public class InputController : MonoBehaviour
	{
		//Cache
		PlayerMover mover;
		PlayerFighter fighter;
		PlayerThrower thrower;
		WeaponHandler weaponHandler;

		//States
		float horizontalMove = 0f;
		bool jump = false;
		bool crouch = false;
		bool haveControl = true;
		Vector2 aimDirection;
		bool weaponsSwitched = false;

		private void Awake()
		{
			mover = GetComponent<PlayerMover>();
			fighter = GetComponent<PlayerFighter>();
			thrower = GetComponent<PlayerThrower>();
			weaponHandler = GetComponentInChildren<WeaponHandler>();
		}

		void Update()
		{
			if (!fighter.IsAlive()) return;

			horizontalMove = Input.GetAxisRaw("Horizontal") * mover.FetchMoveSpeed() *
				Time.fixedDeltaTime;

			if (mover.FetchGrounded() && IsThrowing())
			{
				aimDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

				thrower.Aim(aimDirection);

				if (Input.GetButtonDown("Fire1"))
				{
					thrower.Throw(aimDirection);
				}
			}
			else
			{
				thrower.CancelAim();

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
					fighter.StartRoll();
				}

				if(Input.GetAxisRaw("Pad Vertical") > .1)
				{
					if(weaponsSwitched) return;
					weaponHandler.SwitchWeapons();
					weaponsSwitched = true;
				} 
				else weaponsSwitched = false;
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
	}
}