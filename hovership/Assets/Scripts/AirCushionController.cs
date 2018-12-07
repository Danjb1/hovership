using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCushionController : MonoBehaviour {

    private const float MIN_GRADIENT = 0.5f;

    /**
     * The air cushion's rigid body.
     */
    private Rigidbody rigidbodyComponent;

    private IAirCushionListener listener;

    private Vector3 expectedFallPosition;

    void Start() {

        listener = (IAirCushionListener) transform.parent.GetComponent<PlayerController>();
        rigidbodyComponent = GetComponent<Rigidbody>();

        // Ignore collisions with the parent!
        Physics.IgnoreCollision(
            transform.parent.GetComponent<Collider>(),
            GetComponent<Collider>()
        );
    }
	
	void FixedUpdate() {

        // Position at parent each frame
        transform.position = transform.parent.position;

        // Reset velocity
        rigidbodyComponent.velocity = Vector3.zero;

        // Accelerate cushion with gravity
        Vector3 moveVector = new Vector3(0, PhysicsHelper.GRAVITY, 0);
        rigidbodyComponent.AddForce(moveVector, ForceMode.VelocityChange);
	}

    void OnCollisionEnter(Collision collision) {
        HandleCollision(collision);
    }

    void OnCollisionStay(Collision collision) {
        HandleCollision(collision);
    }

    private void OnCollisionExit(Collision collision) {
        listener.AirCushionCollisionExit();
    }

    void HandleCollision(Collision collision) {

        float minSeparation = 0;

        foreach (ContactPoint point in collision.contacts) {

            if (point.normal.y <= MIN_GRADIENT) {
                // Don't collide with very steep surfaces (or walls!)
                continue;
            }

            // Remember the deepest collision
            minSeparation = Mathf.Min(minSeparation, point.separation);
        }

        if (minSeparation < 0) {
            float depth = Mathf.Abs(minSeparation);
            listener.AirCushionCollided(depth);
        }
    }

}
