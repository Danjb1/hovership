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
     * The maximum speed the player can move in the horizontal plane, in metres
     * per second.
     */
    public float maxSpeed;

    /**
     * Player's rotational acceleration, in degrees per second per second.
     */
    public float rotationalAcceleration;

    /**
     * Speed of the player's horizontal rotation, in degrees per second.
     */
    public float maxRotationSpeed;

    /**
     * How much jumping affects the player's vertical velocity.
     */
    public float jumpStrength;

    /**
     * The minimum amount of time a jump will last, in seconds.
     *
     * This prevents jerky-looking movements when the jump key is tapped very
     * briefly.
     */
    public float minJumpTime;

    /**
     * The maximum amount of time a jump can last, if the jump key is held
     * down, in seconds.
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
     * Friction applied in the player's forward direction, when not
     * accelerating.
     */
    public const float FRICTION_FORWARD = 0.05f;

    /**
     * Friction applied perpendicular to the player's forward direction, every
     * frame.
     *
     * The lower this value, the more the player will drift.
     */
    public const float FRICTION_LATERAL = 0.045f;

    // As above, but applies when the player is airborne.
    public const float AIR_FRICTION_FORWARD = 0.025f;

    // As above, but applies when the player is airborne.
    public const float AIR_FRICTION_LATERAL = 0.025f;

    /**
     * Time (in seconds) to reach the optimal hover height when below it.
     *
     * In reality it takes longer than this because the upwards velocity is
     * recalculated each frame, so the "journey time" resets. However, this at
     * least has some bearing on the overall time taken.
     */
    private const float HOVER_TIME = 0.2f;

    /**
     * The factor by which the player's rotational speed will be multiplied each
     * frame when no rotational input is applied.
     */
    private const float ROTATIONAL_FRICTION = 0.9f;

    /**
     * The distance the player falls below the ground plane before respawning.
     */
    private const float RESPAWN_DEPTH = 40f;

    /**
     * Rotational axis input.
     */
    private float rotationInput;

    /**
     * Whether the jump key is pressed.
     */
    private bool jumpKeyDown;

    /**
     * Velocity in the y-axis.
     *
     * We track this ourselves instead of relying on the rigidbody velocity
     * because we don't want it to be affected by ramps.
     */
    private float yVelocity;

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
     * Maximum height above ground at which jumping is still permitted.
     *
     * Jumping should be allowed even if we are slightly above what we consider
     * "grounded"; this is because when the player hits a ramp, the sudden
     * hover speed can sometimes propel the player upwards.
     */
    private float maxHeightPermittingJump;

    /**
     * Player's height above the ground (averaged from multiple points).
     */
    private float currentHeight;

    /**
     * Player's height above the ground, measured from the centre of the fuselage.
     */
    private float currentCentreHeight;

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
     * Time elaspsed during the current ascent, seconds.
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
     * The box collider's width.
     */
    private float width;

    /**
     * The box collider's length.
     */
    private float length;

    /**
     * The player's current slide direction. -1 => left, 0 => nowhere, 1 => right.
     */
    private int slideDirection;

    /**
     * The clearance allowed beneath a wing before corrective sliding occurs.
     */
    private float slideThreshold;

    /**
     * The clearance allowed beneath a wingtip before corrective sliding occurs.
     */
    private float wingtipSlideThreshold;

    /**
     * Whether the altitude of the fuselage is within the designated hover height.
     */
    private bool isFuselageNearGround;

    /**
     * The global Y-position at which the player will respawn, in metres.
     */
    private float respawnHeight;

    /**
     * The global Y-position of the level's ground plane.
     */
    private float groundPlaneHeight;

    /**
     * Whether the player has fallen below the ground plane, and is about to
     * respawn.
     */
    private bool isRespawning;

    /**
     * Initialises this controller.
     */
    void Start () {

        // Acquire player's components
        rigidbodyComponent = GetComponent<Rigidbody>();
        colliderComponent = GetComponent<Collider>();

        // Set game state to Playing
        StateManager.Instance.SetState(GameState.PLAYING);

        // Determine ground plane and respawn heights
        groundPlaneHeight = StateManager.Instance.GetLevelGroundHeight();
        respawnHeight = groundPlaneHeight - RESPAWN_DEPTH;

        // Determine height thresholds for sliding
        slideThreshold = hoverHeight * PhysicsHelper.SLIDE_THRESHOLD_RATIO;
        wingtipSlideThreshold = slideThreshold * PhysicsHelper.WINGTIP_THRESHOLD_RATIO;

        // Determine the maximum height at which jumping is permitted
        maxHeightPermittingJump = hoverHeight * 2f;

        // Capture the box collider's dimensions
        // TODO: Rotate the collider to align with player.forward
        width = colliderComponent.bounds.extents.x;
        length = colliderComponent.bounds.extents.z;

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

        // Skip all updates if game is paused
        if (StateManager.Instance.GetState() != GameState.PLAYING) {
            rigidbodyComponent.velocity = Vector3.zero;
            return;
        }

        DetectRespawn();
        UpdateRotation();
        UpdateHoverSpeed();
        UpdateCorrectiveSlide();
        UpdateJump();
        UpdateVelocity();
    }

    /**
     * Determine whether the player is about to respawn, i.e. has fallen below
     * the ground plane.
     */
    private void DetectRespawn() {
        isRespawning = transform.position.y < groundPlaneHeight;
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
        currentHeight = GetAverageDistanceToGround();
        if (currentHeight < hoverHeight) {
            hoverSpeed = GetHoverSpeed(currentHeight);

            // Land, if falling
            if (yVelocity < 0) {
                yVelocity = 0;
                grounded = true;
            }
        } else {
            hoverSpeed = 0;
            grounded = false;
        }
    }

    /**
     * Determine whether the player must slide to one side to avoid its wing
     * getting stuck on something.
     */
    private void UpdateCorrectiveSlide() {
        if (!isFuselageNearGround) {
            if (ShouldSlide(true)) {
                slideDirection = -1;
            } else if (ShouldSlide(false)) {
                slideDirection = 1;
            }
        } else {
            slideDirection = 0;
        }
    }

    /**
     * Determine whether the player should slide away from the wing specified.
     */
    private bool ShouldSlide(bool isRightWing) {

        int multiplier = isRightWing ? 1 : -1;

        IList<KeyValuePair<float, float>> checks = new List<KeyValuePair<float, float>>() {
            new KeyValuePair<float, float>(
                    GetHoverHeight(0.7f * multiplier, -0.25f),      // wingtip fore
                    wingtipSlideThreshold
            ),
            new KeyValuePair<float, float>(
                    GetHoverHeight(0.7f * multiplier, -0.4f),       // wingtip aft
                    wingtipSlideThreshold
            ),
            new KeyValuePair<float, float>(
                    GetHoverHeight(0.5f * multiplier, -0.15f),      // wing leading edge
                    slideThreshold
            ),
            new KeyValuePair<float, float>(
                    GetHoverHeight(0.4f * multiplier, -0.1f),       // wing root fore
                    slideThreshold
            ),
            new KeyValuePair<float, float>(
                    GetHoverHeight(0.4f * multiplier, -0.45f),      // wing trailing edge
                    slideThreshold
            )
        };

        foreach(KeyValuePair<float, float> entry in checks) {
            if (entry.Key < entry.Value) {
                return true;
            }
        }
        return false;
    }

    /**
     * Updates the player's current jump parameters.
     */
    private void UpdateJump() {
        if (isRespawning) {
            return;
        }
        if (jumpKeyDown && IsJumpAllowed()) {
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

    private bool IsJumpAllowed() {
        return grounded || currentCentreHeight < maxHeightPermittingJump;
    }

    /**
     * Updates the player's current velocity.
     */
    private void UpdateVelocity() {

        // Set the new velocity
        Vector3 newVelocity = CalculateVelocity();

        // Add hover speed
        newVelocity = new Vector3(
                newVelocity.x,
                newVelocity.y + hoverSpeed,
                newVelocity.z
        );

        // Apply corrective slide, to avoid being stuck
        newVelocity = ApplyCorrectiveSlide(newVelocity);

        // Set the new velocity
        yVelocity = newVelocity.y;
        rigidbodyComponent.velocity = newVelocity;
    }

    /**
     * Gets the current distance between the player and the ground.
     */
    private float GetAverageDistanceToGround() {

        // Determine the distance to the ground from various points within the
        // player. This should be enough to detect what the player is standing
        // on in 99% of cases.
        IList<float> results = new List<float>() {
            GetHoverHeight(0, 0),                 // centre
            GetHoverHeight(-0.3f, -0.5f),         // fuselage left rear
            GetHoverHeight(-0.3f, 0),             // fuselage left side
            GetHoverHeight(-0.2f, 0.5f),          // fuselage left front quarter
            GetHoverHeight(-0.15f, 0.7f),         // fuselage left front
            GetHoverHeight(0.3f, -0.5f),          // fuselage right rear
            GetHoverHeight(0.3f, 0),              // fuselage right side
            GetHoverHeight(0.2f, 0.5f),           // fuselage right front quarter
            GetHoverHeight(0.15f, 0.7f)           // fuselage right front
        };

        // Record central height, for determining whether we can jump
        currentCentreHeight = results[0];

        // Find the average distance to the ground based on all collisions
        float totalDist = 0;
        int numCollisions = 0;
        foreach (float result in results) {
            if (result != Mathf.Infinity) {
                totalDist += result;
                numCollisions++;
            }
        }

        // If there were no collisions, register that the fuselage is high up
        if (numCollisions == 0) {
            isFuselageNearGround = false;
        }

        // Return the average collision distance
        if (numCollisions > 0) {
            isFuselageNearGround = true;
            return totalDist / numCollisions;
        }

        // No collisions!
        return Mathf.Infinity;
    }

    /**
     * Determines the current distance between some point of the player and the
     * ground.
     */
    private float GetHoverHeight(float xFactor, float zFactor) {
        Vector3 localRay = new Vector3(width * xFactor, 0, length * zFactor);
        Vector3 rayOrigin = transform.TransformPoint(localRay);
        return PhysicsHelper.DistanceToGround(rayOrigin, maxHeightPermittingJump);
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

        // Apply acceleration and jumping / gravity
        Vector3 acceleration = GetAcceleration() * transform.forward;
        Vector3 acceleratedVelocity = new Vector3(
                rigidbodyComponent.velocity.x + acceleration.x,
                yVelocity + GetVerticalVelocityModifier(),
                rigidbodyComponent.velocity.z + acceleration.z);

        // Determine lateral friction
        float lateralFriction = grounded
                ? FRICTION_LATERAL
                : AIR_FRICTION_LATERAL;

        // Determine forward friction
        float forwardFriction = 0;
        if (acceleration.magnitude == 0) {
            if (grounded) {
                forwardFriction = FRICTION_FORWARD;
            } else {
                forwardFriction = AIR_FRICTION_FORWARD;
            }
        }

        // Apply friction
        Vector3 velocityInLocalSpace =
                transform.InverseTransformDirection(acceleratedVelocity);
        velocityInLocalSpace = new Vector3(
                velocityInLocalSpace.x * (1 - lateralFriction),
                velocityInLocalSpace.y,
                velocityInLocalSpace.z * (1 - forwardFriction));
        acceleratedVelocity =
                transform.TransformDirection(velocityInLocalSpace);

        // Clamp vertical velocity
        float clampedVelocityY = Mathf.Clamp(
                acceleratedVelocity.y,
                PhysicsHelper.MAX_FALL_SPEED_Y,
                PhysicsHelper.MAX_JUMP_SPEED_Y
        );

        // Clamp horizontal velocity
        Vector2 horizontalVelocity = Vector2.ClampMagnitude(
                new Vector2(acceleratedVelocity.x, acceleratedVelocity.z),
                maxSpeed
        );

        // Put it all together
        return new Vector3(
                horizontalVelocity.x,
                clampedVelocityY,
                horizontalVelocity.y
        );
    }

    /**
     * Add a corrective slide motion to the given velocity vector.
     */
    private Vector3 ApplyCorrectiveSlide(Vector3 velocity) {
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        localVelocity.x += PhysicsHelper.SLIDE_MAGNITUDE * slideDirection;
        return transform.TransformDirection(localVelocity);
    }

    /**
     * Determines the acceleration to apply based on vertical input.
     */
    private float GetAcceleration() {
        return isRespawning ? 0 : Input.GetAxisRaw("Vertical") * acceleration;
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
        if (transform.position.y < respawnHeight) {
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
        rigidbodyComponent.velocity = Vector3.zero;

        // Inform listeners of the new position
        foreach (ICharacterListener listener in characterListeners) {
            listener.CharacterTeleported();
        }
    }

}
