using UnityEngine;
public class Triangle {
    public Edge first;
    public Edge second;
    public Edge third;

    public void DrawTriangle() {
        Debug.DrawLine(first.origin.position, first.end.position, Color.white, 3f);
        Debug.DrawLine(second.origin.position, second.end.position, Color.white, 3f);
        Debug.DrawLine(third.origin.position, third.end.position, Color.white, 3f);
    }

    public string toString() {
        return "(" + first.origin.position.ToString("#.00000") + ", " + second.origin.position.ToString("#.00000") + ", " + third.origin.position.ToString("#.00000") + ")";
    }
}