using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Class responsible for global physics constants.
 */
public class Physics {

    // Gravity, in metres per second squared
    public const float GRAVITY = -0.5f;

    // Maximum player vertical speed, in metres per second
    public const float MAX_PLAYER_SPEED_Y = 15f;

    // Friction multiplier
    public const float FRICTION = 0.9f;

    // Minimum player vertical position before respawning, in metres
    public const float RESPAWN_Y = -25f;

}
