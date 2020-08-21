using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Core;
using WeaponEverything.Movement;
using WeaponEverything.Attributes;
using System;

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

		//Cache
		Animator animator;
		Transform player;
		PlayerFighter playerFighter;
		EnemyMover mover;
		Collider2D myCollider;
		WeaponStashSystem stashSystem;

		// States
		bool canAttack = true;
		public bool canEngage {get; set;} = false;
		float timeSinceLastAttack = Mathf.Infinity;
		Vector2 guardPosition;
		bool isAttacking = false;
		bool isPushed = false;

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
			if (!GetComponent<EnemyHealth>().isAlive) return;

			timeSinceLastAttack += Time.deltaTime;

			CheckForPlayerRoll();

			CanEngage();	
		}

		private void CanEngage()
		{
			if (isAttacking || isPushed) return;

			if (canEngage && stashSystem.isAlive)
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
					CheckAttackDirection(target);
					isAttacking = true;
					timeSinceLastAttack = 0;
					animator.SetTrigger("attack");
					mover.rb.velocity = new Vector2(0, 0);
				}
			}
			else
			{
				mover.MoveToTarget(target.position);
			}
		}

		private void CheckAttackDirection(Transform target)
		{
			if(target.position.x > transform.position.x && !mover.facingRight) mover.Flip();
			else if(target.position.x < transform.position.x && mover.facingRight) mover.Flip();
		}

		private void ReturnToGuardPosition()
		{
			mover.MoveToTarget(guardPosition);
		}

		private void CheckForPlayerRoll()
		{
			if (playerFighter.isRolling)
			{
				myCollider.enabled = false;
			}
			else
			{
				myCollider.enabled = true;
			}
		}

		public void Push()
		{
			isAttacking = false;
			isPushed = true;
			animator.SetTrigger("push");
		}

		private void AttackHit() // Called from animator
		{
			for (int pointIndex = 0; pointIndex <= attackPoints.Length - 1; pointIndex++)
			{
				Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoints[pointIndex].position,
				attackPointRadius[pointIndex], playerLayer);

				if (!hitPlayer || playerFighter.isRolling) continue;

				hitPlayer.GetComponent<PlayerFighter>().GetHit(this.transform);
				return;
			}
		}

		private void IsAttackingFalse() // Called from animator
		{
			isAttacking = false;
		}

		private void IsPushedFalse() //Called from animator
		{
			isPushed = false;
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