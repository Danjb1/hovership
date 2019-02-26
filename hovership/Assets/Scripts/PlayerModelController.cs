using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelController : MonoBehaviour {

    /**
     * Multiplier applied to the player's rotation speed to determine our tilt
     * angle.
     */
    private const float TILT_FACTOR = 0.1f;

    private PlayerController playerCtrl;

    void Start () {
        playerCtrl = transform.parent.GetComponent<PlayerController>();
    }
	
	void LateUpdate () {

        // Determine our desired z-rotation
        float rotZ = -playerCtrl.GetRotationSpeed() * TILT_FACTOR;

        // Apply the rotation
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            rotZ
        );
    }

}
