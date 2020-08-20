using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Core;

namespace WeaponEverything.Combat
{
	public class PlayerThrower : MonoBehaviour
	{
		//Config parameters
		[SerializeField] GameObject weaponToThrow;
		[SerializeField] float throwSpeed;
		[SerializeField] Transform throwOrigin;
		[SerializeField] float renderLineLength;

		//Cache
		LineRenderer lr;
		WeaponHandler weapon;
		WeaponStashSystem stash;

		//States
		Vector3 originVector;
		Vector2 aimDirection;
		Vector3 aimVector;
		float aimAngle;

		private void Awake()
		{
			lr = GetComponentInChildren<LineRenderer>();
			weapon = GetComponentInChildren<WeaponHandler>();
			stash = FindObjectOfType<WeaponStashSystem>();
		}

		public void Aim(Vector2 aimDirection)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			GetComponent<Animator>().SetFloat("horizontalSpeed", 0);
			GetComponent<Animator>().SetFloat("verticalSpeed", 0);

			originVector =
				new Vector3(throwOrigin.position.x, throwOrigin.position.y, throwOrigin.position.z);

			lr.enabled = true;

			if (stash.FetchChargeAmount() == 0) lr.material.color = Color.red;
			else lr.material.color = Color.white;

			aimVector = new Vector3(aimDirection.x, aimDirection.y, 0);

			lr.SetPosition(0, originVector);
			lr.SetPosition(1, originVector + aimVector * renderLineLength);
		}

		public void Throw(Vector2 aimDirection)
		{
			if(weapon.FetchCurrentWeapon() == WeaponType.Unarmed || stash.FetchChargeAmount() == 0) return;

			float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

			GameObject projectile = Instantiate(weaponToThrow,
				originVector, Quaternion.AngleAxis(aimAngle, Vector3.forward));
			projectile.GetComponent<Rigidbody2D>().velocity = aimDirection * throwSpeed * Time.deltaTime;

			stash.RemoveCharge();
			
			// TO DO: Ensure that speed of projectile is always the same, now matter how far thumbstick is pushed
		}

		public void CancelAim()
		{
			lr.enabled = false;
		}
	}
}