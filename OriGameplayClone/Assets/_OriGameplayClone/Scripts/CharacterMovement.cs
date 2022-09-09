using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 1.0f;
    public float midairAcceleration = 10.0f; //In mid-air, if we change direction, we need to accelerate from 0 to a certain speed.

    [Header("Dash")]
    public float dashSpeed = 1000.0f;
    public float dashDuration = 1.0f;
    public float dashCooldown = 3.0f;

    [Header("Jump")]
    public float jumpSpeed = 1.0f;
    public float jumpInputTime = 1.0f; //For the first jump, if we hold they key, we can jump higher. This variable defined how long we can keep pressing

    [Header("Wall Jump")]
    public float wallJumpSpeed = 100.0f;
    public float wallJumpDeacc = 1.0f;

    [Header("Dodge redirect")]
    public float dodgeRedirectForce = 100.0f;
    public float dodgeRedirectSlowDown = 1.0f;

    [Header("Bash")]
    public float bashSpeed = 1000.0f;
    public float bashChargeTime = 2.0f;
    public float bashDeacc = 1.0f;
    public ParticleSystem bashParticles;

    [Header("Gravity")]
    public float gravity = 15.0f; //The default gravity didn't achieve proper results, so I defined in fixed update a new gravity
    public float hoverGravityLimiter = 0.3f;

    [Header("Lanterns")]
    public float lanternRange;
    public LayerMask lanternLayer;
    public RectTransform arrowSprite;

    private bool isGrounded = true;     //Did we hit a ground collider?
    private bool canWallJump = false;  //Did we hit a climbable wall collider?
    private bool canJump = true;        //Set to true when lifting jump key & false when actually jumping
    private bool canDoubleJump = true;  //Reset when we hit the ground
    private bool holdingSpace = false;  //Used to create the effect of holding jump key to jump higher
    private bool holdingShift = false;
    private bool doDash = false;
    private bool canDash = true;
    private bool bashChargeStarted = false;
    private float bashPower = 0.0f;
    private bool playedBashParticles = false;

    private float wallDirection;            //What direction is the climbable wall that we hit in?
    private float jumpTimeCounter = 0.0f;   //Time until we will detect jump hold
    private float horizontalInput = 0.0f;
    private float previousMoveDir = 1.0f;   //Used to detect change of direction in mid-air
    private float accelerationFactor = 1.0f;//Goes from 0 to 1 when we change direction in mid-air
    private float wallJumpPropulsion = 0.0f;//Is used to propell us in an opposite direction to the wall when doing wall jump.
    private float dashStartTime = -100.0f;
    private float dashDirection = 0.0f;
    private Vector3 dodgeRedirectVelocity = Vector3.zero;

    private float bashChargeStartTime = 0.0f;

    private Rigidbody rb;

    private Transform lanternPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Detect all input in update
        horizontalInput = Input.GetAxis("Horizontal");

        holdingShift = Input.GetKey(KeyCode.LeftShift);
        holdingSpace = Input.GetKey(KeyCode.Space);
        if (Input.GetKeyUp(KeyCode.Space))
        {
            canJump = true;
            jumpTimeCounter = Time.time;
        }
        if(Input.GetKeyDown(KeyCode.LeftControl) && canDash && Time.time - dashStartTime > dashCooldown)
        {
            doDash = true;
            canDash = false;
            dashStartTime = Time.time;
            dashDirection = horizontalInput;
            if (dashDirection == 0.0f)
                dashDirection = transform.forward.x > 0.0f ? 1.0f : -1.0f;
        }
        if(doDash && Time.time - dashStartTime > dashDuration)
        {
            doDash = false;
            dashStartTime = Time.time;
        }

        if(isGrounded && Input.GetKeyDown(KeyCode.UpArrow))
        {
            bashChargeStarted = true;
            bashChargeStartTime = Time.time;
        }
        if(bashChargeStarted && !playedBashParticles && Time.time - bashChargeStartTime > bashChargeTime)
        {
            bashParticles.Play();
            playedBashParticles = true;
        }
        if(bashChargeStarted && Input.GetKeyUp(KeyCode.UpArrow))
        {
            bashChargeStarted = false;
            playedBashParticles = false;
            if(Time.time - bashChargeStartTime > bashChargeTime)
                bashPower = bashSpeed;
        }


        bool focusingLantern = false;
        if (Input.GetMouseButton(1) && lanternPos != null)
        {
            focusingLantern = true;
            Time.timeScale = 0.4f;
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
            Vector3 direction = mouseWorldPos - lanternPos.position;

            //Debug.DrawLine(lanternPos.position, mouseWorldPos, Color.red);
            arrowSprite.gameObject.SetActive(true);

            arrowSprite.position = Camera.main.WorldToScreenPoint(lanternPos.position);
            arrowSprite.rotation = Quaternion.LookRotation(Vector3.forward, direction.normalized);
        }
        if (Input.GetMouseButtonUp(1) && lanternPos != null)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10.0f));
            Vector3 direction = mouseWorldPos - lanternPos.position;

            dodgeRedirectVelocity = direction.normalized * dodgeRedirectForce;
            rb.velocity = direction.normalized * dodgeRedirectForce;
            arrowSprite.gameObject.SetActive(false);
        }
        if (!focusingLantern)
        {
            Time.timeScale = 1.0f;
            arrowSprite.gameObject.SetActive(false);
        }
    }

    private void FixedUpdate()
    {
        MovementLogic();
    }

    public void EnteredLanternRange(Transform lanternPosition)
    {
        lanternPos = lanternPosition;
    }

    public void ExitLanternRange()
    {
        lanternPos = null;
    }

    private void MovementLogic()
    {
        Vector3 velocity = rb.velocity;

        float currentMoveDir = horizontalInput > 0.0f ? 1.0f : -1.0f;
        if (velocity.x != 0.0f)
        {
            //Rotate player to face move direction
            float rotAngle = velocity.x > 0.0f ? 90.0f : -90.0f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotAngle, transform.rotation.eulerAngles.z);
        }

        //Detect if we changed direction in mid-air
        if (!isGrounded)
        {
            if (currentMoveDir != previousMoveDir)
            {
                accelerationFactor = 0.0f;
            }
        }

        //Apply horizontal movement
        float xVel = moveSpeed * accelerationFactor * horizontalInput;
        xVel += wallJumpPropulsion * wallDirection;

        if (doDash)
            xVel += dashDirection * dashSpeed;

        velocity = new Vector3(xVel * Time.fixedDeltaTime, velocity.y, velocity.z) + dodgeRedirectVelocity * Time.fixedDeltaTime;

        dodgeRedirectVelocity = Vector3.Lerp(dodgeRedirectVelocity, Vector3.zero, Time.fixedDeltaTime * dodgeRedirectSlowDown);

        wallJumpPropulsion = Mathf.Clamp(wallJumpPropulsion - wallJumpDeacc * Time.fixedDeltaTime, 0.0f, wallJumpSpeed);

        //Retain certain variables used to detect change of direction in mid-air
        previousMoveDir = horizontalInput > 0.0f ? 1.0f : -1.0f;
        accelerationFactor = Mathf.Clamp(accelerationFactor + midairAcceleration * Time.fixedDeltaTime, 0.0f, 1.0f);

        //While we are holding the jump button
        if (holdingSpace)
        {
            //Check if we should make a new jump
            if (canJump && (canWallJump || isGrounded || canDoubleJump))
            {
                float speed = jumpSpeed;
                float jumpCounterMax = jumpInputTime;

                //Based on if it is first jump or second jump, change the properties
                if (!isGrounded && canWallJump)
                {
                    wallJumpPropulsion = wallJumpSpeed;
                    canWallJump = false;
                }
                else if (isGrounded == false) //Second jump
                {
                    speed *= 1.5f;
                    wallJumpPropulsion = 0.0f;
                    canDoubleJump = false;
                    jumpCounterMax = 0.0f;
                }
                else
                    isGrounded = false;
                canJump = false;
                bashPower = 0.0f;

                //Propell the character upwards
                velocity.y = speed * Time.fixedDeltaTime;
                jumpTimeCounter = Time.time + jumpCounterMax;
            }

            //While we are holding jump, keep applying force upwards (until the window for holding the jump is over)
            if (jumpTimeCounter - Time.time > 0.0f)
            {
                velocity.y = jumpSpeed * Time.fixedDeltaTime;
            }
        }

        velocity.y = velocity.y + bashPower * Time.deltaTime;
        bashPower = bashPower - bashDeacc * Time.deltaTime;
        if (bashPower < 0.0f)
            bashPower = 0.0f;

        //Custom gravity
        rb.AddForce(Vector3.down * rb.mass * gravity);

        if (!isGrounded && holdingShift)
            velocity.y = Mathf.Clamp(velocity.y, -hoverGravityLimiter, 100.0f);

        rb.velocity = velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ClimbableWall")
        {
            canWallJump = true;
            canDoubleJump = true;
            wallDirection = other.transform.position.x - transform.position.x;
            wallDirection /= Mathf.Abs(wallDirection); //Convert it to -1 or 1
        }
        else if(other.tag == "GroundPlatform")
        {
            isGrounded = true;
            canDoubleJump = true;
            canDash = true;
            accelerationFactor = 1.0f;
            wallJumpPropulsion = 0.0f;
        }
        else if(other.tag == "BreakablePlatform")
        {

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ClimbableWall")
        {
            canWallJump = false;
        }
    }
}
