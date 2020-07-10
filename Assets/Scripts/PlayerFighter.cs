using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    bool isAlive = true;

    //States
    float dashTimer = 0;
    float pushBackTimer = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        collider = GetComponent<CapsuleCollider2D>();
        stash = FindObjectOfType<WeaponStashSystem>();
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

        for(int pointIndex = 0; pointIndex <= attackPoints.Length - 1; pointIndex++)
        {
            Collider2D[] hitEnemies =
            Physics2D.OverlapCircleAll(attackPoints[pointIndex].position, 
                attackPointRadius[pointIndex], enemyLayers);

            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<EnemyController>().Die();
                hasHitEnemy = true;
            }
        }

        if(!hasHitEnemy) return; 
        stash.RemoveFromStash();
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

    public void GetHit(Transform enemyPos)
    {
        if(stash.FetchStash() > 0)
        {
            stash.RemoveFromStash();
            StartCoroutine(PushBack(enemyPos));
        }
        else
        {
            Die();
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
        isAlive = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        for(int pointIndex = 0; pointIndex <= attackPoints.Length - 1; pointIndex++)
        {
            Gizmos.DrawWireSphere(attackPoints[pointIndex].position, attackPointRadius[pointIndex]);
        }
    }

    public bool IsAlive()
    {
        return isAlive;
    }
}
