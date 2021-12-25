using UnityEngine;
public static class OurLib{
    // Returns the vector with it's y-coordinate set to 0.
    public static Vector3 horizontal(Vector3 vec) => new Vector3(vec.x, 0, vec.z);
}