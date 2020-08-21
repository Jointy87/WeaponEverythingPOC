using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

		//Cache
		Animator animator;
		Rigidbody2D rb;
		WeaponStashSystem stash;
		WeaponHandler weapon;
		
		//States
		float dashTimer = 0;
		float pushBackTimer = 0;
		public bool isRolling {get; private set;} = false;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			rb = GetComponent<Rigidbody2D>();
			stash = FindObjectOfType<WeaponStashSystem>();
			weapon = GetComponentInChildren<WeaponHandler>();
		}

		private void OnEnable() 
		{
			stash.onPlayerDeath += Die;
		}

		public void Attack()
		{
			isRolling = false;
			animator.SetTrigger("attack");
			weapon.GetComponent<Animator>().SetTrigger("attack");	
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
				weapon.GetComponentInChildren<SpriteRenderer>().enabled = false;

				float currentY = transform.position.y;

				while (dashTimer < dashDuration && isRolling == true)
				{
					rb.velocity = new Vector2(direction * dashSpeed * Time.deltaTime,
						rb.velocity.y);

					dashTimer += Time.deltaTime;

					yield return null;
				}
			}
		}

		public void GetHit(Transform enemyPos)
		{
			if (stash.chargesAmount > 0)
			{
				StartCoroutine(PushBack(enemyPos));
			}
			else if (stash.isAlive)
			{
				stash.isAlive = false;
				Die();
			} 
		}

		IEnumerator PushBack(Transform enemyPos)
		{
			float direction = 1;
			if (transform.position.x < enemyPos.position.x) direction = -1;

			pushBackTimer = 0;
			animator.SetTrigger("getHit");
			weapon.GetComponent<Animator>().SetTrigger("getHit");
			float currentY = transform.position.y;

			if (weapon.decayTimerActive) weapon.decayTimer = 0;
			else stash.RemoveCharge("fighter");

			while (pushBackTimer < pushBackDuration)
			{
				rb.velocity = new Vector2(direction * pushBackSpeed * Time.deltaTime,
					rb.velocity.y);

				transform.position = new Vector2(transform.position.x, currentY);

				pushBackTimer += Time.deltaTime;

				yield return null;
			}
		}

		private void Die()
		{
			animator.SetTrigger("die");
			weapon.GetComponentInChildren<SpriteRenderer>().enabled = false;
			rb.velocity = new Vector2(0, 0);
		}

		public void StopRolling() //Called from animator
		{
			isRolling = false;
			weapon.GetComponentInChildren<SpriteRenderer>().enabled = true;
		}

		private void OnDisable()
		{
			stash.onPlayerDeath += Die;
		}
	}
}