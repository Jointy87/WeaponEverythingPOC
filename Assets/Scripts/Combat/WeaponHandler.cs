using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Core;
using WeaponEverything.Movement;

namespace WeaponEverything.Combat
{
	public class WeaponHandler : MonoBehaviour
	{
		//Config parameters
		[SerializeField] WeaponsInfo weaponsInfo;
		[SerializeField] Rigidbody2D parentRigidbody;

		//Cache
		Animator animator;

		//States
		int startValue = 0;
		int condition = 0;
		WeaponType currentWeapon;
		WeaponAttackPoints attackPoints = null;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		private void Start() 			
		{
			SetCurrentWeapon(WeaponType.Unarmed);
		}

		private void OnEnable() 
		{
			GetComponentInParent<PlayerMover>().onAnimation += WeaponAnimations;
		}

		private void Update() 
		{
			print(currentWeapon);	
		}

		public void SwitchWeapons()
		{
			if(currentWeapon == WeaponType.Unarmed) return;

			if(currentWeapon == WeaponType.Spear) SetCurrentWeapon(WeaponType.Sword);
			else if (currentWeapon == WeaponType.Sword) SetCurrentWeapon(WeaponType.Spear);
		}

		public void SetCurrentWeapon(WeaponType weapon)
		{
			currentWeapon = weapon;
			Destroy(attackPoints);
			animator.runtimeAnimatorController = weaponsInfo.FetchWeaponAnimator(weapon);
			attackPoints = Instantiate(weaponsInfo.FetchAttackPoints(weapon), transform);
		}

		public void SetAnimationTrigger(string triggerString)
        {
            animator.SetTrigger(triggerString);
        }

        public void SetAnimatorFloat(string name, float value)
        {
            animator.SetFloat(name, value);
        }

		//Called from animator
		public void AttackHit(int value)
		{
			if(value == 0)
			{
				startValue = 0;
				condition = attackPoints.FetchAttackPoints().Length - 1;
			}
			else if (value == 1) //first unarmedhit
			{
				startValue = 0;
				condition = 3;
			}
			else if (value == 2) //second unarmedhit
			{
				startValue = 4;
				condition = 7;
			}

			bool hasHitEnemy = false;

			for (int pointIndex = startValue; pointIndex <= condition; pointIndex++)
			{
				Transform[] points = attackPoints.FetchAttackPoints();
				float[] pointRadius = attackPoints.FetchAttackPointRadius();

				Collider2D[] hitEnemies =
				Physics2D.OverlapCircleAll(points[pointIndex].position,
					pointRadius[pointIndex], weaponsInfo.FetchEnemyLayer(currentWeapon));

				foreach (Collider2D enemy in hitEnemies)
				{
					enemy.GetComponent<EnemyFighter>().Die();
					hasHitEnemy = true;
				}
			}
			if (!hasHitEnemy) return;
			FindObjectOfType<WeaponStashSystem>().RemoveFromStash();
		}

		private void WeaponAnimations(float move)
		{
			animator.SetFloat("horizontalSpeed", Mathf.Abs(move));
			animator.SetFloat("verticalSpeed", parentRigidbody.velocity.y);
		}

		public WeaponType FetchCurrentWeapon()
		{
			return currentWeapon;
		}
	}
}
