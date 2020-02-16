using UnityEngine;
using UnityEditor;

public struct Quad
{
    public Vector3 LowerLeft, UpperLeft, UpperRight, LowerRight;

    public Quad(Vector3 lowerLeft, Vector3 upperLeft, Vector3 upperRight, Vector3 lowerRight)
    {
        LowerLeft = lowerLeft;
        UpperLeft = upperLeft;
        UpperRight = upperRight;
        LowerRight = lowerRight;
    }

    public static Quad operator +(Quad quad, Vector3 vec)
        => new Quad(quad.LowerLeft + vec, quad.UpperLeft + vec, quad.UpperRight + vec, quad.LowerRight + vec);
}