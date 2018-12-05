using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IAirCushionListener {

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
     * The amount of time in seconds that the player aims to spend hovering up
     * to optimal hover height, used every frame while hovering.
     *
     * N.B. It takes longer than this because we recalculate the required
     * velocity using this constant every frame.
     */
    private static const float HOVER_TIME = 0.1f;

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
    private float previousRotation;

    /**
     * The player's box collider, representing the boundaries of its body.
     */
    private BoxCollider playerCollider;

    /**
     * Player's Rigidbody component.
     */
    private Rigidbody rigidbodyComponent;

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

    // Minimum player vertical position before respawning, in metres
    public const float RESPAWN_Y = -25f;

    /**
     * Initialises this controller.
     */
    void Start () {

        playerCollider = GetComponent<BoxCollider>();
        rigidbodyComponent = GetComponent<Rigidbody>();

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
        rotationInput = Input.GetAxis("Horizontal");
    }

    /**
     * Moves the player according to the user input.
     *
     * Anything physics-related goes here.
     */
    void FixedUpdate() {

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

        // Calculate the final movement vector based on velocity, forward
        // direction and delta time
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 moveForce = new Vector3(
                forward.x * velocity.x,
                velocity.y + hoverSpeed,
                forward.z * velocity.z
        );

        // Clear previous velocity
        rigidbodyComponent.velocity = Vector3.zero;

        // Set the new velocity
        rigidbodyComponent.AddForce(moveForce, ForceMode.VelocityChange);
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

        hoverSpeed = Math.Abs(depth) / HOVER_TIME;

        // Set grounded
        grounded = true;

        // Clear vertical velocity if falling
        if (!jumping && velocity.y < 0) {
            velocity.y = 0;
        }
    }

    public void AirCushionCollisionExit() {
        grounded = false;
    }

}
