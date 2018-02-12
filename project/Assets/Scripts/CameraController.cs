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
     * Distance between the camera and the player.
     */
    public float distanceToPlayer;

    /**
     * Minimum y-position before the camera will stop moving.
     */
    private const float MIN_Y = -5f;

    // Use this for initialization
    void Start () {
        PositionBehindPlayer();
    }
	
	// Update is called once per frame
	void LateUpdate () {
        PositionBehindPlayer();
    }

    private void PositionBehindPlayer() {

        // Determine camera position based on player facing
        Vector3 forward = player.transform.forward;

        // Position behind the player
        transform.position = player.transform.position - forward * distanceToPlayer;

        // Raise to the desired height
        float newY = transform.position.y + height;
        newY = Mathf.Max(newY, MIN_Y);
        transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
        );

        // Determine target based on player position and offset
        Vector3 target = new Vector3(
                player.transform.position.x,
                player.transform.position.y + targetOffsetY,
                player.transform.position.z
        );

        // Face the target
        transform.LookAt(target);
    }

}
