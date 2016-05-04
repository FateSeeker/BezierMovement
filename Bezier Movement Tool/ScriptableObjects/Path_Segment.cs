using UnityEngine;


using System.Collections;

[System.Serializable]
public class Path_Segment : ScriptableObject
{
    public bool ShowDetails;
    public Vector3 Offset;
    public Vector3 Start, End, TangentA, TangentB;
    public float Longitude;

    public void Initialize(Vector3 Start, Vector3 End, Vector3 TangentA, Vector3 TangentB, Vector3 Offset)
    {
        this.Start = Start;
        this.End = End;
        this.TangentA = TangentA;
        this.TangentB = TangentB;
        this.Offset = Offset;
        Longitude = SetLongitude();
    }

    public Vector3 ParametrizedPosition(float T)
    {
        return MathUtils.BezierFuncParam(Start + Offset, TangentA+Start + Offset, TangentB+End + Offset, End + Offset, T);
    }

    public Vector3 ParametrizedPosition(float T,Vector3 CustomOffset)
    {
        return MathUtils.BezierFuncParam(Start + CustomOffset, TangentA + Start + CustomOffset, TangentB + End + CustomOffset, End + CustomOffset, T);
    }

    public float SetLongitude()
    {
        float result = 0;
        
        //Vector3[] points = Handles.MakeBezierPoints(Start + Offset, End + Offset, TangentA + Start + Offset, TangentB + End + Offset, 4);
        for (float i = 0; i < 1; i+=.25f)
        {
            result += Vector3.Distance(ParametrizedPosition(i+.25f), ParametrizedPosition(i));
        }
        return result;
        
    }
}

