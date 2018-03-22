﻿using System;
using UnityEngine;

public class CameraController : MonoBehaviour, ICharacterListener {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    public GameObject player;

    /**
     * Height at which the camera should be positioned.
     */
    public float height;

    /**
     * Vertical offset from the player used to determine where the camera
     * should point.
     * 
     * This prevents the camera being angled too far downwards.
     */
    public float targetOffsetY;

    /*
     * Distance between the camera and the target.
     */
    public float distanceToTarget;

    ///////////////////////////////////////////////////////////////////////////
    // Movement Constants
    ///////////////////////////////////////////////////////////////////////////

    /**
     * Time taken to reach the vertical destination, in seconds.
     */
    private const float VERTICAL_MOVEMENT_TIME = 1.0f;

    /**
     * Minimum y-position before the camera will stop moving.
     */
    private const float MIN_Y = -5f;

    ///////////////////////////////////////////////////////////////////////////
    // CameraController
    ///////////////////////////////////////////////////////////////////////////
    
    /**
     * Last grounded Y of the player.
     */
    private float lastGroundedY;

    /**
     * Optimal Y-position we want to be looking at.
     */
    private float optimalTargetY;

    /**
     * Current Y-position we are looking at.
     */
    private float targetY;

    /**
     * Speed at which camera should move in the y-axis to reach optimalY.
     */
    private float speedY;

    /**
     * Initialises this controller.
     */
    void Start () {

        // Register this class as a CharacterListener for the Player
        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.RegisterCharacterListener(this);

        targetY = player.transform.position.y;
        PositionBehindPlayer();
    }
	
	/**
     * Move the Camera towards the optimal position.
     */
	void LateUpdate () {

        float prevTargetY = targetY;
        targetY += speedY * Time.deltaTime;
        
        // If we have gone past our destination...
        // (using XOR to check if signs are different)
        // Casting to ints is enormously imprecise => FIX THIS!
        if (((int)(prevTargetY - optimalTargetY) ^ (int)(targetY - optimalTargetY)) < 0) {
            // Camera has reached (or gone past) its destination
            targetY = optimalTargetY;
            speedY = 0;
        } else if (Mathf.Abs(prevTargetY - optimalTargetY) < Mathf.Abs(targetY - optimalTargetY)) {
            // Somehow we are moving further away from the optimal position!
            // This should never happen => FIX THIS!
            targetY = optimalTargetY;
            speedY = 0;
        }

        // Determine camera target for this frame
        Vector3 target = new Vector3(
                player.transform.position.x,
                targetY,
                player.transform.position.z
        );

        PositionBehindTarget(target, player.transform.forward);
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
        transform.position = new Vector3(
            target.x - forward.x * distanceToTarget,
            target.y - forward.y * distanceToTarget,
            target.z - forward.z * distanceToTarget);

        // Raise to the desired height
        float newY = Mathf.Max(transform.position.y + height, MIN_Y);
        transform.position = VectorUtils.SetY(transform.position, newY);

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
        optimalTargetY = lastGroundedY;

        // Calculate distance to optimal position
        float dy = optimalTargetY - targetY;

        // Calculate vertical speed required to reach optimal position in time
        speedY = dy / VERTICAL_MOVEMENT_TIME;
    }

}
