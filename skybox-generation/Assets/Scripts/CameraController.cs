using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class CameraController : MonoBehaviour {

    public Camera front;
    public Camera right;
    public Camera back;
    public Camera left;
    public Camera up;
    public Camera down;

    void Start() {
        DisableAll();

        front.enabled = true;
    }

    void DisableAll() {
        front.enabled = false;
        right.enabled = false;
        back.enabled = false;
        left.enabled = false;
        up.enabled = false;
        down.enabled = false;
    }

    void Update() {
        if (Input.GetMouseButton(0) || Input.GetKey("enter")) {
            Capture(front, "front.png");
            Capture(right, "right.png");
            Capture(back, "back.png");
            Capture(left, "left.png");
            Capture(up, "up.png");
            Capture(down, "down.png");
            Application.Quit();
        }
    }

    // Based on:
    // https://forum.unity.com/threads/how-to-save-manually-save-a-png-of-a-camera-view.506269/
    //
    void Capture(Camera camera, string filename) {

        // Render to the Camera's texture
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;
        camera.Render();

        // Save texture to image
        Texture2D Image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        Image.Apply();
        var bytes = Image.EncodeToPNG();
        File.WriteAllBytes(filename, bytes);

        // Clean up
        Destroy(Image);
        RenderTexture.active = prev;
    }

}
