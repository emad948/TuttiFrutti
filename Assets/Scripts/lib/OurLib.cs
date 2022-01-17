using UnityEngine;
using System;
public static class OurLib{
    // Returns the vector with it's y-coordinate set to 0.
    public static Vector3 horizontal(Vector3 vec) => new Vector3(vec.x, 0, vec.z);
    public static string xAfterDot(float num, int x)
    { // example (12.5, 2) -> 12.50 
        String placeHoldersAfterDot = String.Empty;
        while(x-- > 0) placeHoldersAfterDot +="0";
        
        return String.Format("{0:0." + placeHoldersAfterDot + "}", num);
    }
}