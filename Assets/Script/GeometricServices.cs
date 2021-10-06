using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GeometricClass;

public class GeometricServices : MonoBehaviour
{
	public GameObject ptA;
	public GameObject ptB;

    private void Start()
    {
		Segment seg = new Segment();
		seg.pt1 = ptA.transform.position;
		seg.pt2 = ptB.transform.position;
		GeometricClass.Plane plane = new GeometricClass.Plane();
		plane.Normal = new Vector3(0, 0, 1);
		plane.d = 1;
		Vector3 interpt, interNormal;
		bool test;
		test=InterSegmentPlane(seg, plane, out interpt,out interNormal);
		Debug.Log(test);
		Debug.Log(interpt);
		Debug.Log(interNormal);

	}

    bool InterSegmentPlane(Segment segment, GeometricClass.Plane plane, out Vector3 interpt, out Vector3 interNormal)
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
}
