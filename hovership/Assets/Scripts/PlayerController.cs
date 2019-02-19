using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    /**
     * Player's acceleration, in metres per second per second.
     */
    public float acceleration;

    /**
     * The maximum speed the player can move in the horizontal plane.
     */
    public float maxSpeed;

    /**
     * Player's rotational acceleration.
     */
    public float rotationalAcceleration;

    /**
     * Speed of the player's horizontal rotation.
     */
    public float maxRotationSpeed;

    /**
     * How much jumping affects the player's vertical velocity.
     */
    public float jumpStrength;

    /**
     * The minimum amount of time a jump will last.
     *
     * This prevents jerky-looking movements when the jump key is tapped very
     * briefly.
     */
    public float minJumpTime;

    /**
     * The maximum amount of time a jump can last, if the jump key is held
     * down.
     */
    public float maxJumpTime;

    /**
     * The height at which the player should hover above the ground.
     */
    public float hoverHeight;

    ///////////////////////////////////////////////////////////////////////////
    // PlayerController
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The maximum gradient of a surface the player can walk on.
     * 
     * This prevents the player being able to climb up very steep slopes, or
     * even walls.
     */
    private const float MAX_SLOPE_GRADIENT = 0.5f;

    /**
     * Time (in seconds) to reach the optimal hover height when below it.
     * 
     * In reality it takes longer than this because the upwards velocity is
     * recalculated each frame, so the "journey time" resets. However, this at
     * least has some bearing on the overall time taken.
     */
    private const float HOVER_TIME = 0.1f;
    
    /**
     * The factor by which the player's rotational speed will be multiplied each
     * frame when no rotational input is applied.
     */
    private const float ROTATIONAL_FRICTION = 0.9f;

    /**
     * Minimum player vertical position before respawning, in metres.
     */
    private const float RESPAWN_Y = -25f;

    /**
     * Rotational axis input.
     */
    private float rotationInput;

    /**
     * Whether the jump key is pressed.
     */
    private bool jumpKeyDown;

    /**
     * From left to right, the amount of rotation applied during the previous
     * frame.
     */
    private float rotation;

    /**
     * Player's Rigidbody component.
     */
    private Rigidbody rigidbodyComponent;

    /**
     * Player's Collider component.
     */
    private Collider collider;

    /**
     * The position where the player will respawn.
     */
    private Vector3 spawn;

    /**
     * The rotation of the player at the spawn.
     */
    private Vector3 spawnRotation;

    /**
     * Current velocity.
     */
    private Vector3 velocity;

    /**
     * The vertical speed with which the player should hover to aim to return
     * to optimal height after HOVER_TIME.
     * 
     * This is transient, and does not affect velocity between frames. This
     * prevents the player from overshooting the optimal hover height.
     */
    private float hoverSpeed;

    /**
     * Flag set during ascent when jumping.
     */
    private bool jumping;

    /**
     * Time elaspsed during the current ascent.
     */
    private float spentJumpTime;

    /**
     * Flag set if the player is currently grounded.
     */
    private bool grounded;

    /**
     * Flag set if the player was grounded last frame.
     */
    private bool previouslyGrounded;

    /**
     * List of registered CharacterListeners.
     */
    private List<ICharacterListener> characterListeners = new List<ICharacterListener>();
    
    /**
     * Initialises this controller.
     */
    void Start () {

        rigidbodyComponent = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        // Remember the spawn point
        spawn = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z
        );
        spawnRotation = new Vector3(
                transform.rotation.x,
                transform.rotation.y,
                transform.rotation.z
        );
    }

    /**
     * Polls user input.
     */
    private void Update() {
        jumpKeyDown = Input.GetAxis("Jump") > 0;
        rotationInput = Input.GetAxisRaw("Horizontal");
    }

    /**
     * Moves the player according to the user input.
     *
     * Anything physics-related goes here.
     */
    void FixedUpdate() {

        // Hover
        float currentHeight = GetAverageDistanceToGround();
        if (currentHeight < hoverHeight) {
            Hover(currentHeight);
        } else {
            grounded = false;
        }
        
        // Apply rotation
        Vector3 rotation = GetRotation();
        transform.Rotate(
                rotation.x * Time.deltaTime,
                rotation.y * Time.deltaTime,
                rotation.z * Time.deltaTime
        );

        // Handle jumping / landing
        if (grounded && jumpKeyDown) {
            jumping = true;
        }
        if (jumping) {
            spentJumpTime += Time.deltaTime;
            if (HasJumpFinished()) {
                // End jump
                jumping = false;
                spentJumpTime = 0;
            }
        }

        // Set the new velocity
        velocity = CalculateVelocity();

        // Calculate the final movement vector based on velocity and direction
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 moveForce = new Vector3(
                forward.x * velocity.x,
                velocity.y + hoverSpeed,
                forward.z * velocity.z
        );

        // Set the new velocity
        rigidbodyComponent.velocity = moveForce;
    }

    /**
     * Gets the current distance between the player and the ground.
     */
    private float GetAverageDistanceToGround() {

        IList<float> results = new List<float>();

        // Determine the corner points of the player
        float x1 = transform.position.x + collider.bounds.extents.x;
        float x2 = transform.position.x - collider.bounds.extents.x;
        float z1 = transform.position.z + collider.bounds.extents.z;
        float z2 = transform.position.z - collider.bounds.extents.z;

        // Determine the distance to the ground at each corner, and the centre.
        // This should be enough to detect what the player is standing on in
        // 99% of cases.
        results.Add(GetDistanceToGround(transform.position));
        results.Add(GetDistanceToGround(new Vector3(x1, transform.position.y, z1)));
        results.Add(GetDistanceToGround(new Vector3(x1, transform.position.y, z2)));
        results.Add(GetDistanceToGround(new Vector3(x2, transform.position.y, z2)));
        results.Add(GetDistanceToGround(new Vector3(x2, transform.position.y, z1)));

        // Find the average distance to the ground based on all collisions
        float totalDist = 0;
        int numCollisions = 0;
        foreach (float result in results) {
            if (result != Mathf.Infinity) {
                totalDist += result;
                numCollisions++;
            }
        }

        // Return the average collision distance
        if (numCollisions > 0) {
            return totalDist / numCollisions;
        }

        // No collisions!
        return Mathf.Infinity;
    }

    /**
     * Gets the current distance between some position and the ground.
     */
    private float GetDistanceToGround(Vector3 position) {
        RaycastHit hit;
        if (Physics.Raycast(position, -Vector3.up, out hit, hoverHeight)) {
            if (hit.normal.y >= MAX_SLOPE_GRADIENT) {
                return hit.distance;
            }
        }
        return Mathf.Infinity;
    }

    /**
     * Sets the player's hover speed based on the current height.
     */
    private void Hover(float currentHeight) {

        // Skip hover mechanics when jumping
        if (jumping) {
            hoverSpeed = 0;
            return;
        }

        // Set hover speed based on collision depth
        float depth = hoverHeight - currentHeight;
        hoverSpeed = depth / HOVER_TIME;

        // Set grounded
        grounded = true;

        // Clear vertical velocity if falling
        if (velocity.y < 0) {
            velocity.y = 0;
        }
    }

    /**
     * Determines the rotation to apply based on horizontal input.
     */
    private Vector3 GetRotation() {

        if (rotationInput == 0) {
            rotation *= ROTATIONAL_FRICTION;
        } else {
            rotation += rotationalAcceleration * rotationInput;

            // Limit maximum rotational speed
            if (Math.Abs(rotation) > maxRotationSpeed) {
                rotation = rotation > 0
                        ? maxRotationSpeed
                        : -maxRotationSpeed;
            }
        }

        return Vector3.up * rotation;
    }

    /**
     * Determines if the player's jump should end.
     */
    private bool HasJumpFinished() {

        if (spentJumpTime < minJumpTime) {
            // Minimum jump time not yet elapsed
            return false;
        }

        if (Input.GetAxis("Jump") == 0) {
            // Jump was released
            return true;
        }

        // Jump has finished if max jump time has been reached
        return spentJumpTime >= maxJumpTime;
    }

    /**
     * Calculates the new velocity vector.
     */
    private Vector3 CalculateVelocity() {

        // Apply acceleration
        float acceleration = GetAcceleration();
        float newVelocityX = velocity.x + acceleration;
        float newVelocityZ = velocity.z + acceleration;

        // Apply friction (when not accelerating)
        if (Mathf.Abs(acceleration) == 0) {
            float friction = grounded
                    ? PhysicsHelper.FRICTION
                    : PhysicsHelper.AIR_FRICTION;
            newVelocityX *= friction;
            newVelocityZ *= friction;
        }

        // Determine new vertical velocity considering gravity and jumping
        float newVelocityY = velocity.y + GetVerticalVelocityModifier();

        // Limit vertical velocity
        newVelocityY = Mathf.Clamp(newVelocityY,
            PhysicsHelper.MAX_FALL_SPEED_Y,
            PhysicsHelper.MAX_JUMP_SPEED_Y);

        // Limit horizontal velocity
        Vector2 horizontalVelocity = Vector2.ClampMagnitude(
                new Vector2(newVelocityX, newVelocityZ),
                maxSpeed
        );

        return new Vector3(
                horizontalVelocity.x,
                newVelocityY,
                horizontalVelocity.y
        );
    }

    /**
     * Determines the acceleration to apply based on vertical input.
     */
    private float GetAcceleration() {
        return Input.GetAxisRaw("Vertical") * acceleration;
    }

    /**
     * Gets the vertical velocity modifier to apply this frame.
     */
    private float GetVerticalVelocityModifier() {

        // Jumping
        if (jumping) {
            return jumpStrength;
        }

        // Hovering
        if (grounded) {
            return 0;
        }

        // Falling
        return PhysicsHelper.GRAVITY;
    }

    /**
     * Updates the player logic after all movement has finished.
     */
    void LateUpdate() {

        // Respawn if we have fallen out of the world
        if (transform.position.y < RESPAWN_Y) {
            Respawn();
        }

        // Notify listeners when we land!
        if (!previouslyGrounded && grounded) {
            foreach (ICharacterListener listener in characterListeners) {
                listener.CharacterLanded();
            }
            previouslyGrounded = true;
        } else if (!grounded) {
            previouslyGrounded = false;
        }
    }

    /**
     * Registers the given listener for physics-related callbacks.
     */
    public void RegisterCharacterListener(ICharacterListener listener) {
        characterListeners.Add(listener);
    }

    /**
     * Moves the player back to the spawn point.
     */
    private void Respawn() {

        // Reset position
        transform.position = new Vector3(
                spawn.x,
                spawn.y,
                spawn.z
        );

        // Reset rotation
        transform.rotation = Quaternion.Euler(spawnRotation);

        // Reset velocity
        velocity = Vector3.zero;

        // Inform listeners of the new position
        foreach (ICharacterListener listener in characterListeners) {
            listener.CharacterTeleported();
        }
    }

    /**
     * Callback for when the air cushion collides with the ground.
     */
    public void AirCushionCollided(float depth) {
//        maxCollisionDepth = Mathf.Max(maxCollisionDepth, depth);
    }

    /**
     * Callback for when the air cushion stops colliding with the ground.
     */
    public void AirCushionCollisionExit() {
    //    grounded = false;
    }

}
