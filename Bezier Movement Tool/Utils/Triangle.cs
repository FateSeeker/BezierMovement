using UnityEngine;
using System.Collections;

[System.Serializable]
public class Triangle  {

	public Vector2 p1, p2, p3;

	public Edge e1,e2,e3;

	public Vector2 CircumCenter;

	public static Triangle InfiniteTriangle = new Triangle (new Vector2 (-50, -50), new Vector2 (0, 50), new Vector2 (50, 
		-50)); 

	public Triangle(Vector2 P1,Vector2 P2,Vector2 P3)
	{
		p1 = P1;
		p2 = P2;
		p3 = P3;

		e1 = new Edge (p1, p2);
		e2 = new Edge (p2, p3);
		e3 = new Edge (p3, p1);

		CircumCenter = new Vector2 ();

		findCircumCenter ();

	}
	public Triangle(Edge E1,Edge E2,Edge E3)
	{
		e1 = E1;
		e2 = E2;
		e3 = E3;
		p1 = e1.point1;
		p2 = ((e2.point1.x == p1.x) && (e2.point1.y == p1.y)) ? e2.point2 : e2.point1;
		if (((e3.point1.x == p1.x) && (e3.point1.y == p1.y)) || ((e3.point1.x == p2.x) && (e3.point1.y == p2.y)))
			p3 = e3.point2;
		else
			p3 = e3.point1;

		CircumCenter = new Vector2 ();

		findCircumCenter ();

	}

	public Triangle(Vector2 P,Edge E1)
	{
		e1 = E1;

		p1 = E1.point1;
		p2 = E1.point2;
		p3 = P;

		e2 = new Edge (p2, p3);
		e3 = new Edge (p3, p1);



		CircumCenter = new Vector2 ();

		findCircumCenter ();

	}


	public bool IsPointInside(Vector2 point)
	{
		float s = p1.y * p3.x - p1.x * p3.y + (p3.y - p1.y) * point.x + (p1.x - p3.x) * point.y;
		float t = p1.x * p2.y - p1.y * p2.x + (p1.y - p2.y) * point.x + (p2.x - p1.x) * point.y;

		if ((s < 0) != (t < 0)) {
			return false;
		}

		float A = -p2.y * p3.x + p1.y * (p3.x - p2.x) + p1.x * (p2.y - p3.y) + p2.x * p3.y;

		if (A < 0f) {
			s = -s;
			t = -t;
			A = -A;
		}

		return s > 0 && t > 0 && (s + t) <= A;
	}

	public Vector3 Centroid()
	{
		return (p1 + p2 + p3) / 3;
	}

	public bool IsPointInCircumCircle(Vector2 point)
	{			
		return Vector3.Distance(point,CircumCenter) < Vector3.Distance(p1,CircumCenter);
	}

	private void findCircumCenter()
	{
		Vector3 ac = p3 - p1;
		Vector3 ab = p2 - p1;
		Vector3 abXac = Vector3.Cross (ab, ac);

		Vector3 t1 = (ac.magnitude * ac.magnitude) * Vector3.Cross (abXac, ab);
		Vector3 t2 = (ab.magnitude * ab.magnitude) * Vector3.Cross (ac, abXac);

		float t3 = 2 * (abXac.magnitude * abXac.magnitude);

		CircumCenter = (Vector3)p1 + (t1 + t2) / t3;

	}



	public bool Equals(Triangle t)
	{
		return (Edge.Vector2Equals (p1, t.p1) ||
		Edge.Vector2Equals (p1, t.p2) ||
		Edge.Vector2Equals (p1, t.p3)) &&
		(Edge.Vector2Equals (p2, t.p1) ||
		Edge.Vector2Equals (p2, t.p2) ||
		Edge.Vector2Equals (p2, t.p3)) &&
		(Edge.Vector2Equals (p3, t.p1) ||
		Edge.Vector2Equals (p3, t.p2) ||
		Edge.Vector2Equals (p3, t.p3));
	}
	public override string ToString ()
	{
		return (p1 + " " + p2 + " " + p3);
	}
}
