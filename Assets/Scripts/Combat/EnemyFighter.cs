using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Core;
using WeaponEverything.Movement;
using WeaponEverything.Attributes;

namespace WeaponEverything.Combat
{
	public class EnemyFighter : MonoBehaviour
	{
		//Config parameters
		[SerializeField] float attackRange = .5f;
		[SerializeField] float attackInterval = 1f;
		[SerializeField] Transform[] attackPoints;
		[SerializeField] LayerMask playerLayer;
		[SerializeField] float[] attackPointRadius;
		[SerializeField] GameObject weaponPickup;

		//Cache
		Animator animator;
		Transform player;
		PlayerFighter playerFighter;
		EnemyMover mover;
		Collider2D myCollider;
		WeaponStashSystem stashSystem;

		// States
		bool canAttack = true;
		bool canEngage = false;
		float timeSinceLastAttack = Mathf.Infinity;
		Vector2 guardPosition;
		bool isAttacking = false;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			player = GameObject.FindGameObjectWithTag("Player").transform;
			guardPosition = transform.position;
			mover = GetComponent<EnemyMover>();
			playerFighter = player.GetComponent<PlayerFighter>();
			myCollider = GetComponent<Collider2D>();
			stashSystem = FindObjectOfType<WeaponStashSystem>();
		}

		private void Update()
		{
			if (!GetComponent<EnemyHealth>().FetchAlive()) return;

			timeSinceLastAttack += Time.deltaTime;

			CheckForPlayerRoll();

			if (isAttacking) return;

			CanEngage();	
		}

		private void CanEngage()
		{
			if (canEngage && stashSystem.IsAlive())
			{
				EngageTarget(player);
			}
			else
			{
				ReturnToGuardPosition();
			}
		}

		public void EngageTarget(Transform target)
		{
			float distanceToTarget = Vector2.Distance(transform.position, target.position);

			if (distanceToTarget <= attackRange)
			{
				if (timeSinceLastAttack >= attackInterval)
				{
					isAttacking = true;
					timeSinceLastAttack = 0;
					animator.SetTrigger("attack");
					mover.SetVelocity(0, 0);
				}
			}
			else
			{
				mover.MoveToTarget(target.position);
			}
		}

		private void ReturnToGuardPosition()
		{
			mover.MoveToTarget(guardPosition);
		}

		private void CheckForPlayerRoll()
		{
			if (playerFighter.IsRolling())
			{
				myCollider.enabled = false;
			}
			else
			{
				myCollider.enabled = true;
			}
		}

		public void SetCanEngage(bool value)
		{
			canEngage = value;
		}

		// Called from animator
		private void AttackHit()
		{
			for (int pointIndex = 0; pointIndex <= attackPoints.Length - 1; pointIndex++)
			{
				Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoints[pointIndex].position,
				attackPointRadius[pointIndex], playerLayer);

				if (!hitPlayer || playerFighter.IsRolling()) continue;

				hitPlayer.GetComponent<PlayerFighter>().GetHit(this.transform);
				return;
			}
		}

		// Called from animator
		private void IsAttacking()
		{
			isAttacking = false;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.gray;
			for (int pointIndex = 0; pointIndex <= attackPoints.Length - 1; pointIndex++)
			{
				Gizmos.DrawWireSphere(attackPoints[pointIndex].position, attackPointRadius[pointIndex]);
			}
		}
	}
}