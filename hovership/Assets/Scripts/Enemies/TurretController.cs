using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour {

    ///////////////////////////////////////////////////////////////////////////
    // Script Properties
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The turret's target. It must have a PlayerController script.
     */
    public GameObject target;

    /**
     * The game object to instantiate copies of when firing.
     */
    public GameObject projectilePrefab;

    /**
     * Turret fire rate, in shots per second.
     */
    public float fireRate;

    /**
     * Whether the turret should only aim along the horizon.
     */
    public bool horizontalOnly;

    ///////////////////////////////////////////////////////////////////////////
    // Accessors
    ///////////////////////////////////////////////////////////////////////////

    public Vector3 GetTargetedPosition() {
        return targetedPosition;
    }    

    ///////////////////////////////////////////////////////////////////////////
    // TurretController
    ///////////////////////////////////////////////////////////////////////////

    /**
     * The speed at which a fired projectile will leave the barrel.
     */
    private const float MUZZLE_VELOCITY = 50f;

    /**
     * The location at which the turret should aim this frame.
     */
    private Vector3 targetedPosition;

    /**
     * The target object's Rigidbody.
     */
    private Rigidbody targetRigidbody;

    /**
     * The target object's controller.
     */
    private PlayerController targetController;

    /**
     * Set up cron job to fire projectiles.
     */
    void Start() {
        InvokeRepeating("Fire", 0f, (1 / fireRate));
    }

    /**
     * Acquire target and compute point at which to aim.
     */
    void FixedUpdate() {
        // Acquire reference to target if necessary
        if (targetRigidbody == null) {
            targetController = target.GetComponent<PlayerController>();
            targetRigidbody = targetController.GetRigidbody();
        }

        targetedPosition = GetFireSolution();
    }

    /**
     * Calculate the position we should aim at to hit the target, if it continues
     * at present course and speed.
     */
    private Vector3 GetFireSolution() {
        return new Vector3(
            targetRigidbody.position.x,
            GetTargetY(),
            targetRigidbody.position.z
        );
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
        GameObject projectile = CreateProjectile();
        projectile.GetComponent<Rigidbody>().velocity = CalculateLaunchImpulse();
    }

    /**
     * Creates a projectile at the position of the turret.
     */
    private GameObject CreateProjectile() {
        return Instantiate(
            projectilePrefab,
            gameObject.transform.position,
            new Quaternion(0, 0, 0, 0)
        );
    }

    /**
     * Calculates the velocity vector to propel the projectile at the target
     * point.
     */
    private Vector3 CalculateLaunchImpulse() {
        return MUZZLE_VELOCITY * Vector3.Normalize(
            VectorUtils.GetResultant(gameObject.transform.position, targetedPosition));
    }
}
