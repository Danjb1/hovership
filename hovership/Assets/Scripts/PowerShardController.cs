using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerShardController : MonoBehaviour {

    public AudioClip pickUpSound;

    public int value;

    void Start() {
        // Register this PowerShard
        StateManager.Instance.numPowerShards++;
    }

    void Update() { }

    /**
     * Event handler for collisions.
     */
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            pickedUp();
        }
    }

    private void pickedUp() {

        // Play sound
        AudioSource.PlayClipAtPoint(pickUpSound, transform.position);

        // Disappear
        gameObject.SetActive(false);

        // Update Power Shard counter
        StateManager.Instance.AddPowerShardsCollected(value);
    }

}
