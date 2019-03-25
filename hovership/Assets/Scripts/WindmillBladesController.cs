using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindmillBladesController : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The rotation speed of the blades, in degrees per second.
     */
    public float rotationSpeed;

    ///////////////////////////////////////////////////////////////////////////
    // WindmillBladesController
    ///////////////////////////////////////////////////////////////////////////

    void Update () {

        // Do nothing if we are celebrating a level completion
        if (StateManager.Instance.GetState() == GameState.CELEBRATING) {
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
