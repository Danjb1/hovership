using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IGroundedListener {

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
     * The maximum speed the player can move in the horizontal plane.
     */
    public float maxHorizontalSpeed;

    ///////////////////////////////////////////////////////////////////////////
    // PlayerController
    ///////////////////////////////////////////////////////////////////////////
    
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

    /**
     * Initialises this controller.
     */
    void Start () {

        // Remember the spawn point
        spawn = new Vector3(
                transform.position.x,
                transform.position.y,
                transform.position.z
        );
    }

    /**
     * Moves the player according to the user input.
     */
    void Update() {

        // Apply rotation
        Vector3 rotation = GetRotation();
        transform.Rotate(
                rotation.x * Time.deltaTime,
                rotation.y * Time.deltaTime,
                rotation.z * Time.deltaTime
        );

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
        Vector3 moveDir = new Vector3(
                forward.x * velocity.x * Time.deltaTime,
                velocity.y * Time.deltaTime,
                forward.z * velocity.z * Time.deltaTime
        );

        // Move the desired amount
        transform.position += moveDir;
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
                    ? Physics.FRICTION
                    : Physics.AIR_FRICTION;
            newVelocityX *= friction;
            newVelocityZ *= friction;
        }

        // Apply gravity and / or jump force
        float newVelocityY = GetPrevVelocityY() + GetVerticalVelocityModifier();

        // Limit vertical velocity
        newVelocityY = Mathf.Max(newVelocityY, -Physics.MAX_PLAYER_SPEED_Y);
        newVelocityY = Mathf.Min(newVelocityY, Physics.MAX_PLAYER_SPEED_Y);

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

        // Grounded
        if (grounded) {
            return 0;
        }

        return Physics.GRAVITY;
    }

    /**
     * Updates the player logic after all movement has finished.
     */
    void LateUpdate() {

        // Respawn if we have fallen out of the world
        if (transform.position.y < Physics.RESPAWN_Y) {
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
        transform.position = new Vector3(
                spawn.x,
                spawn.y,
                spawn.z
        );
        velocity = new Vector3(0, 0, 0);

        foreach (ICharacterListener listener in characterListeners) {
            listener.CharacterTeleported();
        }
    }

    private void OnTriggerEnter(Collider other) {
    }
    
    private void OnTriggerStay(Collider other) {
    }

    private void OnTriggerExit(Collider other) {
    }

    public void SetGrounded(bool grounded) {
        this.grounded = grounded;
    }
}
