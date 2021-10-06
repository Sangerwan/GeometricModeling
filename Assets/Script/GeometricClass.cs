using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeometricClass
{
    public struct Segment
    {
        Vector3 pt1;
        Vector3 pt2;
    }

    public struct Plane
    {
        Vector3 norm;
        float d;
    }
    public struct Sphere
    {
        Vector3 center;
        float radius;
    }
}
