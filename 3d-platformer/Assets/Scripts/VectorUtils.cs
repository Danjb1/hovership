using UnityEngine;

public class VectorUtils {

    public static Vector3 SetX(Vector3 v, float x) {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 SetY(Vector3 v, float y) {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 SetZ(Vector3 v, float z) {
        return new Vector3(v.x, v.y, z);
    }

    /**
     * Flatten a given Vector3 to a Vector2, discarding the Y-component.
     */
    public static Vector2 Flatten(Vector3 v) {
        return new Vector2(v.x, v.z);
    }

    /**
     * Extrude a given Vector2 to a Vector3 using some Y value.
     */
    public static Vector3 Extrude(Vector2 v, float y) {
        return new Vector3(v.x, y, v.y);
    }

    /**
     * Calculate the 2D resultant vector A->B given vectors A and B
     */
    public static Vector2 GetResultant(Vector2 a, Vector2 b) {
        return new Vector2(b.x - a.x, b.y - a.y);
    }

    /**
     * Determine what position a given arrow vector pointing at a given
     * destination would originate from if scaled to a given length, anchored
     * at the destination.
     */
    public static Vector2 BacktrackVector(
            Vector2 arrow, Vector2 destination, float length) {

        float scaleFactor = length/arrow.magnitude;
        float dX = arrow.x * scaleFactor;
        float dY = arrow.y * scaleFactor;

        return new Vector2(destination.x - dX, destination.y - dY);
    }

}
