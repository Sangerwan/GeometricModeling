using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GeometricClass;
using static GeometricServices;
public class TestManager : MonoBehaviour
{
    MeshFilter m_Mf;
    [Header("Parameters")]
    [SerializeField] GameObject Plane;
    [SerializeField] GameObject Sphere;
    [SerializeField] GameObject Cylinder;
    [SerializeField] GameObject PointA;
    [SerializeField] GameObject PointB;
    [SerializeField] float pointSize;
    void OnDrawGizmos()
    {
        #region Segment
        {
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 16;
            myStyle.normal.textColor = Color.red;

            Gizmos.color = Color.red;
            Vector3 posA = PointA.transform.position;
            Vector3 posB = PointB.transform.position;

            Gizmos.DrawSphere(posA, pointSize);
            Handles.Label(posA, "A", myStyle);
            Gizmos.DrawSphere(posB, pointSize);
            Handles.Label(posB, "B", myStyle);
            Gizmos.DrawLine(posA, posB);
        }
        #endregion

        #region Plane
        {            
            Vector3 interpt;
            Vector3 interNormal;
            Segment segment = new Segment { pt1 = PointA.transform.position, pt2 = PointB.transform.position };
            Vector3 norm = Plane.transform.up;
            GeometricClass.Plane plane = new GeometricClass.Plane { Normal = norm, d = Plane.transform.position.y };
            if (InterSegmentPlane(segment, plane, out interpt, out interNormal))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(interpt, pointSize);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(interpt, interpt + interNormal * 10);
            }
        }
        #endregion

        #region Cylinder
        {
            Vector3 interpt;
            Vector3 interNormal;
            Segment segment = new Segment { pt1 = PointA.transform.position, pt2 = PointB.transform.position };
            GeometricClass.Cylinder cylinder = new GeometricClass.Cylinder { pt1= Cylinder.transform.position, pt2 = Cylinder.transform.position + Cylinder.transform.up, radius = Cylinder.transform.localScale.x/2};
            if (InterSegmentCylinder(segment, cylinder, out interpt, out interNormal))
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawSphere(interpt, pointSize);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(interpt, interpt + interNormal * 10);
            }

            Vector3 interpt2;
            //InterSegmentCylinder2(segment, cylinder, out interpt, out interpt2, out interNormal);
        }
        #endregion

        #region Sphere
        {
            Vector3 interpt;
            Vector3 interNormal;
            Segment segment = new Segment { pt1 = PointA.transform.position, pt2 = PointB.transform.position };
            GeometricClass.Sphere sphere = new GeometricClass.Sphere { center = Sphere.transform.position, radius = Sphere.transform.localScale.x/2 };
            if (InterSegmentSphere(segment, sphere, out interpt, out interNormal))
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(interpt, pointSize);
                Gizmos.color = Color.white;
                Gizmos.DrawLine(interpt, interpt + interNormal * 10);
            }
        }
        #endregion
    }
}
