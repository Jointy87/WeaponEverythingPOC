using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Core;

namespace WeaponEverything.Combat
{
	public class Projectile : MonoBehaviour
	{
		//Cache
		Rigidbody2D rb;

		private void Awake()
		{
			rb = GetComponent<Rigidbody2D>();
		}
		private void Start()
		{
			Destroy(gameObject, 10f);
		}

		private void OnTriggerEnter2D(Collider2D other)
		{
			if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
			{
				other.GetComponent<EnemyFighter>().Die();
				FindObjectOfType<WeaponStashSystem>().RemoveFromStash();
				Destroy(gameObject);
			}
			else if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
			{
				rb.velocity = new Vector2(0, 0);
				GetComponent<BoxCollider2D>().enabled = true;
			}
		}
	}
}