using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GeometricClass
{
    public struct Segment
    {
        public Vector3 pt1;
        public Vector3 pt2;
    }

    public struct Cylindre
    {
        public Vector3 pt1;
        public Vector3 pt2;
        public float radius;
    }

    public struct Plane
    {
        public Vector3 Normal;
        public float d;
    }
    public struct Circle
    {
        public Vector3 center;
        public float radius;
    }
}
