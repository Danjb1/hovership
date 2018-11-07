using System;
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

    private const float SLERP_INTERVAL = 0.2f;

    /**
     * Time taken to reach the vertical destination, in seconds.
     */
    private const float VERTICAL_MOVEMENT_TIME = 0.4f;

    /**
     * Minimum y-position before the camera will stop moving.
     * 
     * TODO: Make this dependent on player size.
     */
    private const float MIN_Y = 0.4999f;

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

    void Update() {

        // Follow the player when falling out of the world
        if (player.transform.position.y < MIN_Y) {
            targetY = player.transform.position.y;
            optimalTargetY = targetY;
        }
    }

    /**
     * Move the Camera towards the optimal position.
     */
    void LateUpdate () {

        float prevTargetY = targetY;
        targetY += speedY * Time.deltaTime;
        
        // If we have gone past our destination, stop at the destination
        if ((prevTargetY < optimalTargetY && targetY > optimalTargetY) ||
                (prevTargetY > optimalTargetY && targetY < optimalTargetY)) {
            targetY = optimalTargetY;
            speedY = 0;
        }
        
        // Determine camera target for this frame
        Vector3 target = new Vector3(
                player.transform.position.x,
                targetY,
                player.transform.position.z
        );

        SlerpToOptimalFollowPosition(target, player.transform.forward);
        LookAtTarget(target);
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
        Vector3 newPos = target + Vector3.Slerp(targetToCamera, targetToOptimalCamera, SLERP_INTERVAL);

        // Correct for over-the-pole slerp movement
        transform.position = new Vector3(newPos.x, optimalPosition.y, newPos.z);
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
        float newY = Mathf.Max(optimalPos.y + height, MIN_Y);
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
        optimalTargetY = lastGroundedY;

        // Calculate distance to optimal position
        float dy = optimalTargetY - targetY;

        // Calculate vertical speed required to reach optimal position in time
        speedY = dy / VERTICAL_MOVEMENT_TIME;
    }

    public void CharacterTeleported() {

        // Just snap to the new position
        targetY = player.transform.position.y;
        optimalTargetY = targetY;
    }

}
