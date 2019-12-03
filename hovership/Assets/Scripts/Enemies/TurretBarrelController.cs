using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBarrelController : MonoBehaviour {

    /**
     * The turret's overall controller.
     */
    private TurretController parentController;

    /**
     * Updates the barrel's aim.
     */
    void FixedUpdate() {
        if (parentController == null) {
            parentController = GetParentController();
        }
        AimAtTarget();
    }

    /**
     * Gets a reference to the parent object's controller.
     */
    private TurretController GetParentController() {
        return transform.parent.gameObject.GetComponent<TurretController>();
    }

    /**
     * Aims the barrel at the designated target location.
     */
    private void AimAtTarget() {
        transform.LookAt(parentController.GetTargetedPosition());
    }
}
