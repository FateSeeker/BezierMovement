using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;



public class Triangulator
{
    public static List<Vector3> ExtractCloudPointFromPath(Path PathToExtract,float detail)
    {
        List<Vector3> result = new List<Vector3>();

        foreach (Path_Segment path in PathToExtract.Segments)
        {
            float pathLongitude = path.Longitude;
            for (float i = 0; i <= 1; i += 1 / (detail))
            {
                result.Add(path.ParametrizedPosition(i,Vector3.zero));
            }
        }
        return result;
    }

    public static List<Vector2> ExtractCloudPoint2DFromPath(Path PathToExtract, float detail)
    {
        List<Vector2> result = new List<Vector2>();

        foreach (Path_Segment path in PathToExtract.Segments)
        {
            float pathLongitude = path.Longitude;
            for (float i = 0; i <= 1; i += 1 / (pathLongitude * detail))
            {
                result.Add(path.ParametrizedPosition(i, Vector3.zero));
            }
        }
        return result;
    }

    public static List<Vector3> ExtractCloudPointFromPath(Path PathToExtract, int detail,Camera tCamera, out float Width, out float Height)
    {
        List<Vector3> result = new List<Vector3>();

        float MaxY = float.NegativeInfinity, MinY = float.PositiveInfinity, MaxX = float.NegativeInfinity, MinX = float.PositiveInfinity;

        foreach (Path_Segment path in PathToExtract.Segments)
        {
            float pathLongitude = path.Longitude;
            for (float i = 0; i <= 1; i += 1 / (pathLongitude * detail))
            {
                Vector3 a = path.ParametrizedPosition(i, Vector3.zero);
                result.Add(a);
                a = tCamera.WorldToScreenPoint(a);
                if (a.x > MaxX) MaxX = a.x;
                if (a.y > MaxY) MaxY = a.y;
                if (a.x < MinX) MinX = a.x;
                if (a.x < MinY) MinY = a.y;
                
            }
        }

        Height = MaxY - MinY;
        Width = MaxX - MinX;
        return result;
    }

    public static List<int> Triangulate(List<Vector3> CloudPoints)
    {
        #region Initializing
        List<Vector3> cloudPoints = CloudPoints.Distinct().ToList();
        List<Triangle> triangles = new List<Triangle>();
        List<int> trianglesindices = new List<int>();
        List<Triangle> cleanTriangles = new List<Triangle>();

        triangles.Add(Triangle.InfiniteTriangle);
        #endregion

        #region ForThroughAllPoints

        for (int i = 0; i < cloudPoints.Count; i++)
        {
            #region ExtractTriangles_Which_Circumcircle_ContainsPoint

            List<Triangle> containerTriangles = triangles.FindAll(t => t.IsPointInCircumCircle((Vector2)cloudPoints[i]));
            //Debug.Log("container" + containerTriangles.Count);
            #endregion

            #region Remove_Container_Triangles_From_List
            foreach (Triangle item in containerTriangles)
            {
                if (triangles.Exists(t => t.Equals(item)))
                {
                    triangles.Remove(item);
                }
            }
            #endregion

            #region MakeNewTriangles_And_AddToList

            List<Triangle> newTriangles = new List<Triangle>();

                #region ExtractUnique_Edges_From_ContainerTriangles           

                    List<Edge> edges = new List<Edge>();
                    List<Edge> blacklist = new List<Edge>();

                    foreach (Triangle t in containerTriangles)
                    {
                        if (edges.Exists(e => e.Equals(t.e1)))
                        {
                            blacklist.Add(t.e1);
                        }
                        else {
                            edges.Add(t.e1);
                        }
                        if (edges.Exists(e => e.Equals(t.e2)))
                        {
                            blacklist.Add(t.e2);
                        }
                        else {
                            edges.Add(t.e2);
                        }
                        if (edges.Exists(e => e.Equals(t.e3)))
                        {
                            blacklist.Add(t.e3);
                        }
                        else {
                            edges.Add(t.e3);
                        }

                    }
                    //Debug.Log (blacklist.Count);
                    foreach (Edge e in blacklist)
                    {
                        edges.RemoveAll(edge => edge.Equals(e));
                    }         

                #endregion


            for (int j = 0; j < edges.Count; j++)
            {
                ///int before = (i - 1 < 0) ? (vertices.Count - 1) : (i - 1);
                Triangle n = new Triangle(cloudPoints[i], edges[j]);
                newTriangles.Add(n);

            }

            #endregion


            triangles.AddRange(newTriangles);
        }

        #endregion

        #region Clean_Triangles

        cleanTriangles = triangles;

        cleanTriangles.RemoveAll(t => ((!cloudPoints.Contains(t.p1) || !cloudPoints.Contains(t.p2) || !cloudPoints.Contains(t.p3))
||
           !MathUtils.PointInsidePolygon(cloudPoints.ToArray(),t.Centroid())
       )

        );

        #endregion

        #region Extract_Indices

        foreach (Triangle t in cleanTriangles)
        {
            trianglesindices.Add(cloudPoints.IndexOf(t.p1));
            trianglesindices.Add(cloudPoints.IndexOf(t.p2));
            trianglesindices.Add(cloudPoints.IndexOf(t.p3));
        }

        #endregion

        //Debug.Log(trianglesindices.Count + " " + triangles.Count);

        #region Results

        return trianglesindices;

        #endregion
    }
}

