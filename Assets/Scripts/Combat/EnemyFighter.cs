using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Movement;

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

		// States
		bool isAlive = true;
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
		}

		private void Update()
		{
			if (!isAlive) return;

			timeSinceLastAttack += Time.deltaTime;

			CheckForPlayerRoll();

			if (isAttacking) return;

			CanEngage();	
		}

		private void CanEngage()
		{
			if (canEngage && player.GetComponent<PlayerFighter>().IsAlive())
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

		public void CanEngage(bool value)
		{
			canEngage = value;
		}

		public void Die()
		{
			animator.SetTrigger("die");
			GetComponent<CapsuleCollider2D>().enabled = false;
			mover.SetVelocity(0, 0);
			isAlive = false;

			GameObject spawnedPickup =
				Instantiate(weaponPickup, transform.position, Quaternion.identity);
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

		public bool IsAlive()
		{
			return isAlive;
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