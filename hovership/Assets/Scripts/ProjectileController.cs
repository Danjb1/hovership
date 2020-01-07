using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour {
    
    /**
     * The GameObjects that compose the object that fired this projectile.
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
     * Projectile's Rigidbody component.
     */
    private Rigidbody rigidbodyComponent;

    /**
     * The projectile's velocity during in-play frames.
     */
    private Vector3 flightVelocity;

    void Start() {
        rigidbodyComponent = GetComponent<Rigidbody>();
        creationTime = Time.fixedTime;
        flightVelocity = rigidbodyComponent.velocity;
    }

    void FixedUpdate() {

        // Skip all updates if game is paused
        if (StateManager.Instance.GetState() != GameState.PLAYING) {
            rigidbodyComponent.velocity = Vector3.zero;
            return;
        } else {
            rigidbodyComponent.velocity = flightVelocity;
        }

        // Decay projectile if it has been in flight too long
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
