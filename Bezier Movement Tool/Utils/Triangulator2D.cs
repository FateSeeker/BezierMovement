using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;




public class Vector3Comparer:IEqualityComparer<Vector3>
{
	#region IEqualityComparer implementation
	public bool Equals (Vector3 x, Vector3 y)
	{
		return (x.x == y.x) && (y.y == x.y) && (x.z == y.z);
	}
	public int GetHashCode (Vector3 obj)
	{
		return obj.GetHashCode ();
	}
	#endregion

}

[Serializable]
public class Triangulation2D {

	//public delegate bool TriangleFilter(Triangle TrianglesToFilter);

	//public Predicate<Triangle> TriangleFilter;

	public List<Vector3> cloudPoints;
	public List<int> TrianglesIndices;
	public List<Triangle> Triangles;
	public List<Triangle> CleanTriangles;



	public Triangulation2D (List<Vector3> CloudPoints)
	{
		cloudPoints = new List<Vector3> ();

		Triangles = new List<Triangle> ();
		TrianglesIndices = new List<int> ();
		CleanTriangles = new List<Triangle> ();

		//Triangles.Add (Triangle.InfiniteTriangle);

		Triangulate (CloudPoints);

        
	}

	public void Triangulate(List<Vector3> CloudPoints)
	{
		cloudPoints = new List<Vector3> ();
		Triangles = new List<Triangle> ();
		TrianglesIndices = new List<int> ();
		CleanTriangles = new List<Triangle> ();

		cloudPoints = CloudPoints.Distinct (new Vector3Comparer()).ToList();

		Triangles.Add (Triangle.InfiniteTriangle);

		for (int i = 0; i < cloudPoints.Count; i++) {
			List<Triangle> containerTriangles = ExtractContainerTriangles ( cloudPoints [i]);
			//Debug.Log (containerTriangles.Count);
			removeTriangles (containerTriangles);

			Triangles.AddRange (MakeNewTriangles (containerTriangles, cloudPoints [i]));


		}

		CleanTriangles = CleanTrianglesList ();
        FillIndicesTriangles();
    }

	private void removeTriangles(List<Triangle> trianglesToRemove)
	{
		
		foreach (Triangle item in trianglesToRemove) {
			if(Triangles.Exists(t=>t.Equals(item)))
			{
				Triangles.Remove (item);
			}
		}
	}

	public List<Triangle> CleanTrianglesList()
	{
		List<Triangle> result = Triangles;
		result.RemoveAll (t => ((!cloudPoints.Contains (t.p1) || !cloudPoints.Contains (t.p2) || !cloudPoints.Contains (t.p3))
||
			!PointInsidePolygon( t.Centroid())
		)
		
		);



		return result;
	}

	public List<Triangle> ExtractContainerTriangles(Vector2 Point)
	{		
		
		return Triangles.FindAll (t => t.IsPointInCircumCircle (Point) );
	}



	public Triangle[] MakeNewTriangles(List<Triangle> vTriangles, Vector3 Point)
	{
		List<Triangle> result = new List<Triangle> ();

		List<Edge> edges = ExtractUniqueEdges (vTriangles);


		for (int i = 0; i < edges.Count; i++) {
			///int before = (i - 1 < 0) ? (vertices.Count - 1) : (i - 1);
			Triangle n = new Triangle(Point,edges[i]);
			result.Add (n);

			}


		return result.ToArray();
	}

	public List<Edge> ExtractUniqueEdges(List<Triangle> vTriangles)
	{
		List<Edge> result = new List<Edge> ();
		List<Edge> blacklist = new List<Edge> ();

		foreach (Triangle t in vTriangles) {
			if (result.Exists (e => e.Equals(t.e1))) {
				blacklist.Add (t.e1);
			} else {
				result.Add (t.e1);
			}
			if (result.Exists (e => e.Equals (t.e2))) {
				blacklist.Add (t.e2);
			} else {
				result.Add (t.e2);
			}
			if (result.Exists (e => e.Equals (t.e3))) {
				blacklist.Add (t.e3);
			} else {
				result.Add (t.e3);
			}
			
		}
		//Debug.Log (blacklist.Count);
		foreach (Edge e in blacklist) {
			result.RemoveAll (edge => edge.Equals (e));
		}

		return result;
	}

	private void FillIndicesTriangles()
	{
		TrianglesIndices = new List<int> ();
		foreach (Triangle t in CleanTriangles) {
			TrianglesIndices.Add (cloudPoints.IndexOf (t.p1));
			TrianglesIndices.Add (cloudPoints.IndexOf (t.p2));
			TrianglesIndices.Add (cloudPoints.IndexOf (t.p3));
		}
	}

	public Mesh GenerateMesh()
	{
		FillIndicesTriangles ();
		Mesh result = new Mesh ();


		result.vertices = cloudPoints.ToArray ();
		result.triangles = TrianglesIndices.ToArray ();

		result.RecalculateNormals ();
		result.Optimize ();


		return result;
	}



	public bool PointInsidePolygon(Vector3 Point)
	{
		
		bool result = false;
		for (int i = 0 , j = cloudPoints.Count-1; i < cloudPoints.Count;j = i++) {

			if((((cloudPoints[i].y <= Point.y) && (Point.y < cloudPoints[j].y)) || 
				((cloudPoints[j].y <= Point.y) && (Point.y < cloudPoints[i].y))) &&
				(Point.x < (cloudPoints[j].x - cloudPoints[i].x) * (Point.y - cloudPoints[i].y) / (cloudPoints[j].y - cloudPoints[i].y) + cloudPoints[i].x)){
				result = !result;
			}
		}

		return result;
	}

}
