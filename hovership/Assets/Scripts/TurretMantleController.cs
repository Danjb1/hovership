using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMantleController : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The turret's target. It must have a PlayerController script.
     */
    public GameObject target;

    /**
     * Whether the turret should only aim along the horizon.
     */
    public bool horizontalOnly;

    ///////////////////////////////////////////////////////////////////////////
    // Accessors
    ///////////////////////////////////////////////////////////////////////////

    public Rigidbody GetTargetRigidbody() {
        return targetRigidbody;
    }

    public bool IsHorizontalOnly() {
        return horizontalOnly;
    }
    
    ///////////////////////////////////////////////////////////////////////////
    // TurretController
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The target object's Rigidbody.
     */
    private Rigidbody targetRigidbody;

    /**
     * The target object's PlayerController.
     */
    private PlayerController targetController;
    
    /**
     * Updates the turret's aim every frame.
     */
    void FixedUpdate() {
        AimAtTarget();
    }

    /**
     * Aims the turret at the target GameObject.
     */
    private void AimAtTarget() {

        // Acquire reference to target if necessary
        if (targetRigidbody == null) {
            targetController = target.GetComponent<PlayerController>();
            targetRigidbody = targetController.GetRigidbody();
        }

        // Aim at target in the horizontal plane
        transform.LookAt(VectorUtils.SetY(
                targetRigidbody.position, transform.position.y));
    }
}
