using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetectorController : MonoBehaviour {

    private IGroundedListener listener;

    /**
     * Initialises this controller.
     */
    void Start() {
        listener = (IGroundedListener)
                transform.parent.gameObject.GetComponent<PlayerController>();
    }

    void Update() {
    }

    private void OnTriggerEnter(Collider other) {
        listener.SetGrounded(true);
    }

    private void OnTriggerStay(Collider other) {
        listener.SetGrounded(true);
    }

    private void OnTriggerExit(Collider other) {
        listener.SetGrounded(false);
    }

}
