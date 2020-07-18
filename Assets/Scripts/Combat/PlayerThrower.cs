using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

		//States
		Vector3 originVector;
		Vector2 aimDirection;
		Vector3 aimVector;
		float aimAngle;

		private void Awake()
		{
			lr = GetComponentInChildren<LineRenderer>();
		}

		public void Aim(Vector2 aimDirection)
		{
			GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
			GetComponent<Animator>().SetFloat("horizontalSpeed", 0);
			GetComponent<Animator>().SetFloat("verticalSpeed", 0);

			originVector =
				new Vector3(throwOrigin.position.x, throwOrigin.position.y, throwOrigin.position.z);

			lr.enabled = true;
			aimVector = new Vector3(aimDirection.x, aimDirection.y, 0);

			lr.SetPosition(0, originVector);
			lr.SetPosition(1, originVector + aimVector * renderLineLength);
		}

		public void Throw(Vector2 aimDirection)
		{
			float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

			GameObject projectile = Instantiate(weaponToThrow,
				originVector, Quaternion.AngleAxis(aimAngle, Vector3.forward));
			projectile.GetComponent<Rigidbody2D>().velocity = aimDirection * throwSpeed * Time.deltaTime;
			
			// TO DO: Ensure that speed of projectile is always the same, now matter how far thumbstick is pushed
		}

		public void CancelAim()
		{
			lr.enabled = false;
		}
	}
}