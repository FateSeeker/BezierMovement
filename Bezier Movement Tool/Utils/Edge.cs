using UnityEngine;
using System.Collections;

public class Edge {
	
	public Vector3 point1;
	public Vector3 point2;

	public Edge(Vector3 Point1, Vector3 Point2)
	{
		point1 = Point1;
		point2 = Point2;
	}

	public float Longitude()
	{
		return Vector3.Distance (point1, point2);
	}

	public bool HasCommonPoint(Edge edge)
	{
		return (Vector2Equals(point1,edge.point1) || Vector2Equals(point1,edge.point2))
			|| (Vector2Equals(point2,edge.point1) || Vector2Equals(point2,edge.point2));
	}

	public bool Equals(Edge edge)
	{
		
		return (Edge.Vector2Equals (point1, edge.point1) && Edge.Vector2Equals (point2, edge.point2))||
			(Edge.Vector2Equals (point1, edge.point2) && Edge.Vector2Equals (point2, edge.point1));
	}

	public static bool Vector2Equals(Vector2 a, Vector2 b)
	{
		return (a.x == b.x) && (a.y == b.y);
	}
}
