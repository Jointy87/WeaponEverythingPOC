using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WeaponEverything.Movement
{
	public class EnemyMover : MonoBehaviour
	{
		// Config parameters
		[SerializeField] float moveSpeed = 10f;

		//States
		bool facingRight = true;
		bool isAlive = true;

		// Cache
		Animator animator;
		Rigidbody2D rb;

		private void Awake()
		{
			animator = GetComponent<Animator>();
			rb = GetComponent<Rigidbody2D>();
		}

		private void Update()
		{
			if (isAlive) return;

			animator.SetFloat("horizontalSpeed", Mathf.Abs(rb.velocity.x));

			CheckFacingDirection();
		}

		public void MoveToTarget(Vector2 target)
		{
			if (transform.position.x < target.x)
			{
				rb.velocity = new Vector2(moveSpeed, 0);
			}
			else if (transform.position.x > target.x)
			{
				rb.velocity = new Vector2(-moveSpeed, 0);
			}
			else
			{
				rb.velocity = new Vector2(0, 0);
			}
		}

		private void CheckFacingDirection()
		{
			if (rb.velocity.x > 0 && !facingRight)
			{
				Flip();
			}
			else if (rb.velocity.x < 0 && facingRight)
			{
				Flip();
			}
		}

		private void Flip()
		{
			facingRight = !facingRight;

			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}

		public void SetVelocity(float xVelocity, float yVelocity)
		{
			rb.velocity = new Vector2(xVelocity, yVelocity);
		}

		public void SetIsAlive(bool value)
		{
			isAlive = value;
		}
	}
}