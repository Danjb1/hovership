using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour {

    /**
     * Event handler for collisions.
     */
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            StateManager.Instance.SetState(GameState.CELEBRATING);
        }
    }

}
