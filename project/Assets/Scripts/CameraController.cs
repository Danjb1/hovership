using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

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

    private float lastGroundedY;

    // Use this for initialization
    void Start () {
        playerPhysics = player.GetComponent<CharacterController>();
        PositionBehindPlayer();
    }
	
	// Update is called once per frame
	void LateUpdate () {

        // Remember last grounded height
        if (playerPhysics.isGrounded) {
            lastGroundedY = player.transform.position.y;
        }

        // Determine camera target
        Vector3 target = new Vector3(
                player.transform.position.x,
                lastGroundedY,
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

        // Position behind the player
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
