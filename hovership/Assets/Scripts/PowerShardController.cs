using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerShardController : MonoBehaviour {

    public int value;

    void Start() { }

    void Update() { }

    /**
     * Event handler for collisions.
     */
    private void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.tag == "Player") {
            gameObject.SetActive(false);
            StateManager.Instance.AddPowerShardsCollected(value);
        }
    }

}
