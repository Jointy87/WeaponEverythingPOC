using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Core;

namespace WeaponEverything.Combat
{
	public class WeaponPickup : MonoBehaviour
	{
		//Config parameters
		[SerializeField] float chargeAmount;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == "Player")
			{
				FindObjectOfType<WeaponStashSystem>().AddToFill(chargeAmount);
				 
				Destroy(gameObject);
			}
		}
	}
}
