﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WeaponEverything.Movement;

namespace WeaponEverything.Attributes
{
	public class EnemyHealth : MonoBehaviour
	{
		//Config parameters
		[SerializeField] float maxHealthPoints;
		[SerializeField] Canvas healthBarCanvas;
		[SerializeField] Image healthBar;
		[SerializeField] GameObject chargeDrop;

		//States
		float healthPoints;
		public bool isAlive {get; private set;} = true;

		private void Start() 
		{
			healthPoints = maxHealthPoints;
		}

		private void Update()
		{
			ScaleHealthBar();
		}

		private void ScaleHealthBar()
		{
			float fraction = healthPoints / maxHealthPoints;
			healthBar.transform.localScale =
				new Vector3(fraction, 1, 1);

			if (Mathf.Approximately(fraction, 0) || Mathf.Approximately(fraction, 1))
				healthBarCanvas.enabled = false;
			else healthBarCanvas.enabled = true;
		}

		public void SubstractHealth(float amount)
		{
			if(!isAlive) return;

			healthPoints = Mathf.Max(healthPoints - amount, 0);
			if(healthPoints == 0) Die();
		}

		public void Die() //can be private if we make projectiles do dmg instead of kill
		{
			GetComponent<Animator>().SetTrigger("die");
			GetComponent<CapsuleCollider2D>().enabled = false;
			GetComponent<EnemyMover>().rb.velocity = new Vector2(0, 0);
			isAlive = false;
			GetComponent<EnemyMover>().isAlive = false;

			GameObject spawnedPickup =
				Instantiate(chargeDrop, transform.position, Quaternion.identity);
		}
	}
}
