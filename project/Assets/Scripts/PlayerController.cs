using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;
    public float rotSpeed;

    // Use this for initialization
    void Start () {

	}

    // Update is called once per frame
    void Update() {
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;

        // Don't allow backwards movement
        z = Mathf.Max(z, 0.0f);

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
    }

}
