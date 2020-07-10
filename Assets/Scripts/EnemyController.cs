using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	// Config parameters
	[SerializeField] float moveSpeed = 10f;
	[SerializeField] float attackRange = .5f;
	[SerializeField] float attackInterval = 1f;
	[SerializeField] Transform[] attackPoints;
	[SerializeField] LayerMask playerLayer;
	[SerializeField] float[] attackPointRadius;
	[SerializeField] GameObject weaponPickup;

	// Cache
	Animator animator;
	Rigidbody2D rb;
	Transform player;

	// States
	bool isAlive = true;
	bool canAttack = true;
	bool canEngage = false;
	float timeSinceLastAttack = Mathf.Infinity;
	Vector2 guardPosition;
	bool facingRight = true;
	bool isAttacking = false;

	private void Awake() 
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody2D>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		guardPosition = transform.position;
	}

	private void Update()
    {
        if (!isAlive) return;

        animator.SetFloat("horizontalSpeed", Mathf.Abs(rb.velocity.x));
        timeSinceLastAttack += Time.deltaTime;

        if (isAttacking) return;

        CanEngage();

        CheckFacingDirection();
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

		if(distanceToTarget <= attackRange)
		{
			if(timeSinceLastAttack >= attackInterval)
			{   
				isAttacking = true;
				timeSinceLastAttack = 0;
				animator.SetTrigger("attack");
				rb.velocity = new Vector2(0, 0);	
			}
		}
		else
		{
			MoveToTarget(target);
		}   
	}

	private void MoveToTarget(Transform target)
	{
		if(transform.position.x < target.position.x)
		{
			rb.velocity = new Vector2(moveSpeed,0);
		}
		else
		{
			rb.velocity = new Vector2(-moveSpeed, 0);
		}
	}

	private void ReturnToGuardPosition()
	{
		float distanceToTarget = Vector2.Distance(transform.position, guardPosition);

		if (transform.position.x < guardPosition.x)
		{
			rb.velocity = new Vector2(moveSpeed, 0);
		}
		else if (transform.position.x > guardPosition.x)
		{
			rb.velocity = new Vector2(-moveSpeed, 0);
		}
		else
		{
			rb.velocity = new Vector2 (0,0);
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

	public void CanEngage(bool value)
	{
		canEngage = value;
	}

	public void Die()
	{
		animator.SetTrigger("die");
		GetComponent<CapsuleCollider2D>().enabled = false;
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

			if(!hitPlayer) return;
				//Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
			    hitPlayer.GetComponent<PlayerFighter>().GetHit(this.transform);					
		}
	}

	// Called from animator
	private void IsAttacking()
	{
		isAttacking = false;
	}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for (int pointIndex = 0; pointIndex <= attackPoints.Length - 1; pointIndex++)
        {
            Gizmos.DrawWireSphere(attackPoints[pointIndex].position, attackPointRadius[pointIndex]);
        }
    }
}
