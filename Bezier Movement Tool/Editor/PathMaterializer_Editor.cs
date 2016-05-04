using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathMaterializer))]
public class PathMaterializer_Editor:Editor
{
    PathMaterializer Target;
    Camera c;
    

    Vector2 nearPoint1, nearPoint2;

    void OnEnable()
    {
        Target = (PathMaterializer)target;
        
        nearPoint1 = Vector2.zero;
        nearPoint2 = Vector2.zero;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        if(Target.GetComponent<PathEditorBehaviour>().PathToEdit!=null && Target.GetComponent<PathEditorBehaviour>().PathToEdit.Segments.Count > 0)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Points"), true);
            Target.Resolution = (EditorGUILayout.Slider("Detail",Target.Resolution/10-1, 0, 10)+1)*10;
            if (GUILayout.Button("Create Collider"))
            {
                Target.Points = Triangulator.ExtractCloudPointFromPath(Target.GetComponent<PathEditorBehaviour>().PathToEdit, Target.Resolution);
                GenerateCollider(Target.Points);
                AssetDatabase.SaveAssets();
                
            }
            if (GUILayout.Button("Create Mesh"))
            {
                Target.Points = Triangulator.ExtractCloudPointFromPath(Target.GetComponent<PathEditorBehaviour>().PathToEdit, Target.Resolution);
                GenerateMesh(Target.Points);
                AssetDatabase.SaveAssets();

            }

            Target.AreaEffector = EditorGUILayout.FloatField("Area Efector Size", Target.AreaEffector);

            if(GUILayout.Button("Create Area Effector"))
            {
                Target.Points = Triangulator.ExtractCloudPointFromPath(Target.GetComponent<PathEditorBehaviour>().PathToEdit, Target.Resolution);
                GenerateAreaEfector(Target.Points);
                AssetDatabase.SaveAssets();

            }
        }

        EditorGUI.EndChangeCheck();

        serializedObject.ApplyModifiedProperties();
        AssetDatabase.SaveAssets();
        

    }

    private void GenerateCollider(List<Vector3> points)
    {
        if (Target.GetComponent<PolygonCollider2D>() == null)
        {
            Target.gameObject.AddComponent<PolygonCollider2D>();
        }
        else
        {
            PolygonCollider2D p = Target.GetComponents<PolygonCollider2D>()[0];
            //Target.gameObject.AddComponent<PolygonCollider2D>();
            Target.gameObject.GetComponents<PolygonCollider2D>()[0] = new PolygonCollider2D();
            Target.gameObject.GetComponents<PolygonCollider2D>()[Target.GetComponents<PolygonCollider2D>().Count()-1] = p;
        }

        

        Target.GetComponent<PolygonCollider2D>().SetPath(0, points.ConvertAll<Vector2>(v => v = new Vector2(v.x, v.y)).ToArray());
    }

    private void GenerateMesh(List<Vector3> points)
    {
        Target.GeneratedMesh = new Mesh();


        if (Target.triangulation == null)
        {
            Target.triangulation = new Triangulation2D(points);
        }
        else
        {
            Target.triangulation.Triangulate(points);
        }

        Target.GeneratedMesh.vertices = points.ToArray();
        Target.GeneratedMesh.triangles = Target.triangulation.TrianglesIndices.ToArray();
        Target.GeneratedMesh.RecalculateNormals();
        Target.GeneratedMesh.Optimize();

        /*Target.GeneratedMesh.vertices = points.ToArray();
        Target.GeneratedMesh.triangles = Triangulator.Triangulate(points).ToArray();
        Target.GeneratedMesh.RecalculateNormals();
        Target.GeneratedMesh.Optimize();*/

        if (Target.GetComponent<MeshRenderer>() == null) Target.gameObject.AddComponent<MeshRenderer>();
        if (Target.GetComponent<MeshFilter>() == null) Target.gameObject.AddComponent<MeshFilter>();

        Target.GetComponent<MeshFilter>().mesh = Target.GeneratedMesh;
    }

    private void GenerateAreaEfector(List<Vector3> points)
    {
        List<Vector3> areaEffectorPoints = new List<Vector3>();
        
        for (int i = 0; i < points.Count-1; i++)
        {
            Vector3 p = points[i];
            Vector3 p2 = points[i + 1];
            
            areaEffectorPoints.Add(Quaternion.Euler(0, 0, 90) * (p2 - p).normalized * Target.AreaEffector + p);
        }
        areaEffectorPoints.Add(Quaternion.Euler(0, 0, 90) * (points[0] - points[points.Count-1]).normalized * Target.AreaEffector + points[points.Count-1] );

        if (Target.GetComponent<AreaEffector2D>() == null) Target.gameObject.AddComponent<AreaEffector2D>();
        if(Target.GetComponents<PolygonCollider2D>().Count() < 2) Target.gameObject.AddComponent<PolygonCollider2D>().SetPath(0, areaEffectorPoints.ConvertAll<Vector2>(v => v = new Vector2(v.x, v.y)).ToArray());
        else Target.GetComponents<PolygonCollider2D>()[1].SetPath(0, areaEffectorPoints.ConvertAll<Vector2>(v => v = new Vector2(v.x, v.y)).ToArray());

        if (Target.GetComponent<PolygonCollider2D>() == null) Target.gameObject.AddComponent<PolygonCollider2D>().SetPath(0, areaEffectorPoints.ConvertAll<Vector2>(v => v = new Vector2(v.x, v.y)).ToArray());
        else if (Target.GetComponents<PolygonCollider2D>().Count() > 0) Target.GetComponents<PolygonCollider2D>()[Target.GetComponents<PolygonCollider2D>().Count()-1].SetPath(0, areaEffectorPoints.ConvertAll<Vector2>(v => v = new Vector2(v.x, v.y)).ToArray());

        
        Target.GetComponents<PolygonCollider2D>()[Target.GetComponents<PolygonCollider2D>().Count() - 1].usedByEffector = true;
        Target.GetComponents<PolygonCollider2D>()[Target.GetComponents<PolygonCollider2D>().Count() - 1].isTrigger = true;
        
    }

    private void SeeDirectionalVectors(List<Vector3> points)
    {
        Handles.color = Color.yellow;
        for (int i = 0; i < points.Count-1; i++)
        {
            Vector3 p = points[i] ;
            Vector3 p2 = points[i + 1];
            Handles.DrawLine(points[i]+Target.transform.position, Quaternion.Euler(0, 0, 90) * (p2 - p).normalized * 2 + p + Target.transform.position);
        }
    }

    void OnSceneGUI()
    {
        if (Camera.current != null) c = Camera.current;

        if (Event.current.type == EventType.repaint)
        {
            //Debug.Log((Vector2)c.ScreenToWorldPoint(Event.current.mousePosition));



            
            //SeeDirectionalVectors(Points);
            //GenerateMesh(Points);

            //GenerateAreaEfector(Points);
            /* GenerateCollider(Points);*/


        }
        
    }
}

