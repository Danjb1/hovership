﻿using System;
using UnityEngine;

public class CameraController : MonoBehaviour, ICharacterListener {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    public GameObject player;

    /**
     * Height at which the camera should be positioned, in metres.
     */
    public float height;

    /**
     * Vertical offset from the player used to determine where the camera
     * should point, in metres.
     *
     * This prevents the camera being angled too far downwards.
     */
    public float targetOffsetY;

    /*
     * Resting distance between the camera and the target, in metres.
     */
    public float distanceToTarget;

    /**
     * Multiple of the resting camera distance used to find maximum distance.
     */
    public float maxDistanceMultiplier;

    /**
     * Multiple of the resting camera distance used to find minimum distance.
     */
    public float minDistanceMultiplier;

    /**
     * Speed of rotation around player when celebrating, in degrees per second.
     */
    public float celebrationRotationSpeed;

    ///////////////////////////////////////////////////////////////////////////
    // CameraController
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The fraction (0-1) of the intervening arc angle made up per frame when
     * slerping behind the player.
     */
    private const float SLERP_INTERVAL = 0.25f;

    /**
     * Multiplier applied to the camera's velocity.
     */
    private const float SPEED_MULTIPLIER = 100;

    /**
     * Upper limit to the size of the velocity vector.
     */
    private const float SPEED_LIMIT = 15;

    /**
     * Time taken to reach the vertical destination, in seconds.
     */
    private const float VERTICAL_MOVEMENT_TIME = 0.3f;

    /**
     * Last grounded y-position of the player, in metres.
     */
    private float lastGroundedY;

    /**
     * Optimal Y-position we want to be looking at, in metres.
     */
    private float desiredTargetY;

    /**
     * Current Y-position we are looking at, in metres.
     */
    private float currentTargetY;

    /**
     * Speed at which camera should move in the y-axis to reach optimalY, in
     * metres per second.
     */
    private float speedY;

    /**
     * Maximum permissible distance to the target, in metres.
     */
    private float maxDistanceToTarget;

    /**
     * Minimum permissible distance to the target, in metres.
     */
    private float minDistanceToTarget;

    /**
     * Minimum Y-position before the camera will stop descending.
     */
    private float minY;

    /**
     * The camera collider's rigidbody component.
     */
    private Rigidbody rigidbodyComponent;

    /**
     * The camera collider's velocity during the last in-play frame.
     */
    private Vector3 previousVelocity;

    /**
     * Initialises this controller.
     */
    void Start () {

        // Register this class as a CharacterListener for the Player
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.RegisterCharacterListener(this);

        // Acquire camera body
        rigidbodyComponent = GetComponent<Rigidbody>();

        // Determine position boundaries
        minY = StateManager.Instance.GetLevelGroundHeight();
        maxDistanceToTarget = maxDistanceMultiplier * distanceToTarget;
        minDistanceToTarget = minDistanceMultiplier * distanceToTarget;

        // Adopt spawn-in position
        lastGroundedY = player.transform.position.y;
        currentTargetY = player.transform.position.y;
        CharacterTeleported();
    }

    /**
     * Move the Camera towards the optimal position.
     */
    void FixedUpdate() {

        // Rotate around the player, if we are celebrating a level win
        if (StateManager.Instance.GetState() == GameState.CELEBRATING) {
            transform.RotateAround(
                player.transform.position,
                Vector3.up,
                celebrationRotationSpeed * Time.deltaTime
            );
            return;
        } else if (StateManager.Instance.GetState() == GameState.PAUSED) {
            rigidbodyComponent.velocity = Vector3.zero;
            return;
        } else {
            rigidbodyComponent.velocity = previousVelocity;
        }

        // If the Player has fallen below their last grounded Y,
        // start tracking them again
        desiredTargetY = Math.Min(lastGroundedY, player.transform.position.y);

        // Calculate distance to optimal position
        float dy = desiredTargetY - currentTargetY;

        // Calculate vertical speed required to reach optimal position in time
        speedY = dy / VERTICAL_MOVEMENT_TIME;

        float prevTargetY = currentTargetY;

        // Follow the player when falling out of the world
        if (player.transform.position.y < minY) {
            SlerpToTarget(player.transform.position);
            return;
        }

        // If we are flying, track player's Y directly
        if (StateManager.Instance.IsFlightMode()) {
            currentTargetY = player.transform.position.y;
        } else {
            currentTargetY += speedY * Time.deltaTime;
        }

        // If we have gone past our destination, stop at the destination
        if (DestinationReached(desiredTargetY, prevTargetY, currentTargetY)) {
            currentTargetY = desiredTargetY;
            speedY = 0;
        }

        // Determine camera target for this frame
        Vector3 target = new Vector3(
                player.transform.position.x,
                currentTargetY,
                player.transform.position.z
        );

        SlerpToOptimalFollowPosition(target, player.transform.forward);
        LookAtTarget(target);
    }

    /**
     * Determines if a change in a value causes it to equal or exceed some target.
     */
    public bool DestinationReached(float destination, float prev, float current) {
        return current == destination ||
            ((prev < destination && current > destination) ||
                (prev > destination && current < destination));
    }

    /**
     * Gradually rotate towards a given target.
     */
    public void SlerpToTarget(Vector3 target) {
        transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(target - transform.position),
                SLERP_INTERVAL / 2);
    }

    /**
     * Gradually move towards the optimal position for a given target.
     */
    public void SlerpToOptimalFollowPosition(Vector3 target, Vector3 forward) {

        // Determine vectors used in slerp
        Vector3 optimalPosition = GetOptimalPosition(target, forward);
        Vector3 targetToCamera = transform.position - target;
        Vector3 targetToOptimalCamera = optimalPosition - target;

        // Determine new position somewhere between current and optimal
        Vector3 newPos = target + Vector3.Slerp(
                targetToCamera, targetToOptimalCamera, SLERP_INTERVAL);

        // Correct for over-the-pole slerp movement
        newPos = VectorUtils.SetY(newPos, optimalPosition.y);

        // Determine acceleration towards new position
        Vector3 toNewPos = newPos - transform.position;
        float dist = toNewPos.magnitude;
        Vector3 newVelocity = toNewPos * dist * SPEED_MULTIPLIER;

        // Limit velocity, to avoid camera jerkiness
        newVelocity = Vector3.ClampMagnitude(newVelocity, SPEED_LIMIT);

        // Apply acceleration towards new position
        rigidbodyComponent.velocity = newVelocity;

        // Store new velocity
        previousVelocity = newVelocity;

        /*
         * Check if the camera is too close to or far from the player.
         * N.B. this happens because the slerp is taking place over an ellipsoid
         * surface, not a sphere, when player is moving.
         */
        Vector2 flatPos = VectorUtils.Flatten(transform.position);
        Vector2 flatTargetPos = VectorUtils.Flatten(target);
        Vector2 flatPath = VectorUtils.GetResultant(flatPos, flatTargetPos);
        float desiredDistance;

        if (flatPath.magnitude > maxDistanceToTarget) {
            desiredDistance = maxDistanceToTarget;
        } else if (flatPath.magnitude < minDistanceToTarget) {
            desiredDistance = minDistanceToTarget;
        } else {
            return;
        }

        // Set follow distance to desired value
        CorrectFollowDistance(
                flatPath,
                VectorUtils.Flatten(target),
                desiredDistance,
                optimalPosition.y
        );
    }

    /**
     * Move the camera back or forward in its current angle to a given distance
     * from some target, at a given height.
     */
    private void CorrectFollowDistance(
            Vector2 arrow, Vector2 target, float distance, float height) {
        Vector2 newPos = VectorUtils.Backtrack(arrow, target, distance);
        transform.position = VectorUtils.Extrude(newPos, height);
    }

    /**
     * Positions the camera behind the player.
     */
    public void PositionBehindPlayer() {
        PositionBehindTarget(player.transform.position, player.transform.forward);
    }

    /**
     * Positions the camera behind the given target.
     *
     * The notion of "behind" is determined by the given forward vector.
     */
    private void PositionBehindTarget(Vector3 target, Vector3 forward) {

        // Position behind the target
        transform.position = GetOptimalPosition(target, forward);

        LookAtTarget(target);
    }

    /**
     * Determines the optimal camera position for the given target.
     */
    private Vector3 GetOptimalPosition(Vector3 target, Vector3 forward) {

        // Start off with the desired distance behind the target
        Vector3 optimalPos = new Vector3(
               target.x - forward.x * distanceToTarget,
               target.y - forward.y * distanceToTarget,
               target.z - forward.z * distanceToTarget
        );

        // Raise to the desired height, keeping within the limits
        float newY = Mathf.Max(optimalPos.y + height, minY);
        optimalPos = VectorUtils.SetY(optimalPos, newY);

        return optimalPos;
    }

    /**
     * Rotates the camera to look at a target, respecting the desired offset.
     */
    private void LookAtTarget(Vector3 target) {

        // Adjust target based on y-offset
        target = VectorUtils.SetY(target, target.y + targetOffsetY);

        // Face the target
        transform.LookAt(target);
    }

    /**
     * Called whenever the Player lands.
     */
    public void CharacterLanded() {
        lastGroundedY = player.transform.position.y;
    }

    public void CharacterTeleported() {

        // Just snap to the new position
        PositionBehindPlayer();

        // Reset the camera's destination and speed
        currentTargetY = player.transform.position.y;
        desiredTargetY = currentTargetY;
        speedY = 0;
    }

}
