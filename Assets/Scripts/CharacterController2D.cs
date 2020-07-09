using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 50f;                           // Movement speed when player moves horizontally.
    [SerializeField] private float dashSpeed = 750f;
    [SerializeField] private float jumpVelocity = 20f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float dashDuration = 10f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] private float attackPointRadius = 0.5f;
    [Range(0, 1)] [SerializeField] private float crouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool airControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask whatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform groundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform ceilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D torsoCollider;                // A collider that will be disabled when crouching
    [SerializeField] private Collider2D feetCollider;

    const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool grounded;            // Whether or not the player is grounded.
    const float ceilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D rb;
    private Animator animator;
    private bool facingRight = true;  // For determining which way the player is currently facing.
    private Vector3 velocity = Vector3.zero;
    private bool isDashing = false;
    private float dashTimer = 0;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool wasCrouching = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = grounded;
        grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, 
            groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if (!wasGrounded)
                    OnLandEvent.Invoke();
            }
        }

        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1);
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1);
        }
    }


    public void Move(float move, bool crouch, bool jump)
    {
        //only control the player if grounded or airControl is turned on
        if (grounded || airControl)
        {
            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move, rb.velocity.y);
            // And then smoothing it out and applying it to the character
            rb.velocity = Vector3.SmoothDamp(rb.velocity, 
                targetVelocity, ref velocity, movementSmoothing);
            animator.SetFloat("horizontalSpeed", Mathf.Abs(move));
            animator.SetFloat("verticalSpeed", rb.velocity.y);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !facingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && facingRight)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (grounded && jump)
        {
            // Add a vertical force to the player.
            grounded = false;
            rb.velocity = Vector2.up * jumpVelocity;
        }
    }

    public void Attack()
    {
        if(grounded)
        {
            animator.SetTrigger("attack");
        }
    }

    public void AttackHit()
    {
        Collider2D[] hitEnemies = 
        Physics2D.OverlapCircleAll(attackPoint.position, attackPointRadius, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyController>().Die();
        }
    }

    public void StartDash(float move)
    {
        StartCoroutine(Dash(move));       
    }

    
    IEnumerator Dash(float move)
    {
        if(grounded)
        {
            dashTimer = 0;
            float direction = transform.localScale.x;

            animator.SetBool("isDashing", true);
            torsoCollider.enabled = false;
            feetCollider.enabled = false;

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
            torsoCollider.enabled = true;
            feetCollider.enabled = true;
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public float FetchMoveSpeed()
    {
        return moveSpeed;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(attackPoint.position, attackPointRadius);
    }
}