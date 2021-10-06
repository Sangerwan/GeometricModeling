using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeometricClass
{
    struct Segment
    {
        Vector3 pt1;
        Vector3 pt2;
    }

    struct Plane
    {
        Vector3 norm;
        float d;
    }
    struct Sphere
    {
        Vector3 center;
        float radius;
    }
}
