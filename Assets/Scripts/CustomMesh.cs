using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomMesh : MonoBehaviour
{
    [SerializeField] public List<Vector3> pointsToTriangulate;
    [SerializeField] public GameObject _verticePrefab;
    [SerializeField] private float _speed;
    [SerializeField] GameObject _panelUI, _showPanelUI;
    [SerializeField] Text _numVerticesTextUI;
    [SerializeField] Slider _speedSliderUI;
    public Plane plane;
    public Mesh mesh;

    private int numVertices = 0;
    private float speed = 1;
    private Coroutine triangulation;

    void CallbackTrianglesFromTriangulation(Vector3[] positions, int[] triangulation, Color[] colors) {
        Mesh auxMesh = new Mesh();
        auxMesh.vertices = positions;
        auxMesh.triangles = triangulation;
        auxMesh.colors = colors;

        auxMesh.RecalculateNormals();

        GetComponent<MeshFilter>().mesh = auxMesh;
    }



    List<Vector3> GeneratePositions() {
        List<Vector3> positions = new List<Vector3>();
        positions = new List<Vector3>();
        
        for (int i = 0; i < numVertices; i++)
        {
            positions.Add(new Vector3(Random.Range(-8.73f, 8.73f), 0, Random.Range(-4.43f, 4.43f)));
        }

        return positions;
    }

    public void StartTriangulation()
    {
        if (triangulation != null)
        {
            StopCoroutine(triangulation);
            triangulation = null;
        }
        GameObject[] vertices = GameObject.FindGameObjectsWithTag("Vertice");
        for (int i = 0; i < vertices.Length; i++)
        {
            Destroy(vertices[i]);
        }
        GeneratePositions();
        plane = new Plane(new Vector3(0, 0, 1), new Vector3(1, 0, 0));
        pointsToTriangulate = GeneratePositions();
        Triangulation triangulationAlgorithm = new Triangulation();
        triangulation = StartCoroutine(triangulationAlgorithm.BowyerWatson(pointsToTriangulate, plane, CallbackTrianglesFromTriangulation, speed, _verticePrefab));
    }

    public void AddVertice()
    {
        numVertices++;
        _numVerticesTextUI.text = numVertices.ToString();
    }

    public void RemoveVertice()
    {
        numVertices--;
        if (numVertices < 0)
        {
            numVertices = 0;
        }
        _numVerticesTextUI.text = numVertices.ToString();
    }

    public void ShowUI()
    {
        _panelUI.SetActive(true);
        _showPanelUI.SetActive(false);
    }

    public void HideUI()
    {
        _panelUI.SetActive(false);
        _showPanelUI.SetActive(true);
    }

    public void SetSpeed()
    {
        speed = _speedSliderUI.value;
    }



}
