using UnityEngine;
public class Vertice {
    public Vertice(Vector3 _position, int _index) {
        position = _position;
        index = _index;
    }
    public Vector3 position;
    public int index;

    override public string ToString() {
        return index + " " + position.ToString("#.00000");
    }
}