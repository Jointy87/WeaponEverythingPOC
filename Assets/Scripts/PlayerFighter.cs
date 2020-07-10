using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFighter : MonoBehaviour
{
    //Config parameters
    [SerializeField] float dashSpeed = 750f;
    [SerializeField] float dashDuration = .5f;
    [SerializeField] Transform[] attackPoints;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] float[] attackPointRadius;

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
        if (GetComponent<PlayerMover>().FetchGrounded())
        {
            animator.SetTrigger("attack");
        }
    }

    // Called from animator
    public void AttackHit()
    {
        bool hasHitEnemy = false;

        foreach(Transform attackPoint in attackPoints)
        {
            Collider2D[] hitEnemies01 =
            Physics2D.OverlapCircleAll(attackPoints[0].position, attackPointRadius[0], enemyLayers);

            foreach (Collider2D enemy in hitEnemies01)
            {
                enemy.GetComponent<EnemyController>().Die();
                hasHitEnemy = true;
            }
        }

        if(!hasHitEnemy) return; 
        FindObjectOfType<WeaponStashSystem>().RemoveFromStash();
    }

    public void StartDash(float move)
    {
        StartCoroutine(Dash(move));
    }


    IEnumerator Dash(float move)
    {
        if (GetComponent<PlayerMover>().FetchGrounded())
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for(int pointIndex = 0; pointIndex <= attackPoints.Length - 1; pointIndex++)
        {
            Gizmos.DrawWireSphere(attackPoints[pointIndex].position, attackPointRadius[pointIndex]);
        }
    }
}
