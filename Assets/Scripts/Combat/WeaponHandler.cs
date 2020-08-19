using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Attributes;
using WeaponEverything.Core;
using WeaponEverything.Movement;

namespace WeaponEverything.Combat
{
	public class WeaponHandler : MonoBehaviour
	{
		//Config parameters
		[SerializeField] WeaponsInfo weaponsInfo = null;
		[SerializeField] Rigidbody2D parentRigidbody = null;
		[SerializeField] float decayTime = 5f;
		[SerializeField] float minFlashInterval = .1f, maxFlashInterval = 1f;

		//Cache
		Animator animator;
		WeaponStashSystem stash;
		SpriteRenderer render;
		PlayerMover mover;

		//States
		int startValue = 0;
		int condition = 0;
		WeaponType currentWeapon;
		WeaponAttackPoints attackPoints = null;
		bool decayTimerActive = false;
		float decayTimer = 0;
		bool flashed = false;
		bool hasHit = false;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			stash = FindObjectOfType<WeaponStashSystem>();
			render = GetComponentInChildren<SpriteRenderer>();
			mover = GetComponentInParent<PlayerMover>();
		}

		private void OnEnable()
		{
			if(mover != null) mover.onAnimation += WeaponAnimations;
		}

		private void Start() 			
		{
			SetCurrentWeapon(WeaponType.Unarmed);
		}

		private void Update()
		{
			UpdateDecayTimer();
			CheckWeaponFlashState();
		}

		public void SetCurrentWeapon(WeaponType weapon)
		{
			currentWeapon = weapon;
			Destroy(attackPoints);
			animator.runtimeAnimatorController = weaponsInfo.FetchWeaponAnimator(weapon);
			attackPoints = Instantiate(weaponsInfo.FetchAttackPoints(weapon), transform);

			if(!weaponsInfo.FetchWeaponMaterial(currentWeapon)) return;
			render.material = weaponsInfo.FetchWeaponMaterial(currentWeapon);
		}

		public void SwitchWeapons()
		{
			if (stash.FetchChargeAmount() == 0) return;

			if (currentWeapon == (WeaponType)Enum.GetValues(typeof(WeaponType)).Length - 1)
			{
				SetCurrentWeapon((WeaponType)0);
			}
			else SetCurrentWeapon((WeaponType)currentWeapon + 1);
		}

		//Called from animator
		public void AttackHit()
		{
			if(hasHit) return;

			for (int pointIndex = 0; pointIndex <= attackPoints.FetchAttackPoints().Length - 1; pointIndex++)
			{
				Transform[] points = attackPoints.FetchAttackPoints();
				float[] pointRadius = attackPoints.FetchAttackPointRadius();

				Collider2D[] hitEnemies =
				Physics2D.OverlapCircleAll(points[pointIndex].position,
					pointRadius[pointIndex], weaponsInfo.FetchEnemyLayer(currentWeapon));

				foreach (Collider2D enemy in hitEnemies)
				{	
					if(!hasHit)
					{
						enemy.GetComponent<EnemyHealth>().SubstractHealth(weaponsInfo.FetchWeaponDamagePerHit(currentWeapon));
						hasHit = true;
					}
					
					if (decayTimerActive == false && stash.FetchChargeAmount() > 0 && currentWeapon != WeaponType.Unarmed)
					{
						decayTimerActive = true;
						decayTimer = decayTime;
					}
				}
			}
		}

		private void UpdateDecayTimer()
		{
			if (!decayTimerActive) return;

			float interval = minFlashInterval + decayTimer / decayTime * (maxFlashInterval - minFlashInterval);
			decayTimer -= Time.deltaTime;
			flashed = Mathf.PingPong(Time.time, interval) > (interval / 2f);

			if (decayTimer <= 0)
			{
				decayTimer = 0;
				decayTimerActive = false;
				flashed = false;
				if (stash.FetchChargeAmount() != 0) stash.RemoveCharge();
				if (stash.FetchChargeAmount() == 0) SetCurrentWeapon(WeaponType.Unarmed);
			}
		}

		private void CheckWeaponFlashState()
		{
			if(flashed) render.material.SetFloat("_Float", 1);
			else render.material.SetFloat("_Float", 0);
		}

		public void SetAnimationTrigger(string triggerString)
        {
            animator.SetTrigger(triggerString);
        }

        public void SetAnimatorFloat(string name, float value)
        {
            animator.SetFloat(name, value);
        }

		private void WeaponAnimations(float move) //Used in delegate
		{
			animator.SetFloat("horizontalSpeed", Mathf.Abs(move));
			animator.SetFloat("verticalSpeed", parentRigidbody.velocity.y);
		}

		public WeaponType FetchCurrentWeapon()
		{
			return currentWeapon;
		}

		public void StopAttack() //Called by animator
		{
			mover.GetComponent<Animator>().SetTrigger("stopAttacking");
			hasHit = false;
		}

		private void OnDisable()
		{
			if (mover != null) mover.onAnimation -= WeaponAnimations;
		}
	}
}
