using UnityEngine;

/**
 * Class responsible for global physics constants.
 */
public class PhysicsHelper {

    // Gravity, in metres per second squared
    public const float GRAVITY = -0.5f;

    // Maximum fall speed, in metres per second
    public const float MAX_FALL_SPEED_Y = -15f;

    // Maximum jump speed, in metres per second
    public const float MAX_JUMP_SPEED_Y = 7.5f;

    // Ground friction multiplier
    public const float FRICTION = 0.9f;

    // Air friction multiplier
    public const float AIR_FRICTION = 0.95f;

    /**
     * The maximum gradient of a walkable surface.
     * 
     * This prevents entities being able to climb up very steep slopes, or
     * even walls.
     */
    private const float MAX_SLOPE_GRADIENT = 0.5f;

    /**
     * Determines the current distance between some position and the ground.
     */
    public static float DistanceToGround(Vector3 position, float max) {
        RaycastHit hit;
        if (Physics.Raycast(position, -Vector3.up, out hit, max)) {
            if (hit.normal.y >= MAX_SLOPE_GRADIENT) {
                return hit.distance;
            }
        }
        return Mathf.Infinity;
    }

}
