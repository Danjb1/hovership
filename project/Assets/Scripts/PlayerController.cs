using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;
    public float rotSpeed;
    public float jumpStrength;

    private const float GRAVITY = -1f;
    private const float FRICTION = 0.95f;

    private CharacterController controller;
    private Vector3 velocity;

    // Use this for initialization
    void Start () {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {

        // Rotate based on horizontal input
        float y = Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed;
        transform.Rotate(0, y, 0);

        // Accelerate based on vertical input
        float acceleration = Input.GetAxis("Vertical") * speed;

        // Don't allow backwards movement
        acceleration = Mathf.Max(acceleration, 0.0f);

        // Determine horizontal velocity modifiers based on acceleration
        float xMod = acceleration;
        float zMod = acceleration;

        // Determine vertical velocity modifier based on gravity and jumping
        float yMod = GRAVITY;
        if (controller.isGrounded) {

            // Reset vertical velocity when player is grounded
            velocity = new Vector3(
                    velocity.x,
                    0,
                    velocity.z
            );

            // Jumping
            if (Input.GetKeyDown(KeyCode.Space)) {
                yMod = jumpStrength;
            }
        }

        // Apply friction and velocity modifiers
        velocity = new Vector3(
                velocity.x * FRICTION + xMod * Time.deltaTime,
                velocity.y + yMod * Time.deltaTime,
                velocity.z * FRICTION + zMod * Time.deltaTime
        );

        // Calculate movement vector based on velocity and forward direction
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 moveDir = new Vector3(
                forward.x * velocity.x,
                velocity.y,
                forward.z * velocity.z
        );

        // Move using our CharacterController
        controller.Move(moveDir);
    }
    
}
