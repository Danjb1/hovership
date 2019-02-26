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
    private float rotationSpeed;

    /**
     * Player's Rigidbody component.
     */
    private Rigidbody rigidbodyComponent;

    /**
     * Player's Collider component.
     */
    private Collider colliderComponent;

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

        // Set game state to Playing
        StateManager.Instance.gameState = GameState.PLAYING;

        rigidbodyComponent = GetComponent<Rigidbody>();
        colliderComponent = GetComponent<Collider>();

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
        UpdateRotation();
        UpdateHoverSpeed();
        UpdateJump();
        UpdateVelocity();
    }

    /**
     * Rotates the player based on the current input.
     */
    private void UpdateRotation() {
        Vector3 rotation = GetRotation();
        transform.Rotate(
                rotation.x * Time.deltaTime,
                rotation.y * Time.deltaTime,
                rotation.z * Time.deltaTime
        );
    }

    /**
     * Updates the player's hover speed when close to the ground.
     * 
     * Also takes care of landing.
     */
    private void UpdateHoverSpeed() {
        float currentHeight = GetAverageDistanceToGround();
        if (currentHeight < hoverHeight) {
            hoverSpeed = GetHoverSpeed(currentHeight);

            // Land, if falling
            if (velocity.y < 0) {
                velocity.y = 0;
                grounded = true;
            }
        } else {
            grounded = false;
        }
    }

    /**
     * Updates the player's current jump parameters.
     */
    private void UpdateJump() {
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
    }

    /**
     * Updates the player's current velocity.
     */
    private void UpdateVelocity() {

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
        
        // Determine the distance to the ground from various points within the
        // player. This should be enough to detect what the player is standing
        // on in 99% of cases.
        IList<float> results = new List<float>() {
            HoverHeight(0, 0),
            HoverHeight(-1, -1),
            HoverHeight(-1, -0.5f),
            HoverHeight(-1, 0.5f),
            HoverHeight(-1, 1),
            HoverHeight(1, -1),
            HoverHeight(1, -0.5f),
            HoverHeight(1, 0.5f),
            HoverHeight(1, 1),
        };
        
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
     * Determines the current distance between some point of the player and the
     * ground.
     */
    private float HoverHeight(float xFactor, float zFactor) {
        float width = colliderComponent.bounds.extents.x;
        float length = colliderComponent.bounds.extents.z;
        return PhysicsHelper.DistanceToGround(
            new Vector3(
                    transform.position.x + width * xFactor,
                    transform.position.y,
                    transform.position.z + length * zFactor
            ),
            hoverHeight);
    }

    /**
     * Determines the player's hover speed based on the current height.
     */
    private float GetHoverSpeed(float currentHeight) {

        // Skip hover mechanics when jumping
        if (jumping) {
            return 0;
        }

        // Set hover speed based on collision depth
        float depth = hoverHeight - currentHeight;
        return depth / HOVER_TIME;
    }

    /**
     * Determines the rotation to apply based on horizontal input.
     */
    private Vector3 GetRotation() {

        if (rotationInput == 0) {
            rotationSpeed *= ROTATIONAL_FRICTION;
        } else {
            rotationSpeed += rotationalAcceleration * rotationInput;

            // Limit maximum rotational speed
            if (Math.Abs(rotationSpeed) > maxRotationSpeed) {
                rotationSpeed = rotationSpeed > 0
                        ? maxRotationSpeed
                        : -maxRotationSpeed;
            }
        }

        return Vector3.up * rotationSpeed;
    }

    /**
     * Gets the speed at which the player is currently rotating.
     */
    public float GetRotationSpeed() {
        return rotationSpeed;
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

    /**
     * Event handler for collisions. Used with specific GameObject tags.
     */
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Finish") {
            StateManager.Instance.gameState = GameState.CELEBRATING;
        }
    }

}
