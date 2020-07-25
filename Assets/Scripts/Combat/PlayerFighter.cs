using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Control;
using WeaponEverything.Core;
using WeaponEverything.Movement;

namespace WeaponEverything.Combat
{
	public class PlayerFighter : MonoBehaviour
	{
		//Config parameters
		[SerializeField] float dashSpeed = 750f;
		[SerializeField] float dashDuration = .5f;
		[SerializeField] float pushBackSpeed = 750f;
		[SerializeField] float pushBackDuration = .25f;
		[SerializeField] Transform[] attackPoints;
		[SerializeField] LayerMask enemyLayers;
		[SerializeField] float[] attackPointRadius;

		//Cache
		Animator animator;
		Rigidbody2D rb;
		new CapsuleCollider2D collider;
		WeaponStashSystem stash;
		WeaponHandler weapon;
		

		//States
		float dashTimer = 0;
		float pushBackTimer = 0;
		bool isAlive = true;
		Vector2 armOrigin;
		bool isRolling = false;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			rb = GetComponent<Rigidbody2D>();
			collider = GetComponent<CapsuleCollider2D>();
			stash = FindObjectOfType<WeaponStashSystem>();
			weapon = GetComponentInChildren<WeaponHandler>();
		}

		public void Attack()
		{
			animator.SetTrigger("attack");
			weapon.SetAnimationTrigger("attack");				
		}

		public void StartRoll()
		{
			StartCoroutine(Roll());
		}


		IEnumerator Roll()
		{
			if (GetComponent<PlayerMover>().FetchGrounded())
			{
				dashTimer = 0;
				float direction = transform.localScale.x;
				isRolling = true;

				animator.SetTrigger("roll");
				weapon.SetAnimationTrigger("roll");

				float currentY = transform.position.y;

				while (dashTimer < dashDuration)
				{
					rb.velocity = new Vector2(direction * dashSpeed * Time.deltaTime,
						rb.velocity.y);

					transform.position = new Vector2(transform.position.x, currentY);

					dashTimer += Time.deltaTime;

					yield return null;
				}
				isRolling = false;
			}
		}

		public void GetHit(Transform enemyPos)
		{
			if (stash.FetchStash() > 0)
			{
				stash.RemoveFromStash();
				StartCoroutine(PushBack(enemyPos));
			}
			else
			{
				Die();
				weapon.SetAnimationTrigger("die");
			}
		}

		IEnumerator PushBack(Transform enemyPos)
		{
			float direction = 1;
			if (transform.position.x < enemyPos.position.x)
			{
				direction = -1;
			}

			pushBackTimer = 0;
			animator.SetTrigger("getHit");
			weapon.SetAnimationTrigger("getHit");
			float currentY = transform.position.y;

			while (pushBackTimer < pushBackDuration)
			{

				rb.velocity = new Vector2(direction * pushBackSpeed * Time.deltaTime,
					rb.velocity.y);

				transform.position = new Vector2(transform.position.x, currentY);

				pushBackTimer += Time.deltaTime;

				yield return null;
			}

			animator.SetTrigger("getHit");
		}

		public void Die()
		{
			animator.SetTrigger("die");
			GetComponent<InputController>().HaveControl(false);
			rb.velocity = new Vector2(0, 0);
			isAlive = false;
		}

		public bool IsAlive()
		{
			return isAlive;
		}

		public bool IsRolling()
		{
			return isRolling;
		}
	}
}