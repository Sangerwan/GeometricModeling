using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeometricClass;
using static GeometricServices;
public class TestManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 A = new Vector3(1, 2, 3);
        Vector3 B = -A;
        Segment AB = new Segment { pt1 = A, pt2 = B };
        Vector3 norm = new Vector3(0, 1, 0);
        float d = 1;
        GeometricClass.Plane plane = new GeometricClass.Plane { Normal = norm, d = d };
        Vector3 interpt;
        Vector3 interNormal;
        bool test =  InterSegmentPlane( AB, plane, out interpt, out interNormal);
        DrawSegment(AB);
        Debug.DrawLine(new Vector3(-10, 1, 10), new Vector3(10, 1, 10), Color.red, 10);
        Debug.Log(test);
    }
}
