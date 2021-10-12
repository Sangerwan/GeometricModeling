using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeometricClass;
using static GeometricServices;
public class TestManager : MonoBehaviour
{

    public GameObject obj;
    public GameObject sphere;
    static Vector3 A = new Vector3(1, 2, 3);
    static Vector3 B = -A;
    static Segment AB = new Segment { pt1 = A, pt2 = B };
    // Start is called before the first frame update
    void Update()
    {
        DrawSegment(AB);
    }

    // Update is called once per frame
    void Start()
    {       
        Vector3 norm = new Vector3(0, 1, 0);
        float d = 1;
        GeometricClass.Plane plane = new GeometricClass.Plane { Normal = norm, d = d };
        Vector3 interpt;
        Vector3 interNormal;
        bool test =  InterSegmentPlane( AB, plane, out interpt, out interNormal);


        Debug.Log("scale: " + sphere.gameObject.transform.localScale.x);
        GeometricClass.Circle circle = new GeometricClass.Circle { radius = sphere.gameObject.transform.localScale.x, center = new Vector3(0,0,0) };
        test = InterSegmentCircle(AB, circle, out interpt, out interNormal);
        Debug.Log(test);
        Debug.Log(interpt);
        Instantiate(obj, interpt, new Quaternion());
        Instantiate(sphere,  circle.center, new Quaternion());


    }
}
