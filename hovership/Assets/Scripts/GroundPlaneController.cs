using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundPlaneController : MonoBehaviour {

    void Start() {
        // Register our ground plane height in the game state
        StateManager.Instance.SetLevelGroundHeight(transform.position.y);
    }
}
