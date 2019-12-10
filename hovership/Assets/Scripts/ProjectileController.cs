using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
    
    /**
     * The object that fired this projectile.
     */
    public List<GameObject> originObjects;

    /**
     * The sound to play when the projectile hits something.
     */
    public AudioClip impactSound;

    /**
     * The length of time in seconds that the projectile will fly before being
     * destroyed.
     */
    private const int MAX_DURATION = 5;

    /**
     * The number of seconds since the game began at the instant of the projectile's
     * creation.
     */
    private float creationTime;

    /**
     * Logs the time of the projectile's creation.
     */
    void Start() {
        creationTime = Time.fixedTime;
    }

    /**
     * Checks whether the projectile has flown for too long and should therefore
     * decay.
     */
    void FixedUpdate() {
        if (Time.fixedTime > creationTime + MAX_DURATION) {
            Destroy(gameObject);
        }
    }

    /**
     * Trigger collision handler.
     */
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            collider.gameObject.GetComponent<IHittable>().RegisterHit();
            Impact();
        } else if (!originObjects.Contains(collider.gameObject)) {
            Impact();
        }
    }

    /**
     * Handles the projectile hitting something solid.
     */
    private void Impact() {
        AudioSource.PlayClipAtPoint(impactSound, transform.position);
        Destroy(gameObject);
    }
}
