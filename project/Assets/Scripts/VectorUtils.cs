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

}
