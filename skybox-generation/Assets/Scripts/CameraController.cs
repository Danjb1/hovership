using UnityEditor;
using UnityEngine;

// Based on:
// https://medium.com/acrossthegalaxy/making-skyboxes-directly-in-unity-7250fe90c632
public class CameraController : MonoBehaviour {

    const int TEXTURE_SIZE = 1024;

    private Camera camera;

    private void Start() {
        camera = GetComponent<Camera>();
    }

    void Update() {
        if (Input.GetKey("a") || Input.GetKey("left")) {
            camera.transform.Rotate(new Vector3(0, -1, 0));
        } else if (Input.GetKey("d") || Input.GetKey("right")) {
            camera.transform.Rotate(new Vector3(0, 1, 0));
        } else if (Input.GetKeyUp("return")) {
            Capture();
        }
    }

    void Capture() {
        Cubemap cubemap = new Cubemap(TEXTURE_SIZE, TextureFormat.ARGB32, true);
        cubemap.name = "Skybox";
        camera.RenderToCubemap(cubemap);

        AssetDatabase.CreateAsset(
          cubemap,
          "Assets/Textures/Skybox.cubemap"
        );
    }

}
