﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The rotation speed of the key, in degrees per second.
     */
    public float rotationSpeed;

    ///////////////////////////////////////////////////////////////////////////
    // KeyController
    ///////////////////////////////////////////////////////////////////////////

    void Update () {

        // Do nothing if we are celebrating a level completion
        if (StateManager.Instance.gameState == GameState.CELEBRATING) {
            return;
        }

        Vector3 rotation = Vector3.up * rotationSpeed;
        transform.Rotate(
                rotation.x * Time.deltaTime,
                rotation.y * Time.deltaTime,
                rotation.z * Time.deltaTime
        );
    }

}
