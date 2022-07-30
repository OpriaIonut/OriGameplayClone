using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float movementSpeed = 1.0f;
    public float maxSpeed = 10.0f;
    public float jumpSpeed = 100.0f;

    private Rigidbody rb;

    private bool isGrounded = true;
    private bool canClimb = false;
    private float wallDirection = 0.0f;
    private float horizontalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        if(Input.GetKey(KeyCode.Space) && (isGrounded /*|| canClimb*/))
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * jumpSpeed);
                rb.velocity += Vector3.right * movementSpeed * horizontalInput * 0.75f;
            }
            //else if (canClimb)
            //{
            //    Vector3 moveDir = new Vector3(wallDirection, 1.0f, 0.0f);
            //    rb.AddForce(moveDir * jumpSpeed);
            //    rb.velocity += Vector3.right * movementSpeed * horizontalInput * 0.75f;
            //}
            isGrounded = false;
            canClimb = false;
        }
    }

    private void FixedUpdate()
    {
        if (isGrounded)
        {
            Vector3 newPos = transform.position + Vector3.right * horizontalInput * movementSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPos);
        }
        else
        {
            float moveFactor = rb.velocity.x + horizontalInput * movementSpeed * 2.0f * Time.fixedDeltaTime;
            moveFactor = Mathf.Clamp(moveFactor, -maxSpeed, maxSpeed);

            rb.velocity = new Vector3(moveFactor, rb.velocity.y, rb.velocity.z);
        }

        if (horizontalInput != 0.0f)
        {
            float rotAngle = horizontalInput > 0.0f ? 90.0f : -90.0f;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotAngle, transform.rotation.eulerAngles.z);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        rb.velocity = Vector3.zero;

        if(collision.gameObject.tag == "ClimbableWall")
        {
            canClimb = true;
            wallDirection = collision.transform.position.x - transform.position.x;
            wallDirection /= Mathf.Abs(wallDirection); //Convert it to -1 or 1
        }
        else
        {
            isGrounded = true;
            canClimb = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        canClimb = true;
    }
}
