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

    /**
     * The game object to instantiate copies of when firing.
     */
    public GameObject projectilePrefab;

    /**
     * Turret fire rate, in shots per second.
     */
    public float fireRate;

    ///////////////////////////////////////////////////////////////////////////
    // TurretBarrelController
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The speed at which a fired projectile will leave the barrel.
     */
    private const float MUZZLE_VELOCITY = 50f;

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
     * The position at which the turret is aiming this frame.
     */
    private Vector3 targetedPosition;

    /**
     * Initialises the controller.
     */
    void Start() {
        parentController = parent.GetComponent<TurretMantleController>();
        horizontalOnly = parentController.IsHorizontalOnly();
        InvokeRepeating("Fire", 0f, (1 / fireRate));
    }

    /**
     * Updates the barrel's aim every frame.
     */
    void FixedUpdate() {
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

        // Determine aim direction
        targetedPosition = new Vector3(
            targetRigidbody.position.x,
            GetTargetY(),
            targetRigidbody.position.z
        );
        
        // Aim barrel
        transform.LookAt(targetedPosition);
    }

    /**
     * Determines the Y-component of the target point we should aim at.
     */
    private float GetTargetY() {
        return horizontalOnly
                ? transform.position.y
                : Mathf.Clamp(
                        targetRigidbody.position.y,
                        transform.position.y - 0.5f,
                        float.MaxValue);
    }

    /**
     * Fires a copy of the stored projectile at the target position.
     */
    private void Fire() {
        // Create projectile
        GameObject projectile = CreateProjectile();

        // Apply launch impulse
        Rigidbody projectileBody = projectile.GetComponent<Rigidbody>();
        projectileBody.velocity = GetFireSolution();
    }

    /**
     * Creates a projectile at the position of the turret.
     */
    private GameObject CreateProjectile() {
        return Instantiate(
            projectilePrefab,
            parent.transform.position,
            new Quaternion(0, 0, 0, 0)
        );
    }

    /**
     * Calculates the velocity vector to propel the projectile at the target
     * point.
     */
    private Vector3 GetFireSolution() {
        return MUZZLE_VELOCITY * Vector3.Normalize(
            VectorUtils.GetResultant(parent.transform.position, targetedPosition));
    }
}
