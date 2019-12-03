using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMantleController : MonoBehaviour {

    /**
     * The turret's overall controller.
     */
    private TurretController parentController;
    
    /**
     * Rotates the mantle with the barrels.
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
        return transform
                .parent
                .gameObject
                .GetComponent<TurretController>();
    }

    /**
     * Traverses mantle to face target.
     */
    private void AimAtTarget() {
        transform.LookAt(
            VectorUtils.SetY(
                parentController.GetTargetedPosition(),
                transform.position.y)
        );
    }
}
