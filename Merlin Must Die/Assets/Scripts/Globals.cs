using UnityEngine;
using System;
using UnityEngine.InputSystem;
public static class Globals
{

    /// <summary>
    /// Returns the angle in degrees a vector would be pointing in.
    /// </summary>
    public static float GetAngleInDegFromVec(Vector2 vector)
    {
        float angle = (float)(Math.Atan2(vector.y, vector.x) * 57.295); //convert from radians to degrees
        //Converts range from -180 to 180 to 0 to 360.
        if (angle < 0)
        {
            angle = 180 + (180 + angle);
        }
        return angle;
    }
}
