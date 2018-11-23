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
     * Speed of the player's horizontal rotation.
     */
    public float rotSpeed;

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
     * The height at which the ship will hover at rest.
     */
    public float restingHeight;

    /**
     * The maximum speed the player can move in the horizontal plane.
     */
    public float maxHorizontalSpeed;

    ///////////////////////////////////////////////////////////////////////////
    // PlayerController
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The current height of the ship above the surface below.
     */
    private float currentHeight;

    /**
     * The player's box collider, representing the boundaries of its body.
     */
    private BoxCollider playerCollider;
  
    /**
     * Player's Rigidbody component.
     */
    private Rigidbody rigidbody;

    /**
     * The position where the player will respawn.
     */
    private Vector3 spawn;

    /**
     * Current velocity.
     */
    private Vector3 velocity;

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

    // Minimum player vertical position before respawning, in metres
    public const float RESPAWN_Y = -25f;

    /**
     * Initialises this controller.
     */
    void Start () {

        playerCollider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();

        // Remember the spawn point
        spawn = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z
        );
    }

    /**
     * Updates the player's visuals.
     */
    private void Update() {

        // Apply rotation
        Vector3 rotation = GetRotation();
        transform.Rotate(
                rotation.x * Time.deltaTime,
                rotation.y * Time.deltaTime,
                rotation.z * Time.deltaTime
        );
    }

    /**
     * Determines the rotation to apply based on horizontal input.
     */
    private Vector3 GetRotation() {
        return new Vector3(
                0,
                Input.GetAxis("Horizontal") * rotSpeed,
                0
        );
    }

    /**
     * Moves the player according to the user input.
     * 
     * Anything physics-related goes here.
     */
    void FixedUpdate() {

        // Determine whether player is grounded
        currentHeight = GetDistanceToGround();
        if (currentHeight <= restingHeight && currentHeight != 0f) {
            grounded = true;
        } else {
            grounded = false;
        }

        // Handle jumping / landing
        if (grounded && Input.GetKeyDown(KeyCode.Space)) {
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

        // Calculate the final movement vector based on velocity, forward
        // direction and delta time
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 moveForce = new Vector3(
                forward.x * velocity.x,
                velocity.y,
                forward.z * velocity.z
        );

        // Clear all previous forces
        rigidbody.velocity = Vector3.zero;

        // Set the new velocity
        rigidbody.AddForce(moveForce, ForceMode.VelocityChange);
    }

    /**
     * Determines if the player's jump should end.
     */
    private bool HasJumpFinished() {

        if (spentJumpTime < minJumpTime) {
            // Minimum jump time not yet elapsed
            return false;
        }

        if (!Input.GetKey(KeyCode.Space)) {
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

        // Determine new vertical velocity considering gravity, jumping and
        // hovering
        float newVelocityY = GetPrevVelocityY() + GetVerticalVelocityModifier();

        // Limit vertical velocity
        newVelocityY = Mathf.Clamp(newVelocityY,
            -PhysicsHelper.MAX_PLAYER_SPEED_Y,
            PhysicsHelper.MAX_PLAYER_SPEED_Y);

        // Limit horizontal velocity
        Vector2 horizontalVelocity = Vector2.ClampMagnitude(
                new Vector2(newVelocityX, newVelocityZ),
                maxHorizontalSpeed
        );

        return new Vector3(
                horizontalVelocity.x,
                newVelocityY,
                horizontalVelocity.y
        );
    }

    /**
     * Determine the distance to the surface beneath using a Raycast.
     */
    private float GetDistanceToGround() {
        RaycastHit hit;
        Vector3 raycastStart = new Vector3(
                playerCollider.transform.position.x,
                playerCollider.transform.position.y,
                playerCollider.transform.position.z
        );

        // Ping ground
        Physics.Raycast(raycastStart, Vector3.down, out hit, Mathf.Infinity);
        return hit.distance;
    }

    /**
     * Determines the acceleration to apply based on vertical input.
     */
    private float GetAcceleration() {
        return Input.GetAxisRaw("Vertical") * acceleration;
    }

    /**
     * Gets the initial vertical velocity to use for this frame.
     */
    private float GetPrevVelocityY() {
        // Reset vertical velocity when player is grounded
        return grounded ? 0 : velocity.y;
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
            return CalculateHoverRate();
        }

        // Falling
        return PhysicsHelper.GRAVITY;
    }

    /**
     * Provide the speed at which the player should hover upwards this frame.
     */
    private float CalculateHoverRate() {
        return 0.1f;
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
        rigidbody.position = new Vector3(
                spawn.x,
                spawn.y,
                spawn.z
        );
        velocity = Vector3.zero;

        foreach (ICharacterListener listener in characterListeners) {
            listener.CharacterTeleported();
        }
    }

}
