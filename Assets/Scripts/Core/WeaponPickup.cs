using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Combat;

namespace WeaponEverything.Core
{
	public class WeaponPickup : MonoBehaviour
	{
		//Config parameters
		[SerializeField] WeaponType weaponPickupType;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == "Player")
			{
				FindObjectOfType<WeaponStashSystem>().AddToStash();

				WeaponHandler handler = other.gameObject.GetComponentInChildren<WeaponHandler>();
				
				if(handler.FetchCurrentWeapon() == WeaponType.Unarmed)
				{
					handler.SetCurrentWeapon(weaponPickupType);
				}
				 
				Destroy(gameObject);
			}
		}
	}
}
