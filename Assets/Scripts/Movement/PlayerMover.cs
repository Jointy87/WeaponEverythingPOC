using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponEverything.Combat;

namespace WeaponEverything.Movement
{
	public class PlayerMover : MonoBehaviour
	{
		//Config parameters
		[SerializeField] float moveSpeed = 50f;
		[SerializeField] float jumpVelocity = 6f;
		[SerializeField] float fallMultiplier = 2.5f;
		[SerializeField] float lowJumpMultiplier = 2f;
		[Range(0, .3f)] [SerializeField] float movementSmoothing = .05f;
		[SerializeField] bool airControl = false;
		[SerializeField] LayerMask whatIsGround;
		[SerializeField] Transform groundCheck;

		//Cache
		Rigidbody2D rb;
		Animator animator;
		WeaponHandler weapon;

		//States
		const float groundedRadius = .2f;
		bool grounded;
		bool facingRight = true;
		Vector3 velocity = Vector3.zero;

		private void Awake()
		{
			rb = GetComponent<Rigidbody2D>();
			animator = GetComponent<Animator>();
			weapon = GetComponentInChildren<WeaponHandler>();
		}

		private void FixedUpdate()
		{
			CheckIfGrounded();
			AddGravityIfFalling();
		}

		private void CheckIfGrounded()
		{
			grounded = false;

			Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position,
						groundedRadius, whatIsGround);
			for (int i = 0; i < colliders.Length; i++)
			{
				if (colliders[i].gameObject != gameObject)
				{
					grounded = true;
				}
			}
		}
		private void AddGravityIfFalling()
		{
			if (rb.velocity.y < 0)
			{
				rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1);
			}
			else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
			{
				rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1);
			}
		}

		public void Move(float move, bool jump)
		{
			if (grounded || airControl)
			{
				// Move the character by finding the target velocity
				Vector3 targetVelocity = new Vector2(move, rb.velocity.y);
				// And then smoothing it out and applying it to the character
				rb.velocity = Vector3.SmoothDamp(rb.velocity,
					targetVelocity, ref velocity, movementSmoothing);

				animator.SetFloat("horizontalSpeed", Mathf.Abs(move));
				weapon.SetAnimatorFloat("horizontalSpeed", Mathf.Abs(move));
				animator.SetFloat("verticalSpeed", rb.velocity.y);
				weapon.SetAnimatorFloat("verticalSpeed", rb.velocity.y);

				if (move > 0 && !facingRight)
				{
					Flip();
				}
				else if (move < 0 && facingRight)
				{
					Flip();
				}
			}

			if (grounded && jump)
			{
				grounded = false;
				rb.velocity = Vector2.up * jumpVelocity;
			}
		}

		private void Flip()
		{
			facingRight = !facingRight;

			Vector3 theScale = transform.localScale;
			theScale.x *= -1;
			transform.localScale = theScale;
		}

		public float FetchMoveSpeed()
		{
			return moveSpeed;
		}

		public bool FetchGrounded()
		{
			return grounded;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object other)
		{
			return base.Equals(other);
		}

		public override string ToString()
		{
			return base.ToString();
		}
	}
}