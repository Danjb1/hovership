using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject player;

    public float height;
    public float distanceToPlayer;

    // Use this for initialization
    void Start () {
        PositionBehindPlayer();
    }
	
	// Update is called once per frame
	void LateUpdate () {
        PositionBehindPlayer();
    }

    private void PositionBehindPlayer() {

        // Determine camera position based on player facing
        Vector3 forward = player.transform.forward;

        // Position behind the player and face him
        transform.position = player.transform.position - forward * distanceToPlayer;
        transform.LookAt(player.transform.position);

        // Raise to the desired height
        transform.position = new Vector3(
                transform.position.x,
                transform.position.y + height,
                transform.position.z
        );

    }

}
