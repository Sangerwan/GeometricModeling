using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeometricClass;

public static class GeometricServices
{

    public static bool InterSegmentPlane(Segment segment, GeometricClass.Plane plane, out Vector3 interpt, out Vector3 interNormal)
    {
		interpt = new Vector3();
		interNormal = new Vector3();
		//1
		Vector3 AB = segment.pt2 - segment.pt1;
		//2
		float dotABn = Vector3.Dot(AB, plane.Normal);
		//3
		if (Mathf.Approximately(dotABn, 0))
		{
			return false;
		}
		//4
		float t = (plane.d - Vector3.Dot(segment.pt1,plane.Normal)) / dotABn;
		//4 bis 
		if (t < 0 || t > 1)
		{
			return false;
		}
		//5
		interpt = segment.pt1 + t*AB;
		//6
		if (dotABn < 0)
		{
			interNormal = plane.Normal;
		}
		else
		{
			interNormal = -plane.Normal;
		}
		return true;
	}

	public static bool InterSegmentCircle(Segment segment, GeometricClass.Circle circle, out Vector3 interpt, out Vector3 interNormal)
	{
		interpt = new Vector3();
		interNormal = new Vector3();
		//1
		Vector3 AB = segment.pt2 - segment.pt1; 
		Vector3 OmgA = segment.pt1 - circle.center;
		float R = circle.radius;
		Debug.Log("Radius:" + R);
		float t1 = (-Vector3.Magnitude(OmgA) * Mathf.Cos(Vector3.Angle(OmgA, AB)) + R)/ Vector3.Magnitude(AB);
		float t2 = (-Vector3.Magnitude(OmgA) * Mathf.Cos(Vector3.Angle(OmgA, AB)) + R) / Vector3.Magnitude(AB);
		
		if (t1 < 0 || t1 > 1)
		{
			return false;
		}
		//5
		//interpt = segment.pt1 + t1 * AB;
		interpt = OmgA + t1 * AB;
		//interNormal = plane.Normal;
		return true;
	}

	public static void DrawSegment(Segment seg)
    {
		Debug.DrawLine(seg.pt1, seg.pt2,Color.red,10);
    }

	public static void InterCylinderSegment(Segment seg, Cylindre cyl, out Vector3 interpt, out Vector3 internormal)
    {
		interpt = new Vector3();
		interNormal = new Vector3();
		//1
		Vector3 u = (cyl.pt2 - cyl.pt1) / Vector3.Magnitude(cyl.pt2 - cyl.pt1);
		Vector3 AB = seg.pt2 - seg.pt1;
		Vector3 PA = cyl.pt1 - seg.pt2;
		float t1 = -((2 * AB * PA) * (1 - 4 * u * u + 3 * u * u * u * u) - Mathf.Sqrt(Mathf.Pow((2 * AB * PA) * (1 - 4 * u * u + 3 * u * u * u * u), 2)
			- 4 * AB * AB * (1 - 2 * u * u + u * u * u * u) * (PA * PA * (1 - 2 * u * u + u * u * u * u) - cyl.radius)) / (2 * 4 * AB * AB * (1 - 2 * u * u + u * u * u * u)));
		float t2 = -((2 * AB * PA) * (1 - 4 * u * u + 3 * u * u * u * u) + Mathf.Sqrt(Mathf.Pow((2 * AB * PA) * (1 - 4 * u * u + 3 * u * u * u * u), 2)
			- 4 * AB * AB * (1 - 2 * u * u + u * u * u * u) * (PA * PA * (1 - 2 * u * u + u * u * u * u) - cyl.radius)) / (2 * 4 * AB * AB * (1 - 2 * u * u + u * u * u * u)));
		if (t1 < 0 || t1 > 1)
		{
			return false;
		}
	}

	//public static void DrawPlane(GeometricClass.Plane plane)
	//{
	//	Debug.DrawSurface(seg.pt1, seg.pt2, Color.red);
	//}
}
