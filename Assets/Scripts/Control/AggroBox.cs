using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Combat;

namespace WeaponEverything.Control
{
	public class AggroBox : MonoBehaviour
	{
		//Config parameters
		[SerializeField] EnemyFighter enemy;

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.tag == "Player")
			{
				enemy.CanEngage(true);
			}
		}

		private void OnTriggerExit2D(Collider2D other)
		{
			if (other.tag == "Player")
			{
				enemy.CanEngage(false);
			}
		}
	}
}