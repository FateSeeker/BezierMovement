using UnityEngine;

using System.Collections.Generic;


[System.Serializable]
public class Path : ScriptableObject
{
#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/Create/Path")]
    public static void CreateAsset()
    {
        ScriptableObjectUtils.CreateAsset<Path>().Initialize(new List<Path_Segment>(), Vector3.zero);
    }

#endif


    public bool ShowSegmentsInfo;


    public List<Path_Segment> Segments;
    public Vector3 Offset;

    void OnEnable()
    {

        //Segments = new List<Path_Segment>();
    }

    public void UpdateOffset(Vector3 newOffset)
    {
        Offset = newOffset;

        Segments.ForEach(s => s.Offset = newOffset);
    }

    public Vector3 ParametrizedPosition(float T)
    {
        float d = 0;
        int index = 0;
        for (int i = 0; i < Segments.Count; i++)
        {
            
            if (d + Segments[i].Longitude/Longitude() >= T)
            {
                index = i;
                break;
            }
            d += Segments[i].Longitude / Longitude();
        }


        Vector3 result = Segments[/*(index + 1 > Segments.Count - 1) ? 0 : index + 1*/index].ParametrizedPosition((T - d) / (Segments[index].Longitude / Longitude()));

        


        return result;
    }
    public Vector3 ParametrizedPosition(float T,Vector3 CustomOffset)
    {
        float d = 0;
        int index = 0;
        for (int i = 0; i < Segments.Count; i++)
        {

            if (d + Segments[i].Longitude / Longitude() >= T)
            {
                index = i;
                break;
            }
            d += Segments[i].Longitude / Longitude();
        }

        float rT = T * Segments.Count - Mathf.Floor(T * Segments.Count);

        //Vector3 result = Segments[/*(index + 1 > Segments.Count - 1) ? 0 : index + 1*/index].ParametrizedPosition((T - d) / (Segments[index].Longitude / Longitude()),CustomOffset);

        Vector3 result = Segments[Mathf.FloorToInt(T * Segments.Count)].ParametrizedPosition(rT, CustomOffset);


        return result;
    }


    public float Longitude()
    {
        float result = 0;
        Segments.ForEach(s => result += s.Longitude);
        
        return result;
    }

    public void Initialize(List<Path_Segment> PathSegments, Vector3 Offset)
    {
        Segments = PathSegments;
        this.Offset = Offset;
        Segments.ForEach(s => s.Offset = Offset);
    }
}

