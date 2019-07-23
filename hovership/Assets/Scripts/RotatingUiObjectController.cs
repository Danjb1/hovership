using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingUiObjectController : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The rotation speed of the object, in degrees per second.
     */
    public float rotationSpeed;

    ///////////////////////////////////////////////////////////////////////////
    // RotatingUiObjectController
    ///////////////////////////////////////////////////////////////////////////

    void Update () {
        Vector3 rotation = Vector3.up * rotationSpeed;
        transform.Rotate(
                rotation.x * Time.deltaTime,
                rotation.y * Time.deltaTime,
                rotation.z * Time.deltaTime
        );
    }

}
