using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModelController : MonoBehaviour {

    private PlayerController playerCtrl;

    void Start () {
        playerCtrl = transform.parent.GetComponent<PlayerController>();
    }
	
	void LateUpdate () {

        // Determine our desired z-rotation
        float tiltFactor = StateManager.Instance.IsFlightMode() ? 0.25f : 0.1f;
        float rotZ = -playerCtrl.GetRotationSpeed() * tiltFactor;

        // Apply the rotation
        transform.eulerAngles = new Vector3(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            rotZ
        );
    }

}
