using UnityEngine;
public class Plane {
    public Vector3 firstVector;
    public Vector3 secondVector;

    public Vector3 normal;

    public Plane(Vector3 _firstVector, Vector3 _secondVector) {
        firstVector = _firstVector;
        secondVector = _secondVector;
        normal = Vector3.Cross(_firstVector, _secondVector);
    }

    public void DrawPlane(Vector3 position, float size, float time) {
        Vector3 firstScaled = firstVector * size;
        Vector3 secondScaled = secondVector * size;
        Debug.DrawLine(position + firstScaled, position + secondScaled, Color.yellow, time);
        Debug.DrawLine(position + firstScaled, position - secondScaled, Color.yellow, time);
        Debug.DrawLine(position - firstScaled, position - secondScaled, Color.yellow, time);
        Debug.DrawLine(position - firstScaled, position + secondScaled, Color.yellow, time);
        Debug.DrawLine(position - firstScaled, position + secondScaled, Color.yellow, time);
        Debug.DrawLine(position, position + normal, Color.yellow, time);
    }
}