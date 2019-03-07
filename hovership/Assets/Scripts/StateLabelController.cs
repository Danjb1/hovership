using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateLabelController : MonoBehaviour {

    void Start() {
        SetVisible(false);
    }

    void Update() {
        if (StateManager.Instance.gameState == GameState.CELEBRATING) {
            SetVisible(true);
        } else {
            SetVisible(false);
        }
    }

    void SetVisible(bool visible) {
        // TODO
    }

}
