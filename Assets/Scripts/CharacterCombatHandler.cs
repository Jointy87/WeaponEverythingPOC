using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCombatHandler : MonoBehaviour
{
	//Config parameters
	[SerializeField] float dashSpeed = 750f;
	[SerializeField] float dashDuration = .5f;
	[SerializeField] Transform attackPoint;
	[SerializeField] LayerMask enemyLayers;
	[SerializeField] float attackPointRadius = 0.5f;

    //Cache
    Animator animator;
    Rigidbody2D rb;
    new CapsuleCollider2D collider;

    //States
    bool isDashing = false;
    float dashTimer = 0;

	private void Awake()
	{
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
	}

    public void Attack()
    {
        if (GetComponent<CharacterMover>().FetchGrounded())
        {
            animator.SetTrigger("attack");
        }
    }

    // Called from animator
    public void AttackHit()
    {
        Collider2D[] hitEnemies =
        Physics2D.OverlapCircleAll(attackPoint.position, attackPointRadius, enemyLayers);

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyController>().Die();
        }

        FindObjectOfType<WeaponStashSystem>().RemoveFromStash();
    }

    public void StartDash(float move)
    {
        StartCoroutine(Dash(move));
    }


    IEnumerator Dash(float move)
    {
        if (GetComponent<CharacterMover>().FetchGrounded())
        {
            dashTimer = 0;
            float direction = transform.localScale.x;

            animator.SetBool("isDashing", true);
            collider.enabled = false;

            float currentY = transform.position.y;

            while (dashTimer < dashDuration)
            {
                rb.velocity = new Vector2(direction * dashSpeed * Time.deltaTime,
                    rb.velocity.y);

                transform.position = new Vector2(transform.position.x, currentY);

                dashTimer += Time.deltaTime;

                yield return null;
            }

            animator.SetBool("isDashing", false);
            collider.enabled = true;
        }
    }
}
