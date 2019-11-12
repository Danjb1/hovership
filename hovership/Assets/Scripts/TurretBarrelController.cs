using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBarrelController : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The turret mantle this belongs to.
     */
    public GameObject parent;

    ///////////////////////////////////////////////////////////////////////////
    // TurretBarrelController
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The controller of the turrent mantle which aims this barrel.
     */
    private TurretMantleController parentController;

    /**
     * The rigidbody of the turret's target object.
     */
    private Rigidbody targetRigidbody;

    /**
     * Whether the barrel should remain level.
     */
    private bool horizontalOnly;

    /**
     * Initialises the controller.
     */
    void Start() {
        parentController = parent.GetComponent<TurretMantleController>();
        horizontalOnly = parentController.IsHorizontalOnly();
    }

    /**
     * Updates the barrel's aim every frame.
     */
    void Update() {
        AimAtTarget();
    }

    /**
     * Aims the barrel vertically at the parent mantle's target.
     */
    private void AimAtTarget() {

        // Get target rigidbody if necessary
        if (targetRigidbody == null) {
            targetRigidbody = parentController.GetTargetRigidbody();
        }
        
        // Apply rotation to aim at point
        transform.LookAt(new Vector3(
            targetRigidbody.position.x,
            GetTargetY(),
            targetRigidbody.position.z
        ));
    }

    /**
     * Determine the Y-component of the target point we should aim at.
     */
    private float GetTargetY() {
        return horizontalOnly
                ? transform.position.y
                : Mathf.Clamp(
                        targetRigidbody.position.y,
                        transform.position.y - 0.5f,
                        float.MaxValue);
    }
}
