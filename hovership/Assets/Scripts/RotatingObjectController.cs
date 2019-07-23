using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObjectController : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The rotation speed of the object, in degrees per second.
     */
    public float rotationSpeed;

    ///////////////////////////////////////////////////////////////////////////
    // RotatingObjectController
    ///////////////////////////////////////////////////////////////////////////

    void Update () {

        // Do nothing if we are celebrating, or the game is paused
        GameState state = StateManager.Instance.GetState();
        if (state == GameState.CELEBRATING || state == GameState.PAUSED) {
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
