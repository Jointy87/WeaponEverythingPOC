using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	private void Start() 
	{
		Destroy(gameObject, 10f);
	}
	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			other.GetComponent<EnemyController>().Die();
			FindObjectOfType<WeaponStashSystem>().RemoveFromStash();
			Destroy(gameObject);
		}
	}


}
