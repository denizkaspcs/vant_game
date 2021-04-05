using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestMovementScript : MonoBehaviour
{
    //var
    public float maxCappedSpeed = 5f;
    private bool checkJump = true;
    const float groundedRadius = .137f;
    private bool facingRight = true;
    float groundedRemember = 0;
    float groundedRememberTime = 0.2f;
    float jumpRemember = 0;
    float jumpRememberTime = .2f;
    private Vector3 m_velocity = Vector3.zero;
    private Rigidbody2D rigidbody2D;
    private bool grounded;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform groundCheck2;

    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private bool airControl = true;
    [SerializeField] private float jumpForce = 400f;
    public float afterJumpHeight = 10f;


    public Animator animator;
    float horizontalMove = 0f;
    public float runSpeed = 40f;
    bool jump = false;
    float dir;
    bool canMove = false;
    bool firstPressCheck = true;
    /*
        //Event Example
        [Header("Events")]
        [Space]

        public UnityEvent OnLandEvent;
    */

    //const
    public void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        if (mousePosition.x - transform.position.x < 0)
        {
            dir = 1;
        }
        else
        {
            dir = -1;
        }
        /*
        if (Input.GetAxisRaw("Horizontal") > 0)
        {
            
            dir = -1;
        }
        else if (Input.GetAxisRaw("Horizontal") < 0)
        {
            dir = 1;
        }
        */
        if (Input.GetMouseButtonDown(0))
        {
            horizontalMove = runSpeed;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            horizontalMove = 0f;
        }
        //horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        /*
            if (Input.GetButtonDown("Fans"))
            {
                canMove = 1;
                animator.SetFloat("Speed", horizontalMove);

            }
            else if (Input.GetButtonUp("Fans"))
            {
                canMove = 0;
                animator.SetFloat("Speed", 0);
            }
            //animator.SetFloat("Speed",Mathf.Abs(horizontalMove));
        */
        if (Input.GetMouseButtonDown(1))
        {
            jump = true;
            jumpRemember = jumpRememberTime;
        }
    }
    private void FixedUpdate()
    {
        Move(horizontalMove * Time.fixedDeltaTime, jump, dir);
        DampenForceForVelocity();
        jump = false;
        bool wasGrounded = grounded;
        grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            { // everything we collide with except for our player which has this script
                grounded = true;
                if (wasGrounded)
                {   //If wasn't grounded but now is
                    //OnLandEvent.Invoke();
                    canMove = false;
                    firstPressCheck = true;
                    OnLanding();
                }

                if (checkJump) { groundedRemember = groundedRememberTime; }
            }
        }
        Collider2D[] colliders2 = Physics2D.OverlapCircleAll(groundCheck2.position, groundedRadius, whatIsGround);
        for (int i = 0; i < colliders2.Length; i++)
        {
            if (colliders2[i].gameObject != gameObject)
            { // everything we collide with except for our player which has this script
                grounded = true;
                if (wasGrounded)
                {   //If wasn't grounded but now is
                    //OnLandEvent.Invoke();
                    canMove = false;
                    firstPressCheck = true;
                    OnLanding();
                }

                if (checkJump) { groundedRemember = groundedRememberTime; }
            }
        }
    }
    //methods
    public void Move(float move, bool jump, float dir)
    {
        if (grounded || airControl)
        {
            //Vector3 targetVelocity = new Vector2(move * 10f, rigidbody2D.velocity.y);
            //rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref m_velocity, movementSmoothing);
            //float controlMove = DampenForceForVelocity(move);
            float canMoveFloat;
            if (canMove) { canMoveFloat = 1f; } else { canMoveFloat = 0f; }
            float generalMove = move * canMoveFloat * dir;
            Vector2 movement = new Vector2(generalMove * 100f, 0f);
            rigidbody2D.AddForce(movement * Time.deltaTime); // <------------
            if (generalMove != 0 && firstPressCheck)
            {
                Vector2 vect;
                vect.x = rigidbody2D.velocity.x;
                vect.y = afterJumpHeight;
                firstPressCheck = false;
                rigidbody2D.velocity = vect;
            }

            if (dir > 0 && !facingRight)
            {
                Flip();
            }
            else if (dir < 0 && facingRight)
            {
                Flip();
            }
        }
        groundedRemember -= Time.deltaTime;
        jumpRemember -= Time.deltaTime;

        if (groundedRemember > 0 && jumpRemember > 0)
        {
            Debug.Log("hello");
            //add a vertical force
            canMove = true;
            checkJump = false;
            groundedRemember = 0;
            jumpRemember = 0;
            grounded = false;
            rigidbody2D.AddForce(new Vector2(0f, jumpForce));
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

    private void OnLanding()
    {
        checkJump = true;
        airControl = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spike"))
        {
            airControl = false;
            Vector2 vect = new Vector2(0, 0);
            rigidbody2D.velocity = vect;
        }
    }

    private void DampenForceForVelocity()
    {
        //velocity = v0 + a * t ;
        //force = m(1) * a;
        //add force = (move* 10 * deltaTime)
        //i want top speed to be 500;
        //topSpeed = 500 = 0 + a*t
        float cappedVelocity = Mathf.Min(Mathf.Abs(rigidbody2D.velocity.x), maxCappedSpeed) * Mathf.Sign(rigidbody2D.velocity.x);
        rigidbody2D.velocity = new Vector2(cappedVelocity, rigidbody2D.velocity.y);
    }
    //mutator methods
}
