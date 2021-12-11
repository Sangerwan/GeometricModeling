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
		interNormal.Normalize();
		return true;
	}

	public static bool isValid(float t)
    {
		return t>=0 && t<=1;

	}

	public static bool InterSegmentSphere(Segment segment, GeometricClass.Sphere sphere, out Vector3 interpt, out Vector3 interNormal)
	{
		interpt = new Vector3();
		interNormal = new Vector3();
		//1

		Vector3 AB = segment.pt2 - segment.pt1;
		Vector3 OA = segment.pt1 - sphere.center;


		float a = Vector3.Dot(AB, AB);
		float b = 2f * Vector3.Dot(OA, AB);
		float c = Vector3.Dot(OA,OA) - sphere.radius * sphere.radius;

		float det = b * b - 4f * a * c;

		if (det < 0)
        {
			return false;
        }

		float x1 = (-b - Mathf.Sqrt(det))/(2f*a);
		float x2 = (-b + Mathf.Sqrt(det))/(2f*a);

		if(isValid(x1))
        {
			interpt = segment.pt1 + x1 * AB;
			interNormal = (interpt - sphere.center);
			interNormal.Normalize();
			return true;
		}
		if (isValid(x2))
		{
			interpt = segment.pt1 + x2 * AB;
			interNormal = -(interpt - sphere.center);
			interNormal.Normalize();
			return true;
		}

		return false;
	}

    public static bool InterSegmentCylinder(Segment segment, Cylinder cylinder, out Vector3 interpt, out Vector3 interNormal)
    {
        interpt = new Vector3();
        interNormal = new Vector3();

        Vector3 AB = segment.pt2 - segment.pt1;
		Vector3 PA = segment.pt1 - cylinder.pt1;
		Vector3 PQ = cylinder.pt2 - cylinder.pt1;
		Vector3 u = PQ / PQ.magnitude;

        float a1 =
            Vector3.Dot(AB, AB)
            + (Vector3.Dot(AB, PQ) / PQ.magnitude)
            * (
                -2 * Vector3.Dot(AB, u)
                + (Vector3.Dot(AB, PQ) / PQ.magnitude) * Vector3.Dot(u, u)
            );

        float b1 =
            2f *
            (
                Vector3.Dot(AB, PA)
                - Vector3.Dot(PA, (Vector3.Dot(AB, PQ) / PQ.magnitude) * u)
                - Vector3.Dot(AB, (Vector3.Dot(PA, PQ) / PQ.magnitude) * u)
            );

        float c1 =
            Vector3.Dot(PA, PA)
            - 2f * Vector3.Dot(PA, (Vector3.Dot(PA, PQ) / PQ.magnitude) * u)
            + (Vector3.Dot(PA, PQ) / PQ.magnitude) * (Vector3.Dot(PA, PQ) / PQ.magnitude) * Vector3.Dot(u, u)
            - cylinder.radius * cylinder.radius;

        float a = Vector3.Dot(AB, AB) - 2 * Vector3.Dot(AB, Vector3.Dot(AB,PQ)/PQ.magnitude * u) + Mathf.Pow(Vector3.Dot(AB, PQ)/PQ.magnitude, 2)* Vector3.Dot(u,u);
		float b = 2 * Vector3.Dot(AB, PA) - 4 * Vector3.Dot(AB, Vector3.Dot(PA, PQ) / PQ.magnitude * u) + 2*Vector3.Dot(AB,PQ)*Vector3.Dot(PA,PQ)/Mathf.Pow(PQ.magnitude, 2) * Vector3.Dot(u,u);
		float c = Vector3.Dot(PA,PA) - 2 *  Vector3.Dot(PA, Vector3.Dot(PA,PQ) / PQ.magnitude * u) + Mathf.Pow(Vector3.Dot(PA, PQ)/PQ.magnitude,2) *Vector3.Dot(u, u) - Mathf.Pow(cylinder.radius, 2);

        Debug.Log(Vector3.Dot(AB, PA));
        Debug.Log("a" + a);
        Debug.Log("b" + b);
        Debug.Log("c" + c);


        float det = b * b - 4f * a * c;
		Debug.Log("det "+ det);
		if (det < 0)
		{
			return false;
		}

		float x1 = (-b - Mathf.Sqrt(det)) / (2f * a);
		float x2 = (-b + Mathf.Sqrt(det)) / (2f * a);

		if (isValid(x1))
		{
			interpt = segment.pt1 + x1 * AB;
			Debug.Log("inter " + interpt);
			//interNormal = (interpt - cylinder.center);
			//interNormal.Normalize();
			return true;
		}
		if (isValid(x2))
		{
			interpt = segment.pt1 + x2 * AB;
			Debug.Log("inter " + interpt);
			//interNormal = -(interpt - cylinder.center);
			//interNormal.Normalize();
			return true;
		}

		return false;
	}


    /////a tester
    //public static bool InterSegmentCylinder2(Segment segment, Cylinder cylinder, out Vector3 interPt1, out Vector3 interPt2, out Vector3 interNormal)
    //{
    //    interPt1 = new Vector3();
    //    interPt2 = new Vector3();
    //    interNormal = new Vector3();

    //    Vector3 u = (cylinder.pt2 - cylinder.pt1) / Vector3.Magnitude(cylinder.pt2 - cylinder.pt1);
    //    Vector3 AB = segment.pt2 - segment.pt1;
    //    Vector3 PA = segment.pt1 - cylinder.pt1;

    //    float a = AB.magnitude * AB.magnitude - (1 - 2 * Vector3.Dot(u, u) + Vector3.Dot(u, u) * Vector3.Dot(u, u));
    //    float b = Vector3.Dot(AB, PA) * (1 - 4 * Vector3.Dot(u, u) + 3 * Vector3.Dot(u, u) * Vector3.Dot(u, u));
    //    float c = Vector3.Dot(PA, PA) - (1 - 2 * Vector3.Dot(u, u) + Vector3.Dot(u, u) * Vector3.Dot(u, u)) - cylinder.radius * cylinder.radius;
    //    Debug.Log(Vector3.Dot(AB, PA));
    //    Debug.Log("a2" + a);
    //    Debug.Log("b2" + b);
    //    Debug.Log("c2" + c);


    //    float Delt = (b * b) - 4 * a * c;

    //    float racine1;
    //    float racine2;

    //    if (Delt < 0)
    //    {
    //        return false;
    //    }
    //    else
    //    {
    //        racine1 = (-b - Mathf.Sqrt(Delt)) / 2 * a;
    //        racine2 = (-b + Mathf.Sqrt(Delt)) / 2 * a;
    //    }

    //    float t1 = racine1;
    //    float t2 = racine2;
    //    Debug.Log(t1);
    //    Debug.Log(t2);
    //    if (!(t1 < 0 || t1 > 1))
    //    {
    //        Debug.Log(t1);
    //        Debug.Log(t2);
    //    }

    //    interPt1 = segment.pt1 + t1 * AB;
    //    interPt2 = segment.pt1 + t2 * AB;

    //    return true;
    //}
}
