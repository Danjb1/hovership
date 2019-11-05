using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelController : MonoBehaviour {

    /**
     * Multiplier applied to the player's rotation speed to determine our tilt
     * angle.
     */
    private float tiltFactor;

    private PlayerController playerCtrl;

    void Start () {
        playerCtrl = transform.parent.GetComponent<PlayerController>();
    }
	
	void LateUpdate () {

        // Determine our desired z-rotation
        tiltFactor = playerCtrl.IsInFlightMode() ? 0.25f : 0.1f;
        float rotZ = -playerCtrl.GetRotationSpeed() * tiltFactor;

        // Apply the rotation
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            rotZ
        );
    }

}
