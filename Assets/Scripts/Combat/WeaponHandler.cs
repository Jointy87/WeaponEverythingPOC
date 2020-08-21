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
		[SerializeField] ParticleSystem weaponBreakVFX = null;
		[SerializeField] float weaponBreakRadius = 2f;

		//Cache
		Animator animator;
		WeaponStashSystem stash;
		SpriteRenderer render;
		PlayerMover mover;

		//States
		int startValue = 0;
		int condition = 0;
		public WeaponType currentWeapon {get; private set;}
		WeaponAttackPoints attackInfo = null;
		public bool decayTimerActive {get; private set;} = false;
		public float decayTimer {get; set;} = 0;
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
			if (mover) mover.onAnimation += WeaponAnimations;
			if (stash) stash.onWeaponSwap += SwitchToWeaponByIndex;
			if (stash) stash.onRemoveCharge += EmitWeaponBreakAOE;
			if (stash) stash.onRemoveCharge += PlayWeaponBreakVFX;
		}

		private void Start() 			
		{
			SetCurrentWeapon(WeaponType.Unarmed);
		}

		private void Update()
		{
			HandleDecayTimerAndFlashing();
			CheckWeaponFlashState();
		}

		public void SwitchWeapons()
		{
			if (stash.chargesAmount == 0) return;

			if (currentWeapon == (WeaponType)Enum.GetValues(typeof(WeaponType)).Length - 1)
			{
				SetCurrentWeapon((WeaponType)1);
			}
			else SetCurrentWeapon((WeaponType)currentWeapon + 1);
		}

		private void SwitchToWeaponByIndex(int weaponTypeIndex)
		{
			SetCurrentWeapon((WeaponType)weaponTypeIndex);
		}

		public void SetCurrentWeapon(WeaponType weapon)
		{
			currentWeapon = weapon;
			Destroy(attackInfo);
			animator.runtimeAnimatorController = weaponsInfo.FetchWeaponAnimator(weapon);
			attackInfo = Instantiate(weaponsInfo.FetchAttackPoints(weapon), transform);

			if(!weaponsInfo.FetchWeaponMaterial(currentWeapon)) return;
			render.material = weaponsInfo.FetchWeaponMaterial(currentWeapon);
		}

		private void HandleDecayTimerAndFlashing()
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
				if (stash.chargesAmount != 0) stash.RemoveCharge();
			}
		}

		private void CheckWeaponFlashState()
		{
			if (flashed) render.material.SetFloat("_Float", 1);
			else render.material.SetFloat("_Float", 0);
		}

		private void EmitWeaponBreakAOE() //Used in delegate
		{
			Collider2D[] effectedEnemies = 
				Physics2D.OverlapCircleAll(transform.position, weaponBreakRadius, LayerMask.GetMask("Enemy"));
				print("Emitting");

			foreach (Collider2D enemy in effectedEnemies)
			{
				enemy.GetComponent<EnemyFighter>().Push();
			}
		}

		//Called from animator
		public void AttackHit()
		{
			if(hasHit) return;

			for (int pointIndex = 0; pointIndex <= attackInfo.FetchAttackPoints().Length - 1; pointIndex++)
			{
				Transform[] points = attackInfo.FetchAttackPoints();
				float[] pointRadius = attackInfo.FetchAttackPointRadius();

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
					
					if (decayTimerActive == false && stash.chargesAmount > 0 && currentWeapon != WeaponType.Unarmed)
					{
						decayTimerActive = true;
						decayTimer = decayTime;
					}
				}
			}
		}

		private void WeaponAnimations(float move) //Used in delegate
		{
			animator.SetFloat("horizontalSpeed", Mathf.Abs(move));
			animator.SetFloat("verticalSpeed", parentRigidbody.velocity.y);
		}

		public void StopAttack() //Called by animator
		{
			mover.GetComponent<Animator>().SetTrigger("stopAttacking");
			hasHit = false;
		}

		public void PlayWeaponBreakVFX() //Used in delegate
		{
			weaponBreakVFX.Play();
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.gray;
			Gizmos.DrawWireSphere(transform.position, weaponBreakRadius);
		}

		private void OnDisable()
		{
			if (mover) mover.onAnimation -= WeaponAnimations;
			if (stash) stash.onWeaponSwap -= SwitchToWeaponByIndex;
			if (stash) stash.onRemoveCharge -= EmitWeaponBreakAOE;
			if (stash) stash.onRemoveCharge -= PlayWeaponBreakVFX;
		}
	}
}
