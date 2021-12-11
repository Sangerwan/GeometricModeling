using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GeometricClass;
using static GeometricServices;
public class TestManager : MonoBehaviour
{
    [Header("Objects")]    
    [SerializeField] GameObject Plane;
    [SerializeField] GameObject Sphere;
    [SerializeField] GameObject Cylinder;
    [SerializeField] GameObject PointA;
    [SerializeField] GameObject PointB;

    [Header("Points (A,B)")]
    [SerializeField] float Size_Point_AB;
    [SerializeField] Color Color_PointA;
    [SerializeField] Color Color_PointB;
    [SerializeField] Color Color_Segment_AB;

    [Header("Normals")]
    [SerializeField] float Size_normals;
    [SerializeField] Color Color_Normals;

    [Header("Intersections")]
    [SerializeField] float Size_Intersect_Points;
    [SerializeField] Color Color_Intersect_Sphere;
    [SerializeField] Color Color_Intersect_Cylinder;
    [SerializeField] Color Color_Intersect_Plane;    
    
    void OnDrawGizmos()
    {
        #region Segment
        {
            GUIStyle myStyle = new GUIStyle();
            myStyle.fontSize = 16;

            Vector3 posA = PointA.transform.position;
            Vector3 posB = PointB.transform.position;

            Gizmos.color = Color_PointA;
            myStyle.normal.textColor = Color_PointA;
            Gizmos.DrawSphere(posA, Size_Point_AB);
            Handles.Label(posA, "A", myStyle);

            Gizmos.color = Color_PointB;
            myStyle.normal.textColor = Color_PointB;
            Gizmos.DrawSphere(posB, Size_Point_AB);
            Handles.Label(posB, "B", myStyle);

            Gizmos.color = Color_Segment_AB;
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
                Gizmos.color = Color_Intersect_Plane;
                Gizmos.DrawSphere(interpt, Size_Intersect_Points);
                Gizmos.color = Color_Normals;
                Gizmos.DrawLine(interpt, interpt + interNormal * Size_normals);
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
                Gizmos.color = Color_Intersect_Cylinder;
                Gizmos.DrawSphere(interpt, Size_Intersect_Points);
                Gizmos.color = Color_Normals;                 
                Gizmos.DrawLine(interpt, interpt + interNormal * Size_normals);
            }
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
                Gizmos.color = Color_Intersect_Sphere;
                Gizmos.DrawSphere(interpt, Size_Intersect_Points);
                Gizmos.color = Color_Normals;                
                Gizmos.DrawLine(interpt, interpt + interNormal * Size_normals);
            }
        }
        #endregion
    }
}
