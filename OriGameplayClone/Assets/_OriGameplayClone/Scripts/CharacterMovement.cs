using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float jumpSpeed = 1.0f;
    public float jumpInputTime = 1.0f; //For the first jump, if we hold they key, we can jump higher. This variable defined how long we can keep pressing
    public float gravity = 15.0f; //The default gravity didn't achieve proper results, so I defined in fixed update a new gravity
    public float midairAcceleration = 10.0f; //In mid-air, if we change direction, we need to accelerate from 0 to a certain speed.

    private bool isGrounded = true;     //Did we hit a ground collider?
    private bool canClimbWall = false;  //Did we hit a climbable wall collider?
    private bool canJump = true;        //Set to true when lifting jump key & false when actually jumping
    private bool canDoubleJump = true;  //Reset when we hit the ground
    private bool holdingSpace = false;  //Used to create the effect of holding jump key to jump higher

    private float wallDirection;            //What direction is the climbable wall that we hit in?
    private float jumpTimeCounter = 0.0f;   //Time until we will detect jump hold
    private float horizontalInput = 0.0f;
    private float previousMoveDir = 1.0f;   //Used to detect change of direction in mid-air
    private float accelerationFactor = 1.0f;//Goes from 0 to 1 when we change direction in mid-air

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //Detect all input in update
        horizontalInput = Input.GetAxis("Horizontal");

        holdingSpace = Input.GetKey(KeyCode.Space);
        if (Input.GetKeyUp(KeyCode.Space))
        {
            canJump = true;
            jumpTimeCounter = Time.time;
        }
    }

    private void FixedUpdate()
    {
        //Rotate player to face move direction
        if (horizontalInput != 0.0f)
        {
            float rotAngle = horizontalInput > 0.0f ? 90.0f : -90.0f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotAngle, transform.rotation.eulerAngles.z);
        }

        //Detect if we changed direction in mid-air
        if (!isGrounded)
        {
            float currentMoveDir = horizontalInput > 0.0f ? 1.0f : -1.0f;
            if (currentMoveDir != previousMoveDir)
            {
                accelerationFactor = 0.0f;
            }
        }

        //Apply horizontal movement
        rb.velocity = new Vector3(moveSpeed * accelerationFactor * horizontalInput * Time.fixedDeltaTime, rb.velocity.y, rb.velocity.z);

        //Retain certain variables used to detect change of direction in mid-air
        previousMoveDir = horizontalInput > 0.0f ? 1.0f : -1.0f;
        accelerationFactor = Mathf.Clamp(accelerationFactor + midairAcceleration * Time.fixedDeltaTime, 0.0f, 1.0f);

        //While we are holding the jump button
        if (holdingSpace)
        {
            //Check if we should make a new jump
            if (canJump && (isGrounded || canDoubleJump))
            {
                float speed = jumpSpeed;
                float jumpCounterMax = jumpInputTime;

                //Based on if it is first jump or second jump, change the properties
                if (isGrounded == false)
                {
                    speed *= 1.5f;
                    canDoubleJump = false;
                    jumpCounterMax = 0.0f;
                }
                else
                    isGrounded = false;
                canJump = false;

                //Propell the character upwards
                rb.velocity = new Vector3(rb.velocity.x, speed * Time.fixedDeltaTime, rb.velocity.z);
                jumpTimeCounter = Time.time + jumpCounterMax;
            }

            //While we are holding jump, keep applying force upwards (until the window for holding the jump is over)
            if (jumpTimeCounter - Time.time > 0.0f)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed * Time.fixedDeltaTime, rb.velocity.z);
            }
        }

        //Custom gravity
        rb.AddForce(Vector3.down * rb.mass * gravity);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "ClimbableWall")
        {
            canClimbWall = true;
            wallDirection = other.transform.position.x - transform.position.x;
            wallDirection /= Mathf.Abs(wallDirection); //Convert it to -1 or 1
        }
        else if(other.tag == "GroundPlatform")
        {
            isGrounded = true;
            canDoubleJump = true;
            accelerationFactor = 1.0f;
        }
        else if(other.tag == "BreakablePlatform")
        {

        }
    }
}
