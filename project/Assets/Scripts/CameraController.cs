using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    /**
     * Time taken to reach the vertical destination, in seconds.
     */
    private const float VERTICAL_MOVEMENT_TIME = 1.0f;

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

    /**
     * Minimum y-position before the camera will stop moving.
     */
    private const float MIN_Y = -5f;

    private CharacterController playerPhysics;

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

    private bool verticalSpeedSet;

    // Use this for initialization
    void Start () {
        playerPhysics = player.GetComponent<CharacterController>();
        targetY = player.transform.position.y;
        PositionBehindPlayer();
    }
	
	// Update is called once per frame
	void LateUpdate () {

        // Remember last grounded height
        if (playerPhysics.isGrounded) {

            // Do this just once every time we're grounded!
            if (!verticalSpeedSet) {

                lastGroundedY = player.transform.position.y;

                optimalTargetY = lastGroundedY;

                // Calculate distance to optimal position
                float dy = optimalTargetY - targetY;

                // Calculate vertical speed required to reach optimal position in time
                speedY = dy / VERTICAL_MOVEMENT_TIME;

                verticalSpeedSet = true;
            }
        } else {
            verticalSpeedSet = false;
        }

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
    private void PositionBehindPlayer() {
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
        float newY = transform.position.y + height;
        newY = Mathf.Max(newY, MIN_Y);
        transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
        );

        // Adjust target based on y-offset
        target = new Vector3(
                target.x,
                target.y + targetOffsetY,
                target.z
        );

        // Face the target
        transform.LookAt(target);
    }

}
