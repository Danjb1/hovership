using UnityEngine;
using NUnit.Framework;

public class CameraTest {

    private GameObject camera;
    private GameObject player;
    private CameraController cameraCtrl;

    [SetUp]
    public void Init() {
        camera = GameObject.Find("Main Camera");
        player = GameObject.Find("Player");
        cameraCtrl = camera.GetComponent<CameraController>();
    }

    [Test]
    public void PositionBehindPlayer() {

        cameraCtrl.PositionBehindPlayer();

        float expectedX = player.transform.position.x -
                player.transform.forward.x * cameraCtrl.distanceToTarget;
        float expectedY = player.transform.position.y +
                cameraCtrl.height;
        float expectedZ = player.transform.position.z -
                player.transform.forward.z * cameraCtrl.distanceToTarget;

        Assert.AreEqual(expectedX, camera.transform.position.x);
        Assert.AreEqual(expectedY, camera.transform.position.y);
        Assert.AreEqual(expectedZ, camera.transform.position.z);
    }
    
}