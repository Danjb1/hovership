using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class responsible for global physics constants.
 */
public class PhysicsHelper {

    // Gravity, in metres per second squared
    public const float GRAVITY = -0.5f;

    // Maximum player vertical speed, in metres per second
    public const float MAX_PLAYER_SPEED_Y = 15f;

    // Ground friction multiplier
    public const float FRICTION = 0.9f;

    // Air friction multiplier
    public const float AIR_FRICTION = 0.95f;

}
