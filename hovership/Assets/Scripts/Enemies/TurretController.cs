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
     * The speed at which a fired projectile will leave the barrel, in metres
     * per second.
     */
    public float muzzleVelocity;

    /**
     * The maximum inaccuracy ratio permitted, where 0.1 represents ±10%.
     */
    public float inaccuracySize;

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
     * The location at which the turret should aim this frame.
     */
    private Vector3 targetedPosition;

    /**
     * The target object's Rigidbody.
     */
    private Rigidbody targetRigidbody;

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
        if (targetRigidbody == null) {
            targetRigidbody = GetTargetRigidbody();
        }
        targetedPosition = GetFireSolution();
    }

    /**
     * Gets a reference to the target object's rigidbody.
     */
    private Rigidbody GetTargetRigidbody() {
        PlayerController targetController = target.GetComponent<PlayerController>();
        return targetController.GetRigidbody();
    }

    /**
     * Calculates the position we should aim at to hit the target using dead
     * reckoning.
     */
    private Vector3 GetFireSolution() {
        float range = VectorUtils.GetResultant(
                gameObject.transform.position, targetedPosition).magnitude;
        float transitTime = range / muzzleVelocity;
        Vector3 projectedPosition =
                targetRigidbody.position + targetRigidbody.velocity * transitTime;
        return PreventExcessDepression(projectedPosition);
    }

    /**
     * Adjusts a given target position vector to prevent the turret aiming down
     * through its own mantle.
     */
    private Vector3 PreventExcessDepression(Vector3 vector) {
        return VectorUtils.SetY(
            vector,
            Mathf.Clamp(vector.y, transform.position.y - 0.5f, float.MaxValue)
        );
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

        // Create projectile at turret position
        GameObject projectile = Instantiate(
            projectilePrefab,
            gameObject.transform.position,
            new Quaternion(0, 0, 0, 0)
        );

        // Register this turret and its components on the projectile
        projectile.GetComponent<ProjectileController>().originObjects = GetTurretObjects();

        return projectile;
    }

    /**
     * Gets the turret's object, and all of its child objects.
     */
    private List<GameObject> GetTurretObjects() {
        List<GameObject> objects = new List<GameObject>{gameObject};
        foreach(Transform child in transform) {
            objects.Add(child.gameObject);
        }
        return objects;
    }

    /**
     * Calculates the velocity vector to propel the projectile at the target
     * point.
     */
    private Vector3 CalculateLaunchImpulse() {
        return AddRandomness(muzzleVelocity * Vector3.Normalize(
            VectorUtils.GetResultant(gameObject.transform.position, targetedPosition)));
    }

    /**
     * Adds uncertainty to a vector, according to the stored ratio.
     */
    private Vector3 AddRandomness(Vector3 vector) {
        float minRatio = 1 - inaccuracySize;
        float maxRatio = 1 + inaccuracySize;
        return new Vector3(
            vector.x * Random.Range(minRatio, maxRatio),
            vector.y * Random.Range(minRatio, maxRatio),
            vector.z * Random.Range(minRatio, maxRatio)
        );
    }
}
