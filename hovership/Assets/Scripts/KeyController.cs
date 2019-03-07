using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyController : MonoBehaviour {

    /**
     * The scale factor for the speed of the key's rotation.
     */
    public float rotationSpeed;

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
