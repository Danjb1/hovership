using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerShardController : MonoBehaviour {

    public AudioClip pickUpSound;

    public int value;

    void Start() { }

    void Update() { }

    /**
     * Event handler for collisions.
     */
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            AudioSource.PlayClipAtPoint(pickUpSound, transform.position);
            gameObject.SetActive(false);
            StateManager.Instance.AddPowerShardsCollected(value);
        }
    }

}
