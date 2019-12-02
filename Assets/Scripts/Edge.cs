using UnityEngine;
public class Edge {
    public Edge (Vertice _origin, Vertice _end) {
        origin = _origin;
        end = _end;
    }
    public Vertice origin;
    public Vertice end;

    public float MagnitudeEdge() {
        return Vector3.Distance(origin.position, end.position);
    }

    public Vector3 GetEdgeVector() {
        return end.position - origin.position;
    }

    static public bool IsSameVertices(Edge e1, Edge e2) {
        return (e1.origin.index == e2.origin.index || e1.origin.index == e2.end.index) && (e1.end.index == e2.origin.index || e1.end.index == e2.end.index);
    }
}