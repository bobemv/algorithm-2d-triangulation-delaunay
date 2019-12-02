using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Triangulation
{
 
    [SerializeField] private GameObject _verticePrefab;


    private Triangle GetSuperTriangle(Plane plane) {
        Triangle superTriangle = new Triangle();
        Vertice firstVertice = new Vertice(plane.firstVector * 666, -1);
        Vertice secondtVertice = new Vertice(plane.secondVector * 500 - plane.firstVector * 200, -2);
        Vertice thirdVertice = new Vertice(-plane.secondVector * 500 - plane.firstVector * 200, -3);

        superTriangle.first = new Edge(secondtVertice, firstVertice);
        superTriangle.second = new Edge(firstVertice, thirdVertice);
        superTriangle.third = new Edge(thirdVertice, secondtVertice);
        return superTriangle;
    }

    private int GetIndexDependingOnSuperTrianglePresent(int index, bool isTriangulationFinished) {
        if (!isTriangulationFinished) {
            if (index == -1) {
                return 0;
            }
            if (index == -2) {
                return 1;
            }
            if (index == -3) {
                return 2;
            }
            return index + 3;
        }
        
        return index;
    }

    private void ConvertTrianglesToMeshVerticesAndTriangles(List<Triangle> trianglesGenerated, List<Vertice> verticesGenerated, out Vector3[] meshVertices, out int[] meshTriangles, out Color[] meshColors, bool isTriangulationFinished) {
        int sizeMeshVertices = verticesGenerated.Count;
        int sizeMeshTriangles = trianglesGenerated.Count * 3;
        int sizeMeshColors = verticesGenerated.Count;

        meshVertices = new Vector3[sizeMeshVertices];
        meshTriangles = new int[sizeMeshTriangles];
        meshColors = new Color[sizeMeshColors];

        for (int i = 0; i < verticesGenerated.Count; i++) {
            //ADD SUPERTRIANGLE
            int indexParsed = GetIndexDependingOnSuperTrianglePresent(verticesGenerated[i].index, isTriangulationFinished);
            meshVertices[indexParsed] = verticesGenerated[i].position;

            if ((i % 3) == 0)
            {
                meshColors[i] = new Color(1, 0, 0, 1);
            }
            else if ((i % 3) == 1)
            {
                meshColors[i] = new Color(0, 1, 0, 1);
            }
            else if ((i % 3) == 2)
            {
                meshColors[i] = new Color(0, 0, 1, 1);
            }
        }

        for (int i = 0; i < trianglesGenerated.Count; i++) {
            int indexParsed = GetIndexDependingOnSuperTrianglePresent(trianglesGenerated[i].first.origin.index, isTriangulationFinished);
            meshTriangles[i * 3] = indexParsed;

            indexParsed = GetIndexDependingOnSuperTrianglePresent(trianglesGenerated[i].second.origin.index, isTriangulationFinished);
            meshTriangles[(i * 3) + 1] = indexParsed;

            indexParsed = GetIndexDependingOnSuperTrianglePresent(trianglesGenerated[i].third.origin.index, isTriangulationFinished);
            meshTriangles[(i * 3) + 2] = indexParsed;

        }
    }

    List<Vertice> GetVerticesForTriangulation(List<Vector3> positions, Triangle superTriangle) {
        int indexVertices = 0;
        List<Vertice> vertices = new List<Vertice>();

        vertices.Add(superTriangle.first.origin);
        vertices.Add(superTriangle.second.origin);
        vertices.Add(superTriangle.third.origin);

        positions.ForEach(position => {
            Vertice newVerticeMesh = new Vertice(position, indexVertices++);
            vertices.Add(newVerticeMesh);
        });

        return vertices;
    }

    int[] GetRandomNumberArray(int from, int to) {
        int count = to - from + 1;
        int[] randomNumberArray = new int[count];
        for (int i = from, j = 0; i <= to; i++, j++) {
            randomNumberArray[j] = i;
        }
        for (int j = count - 1; j > 0; j--) {
            int k = UnityEngine.Random.Range(0, j + 1);
            int temp = randomNumberArray[j];
            randomNumberArray[j] = randomNumberArray[k] ;
            randomNumberArray[k] = temp;
        }
        return randomNumberArray;
    }


    public IEnumerator BowyerWatson(List<Vector3> positions, Plane plane, Action<Vector3[], int[], Color[]> callbackReturnTriangulation, float stepTime, GameObject _verticePrefab) {
        Triangle superTriangle = GetSuperTriangle(plane);
        List<Vertice> vertices = GetVerticesForTriangulation(positions, superTriangle);
        List<Triangle> triangulation = new List<Triangle>();
        Vector3[] meshVertices;
        int[] meshTriangles;
        Color[] meshColors;

        triangulation.Add(superTriangle);
        if (stepTime != 0) {
            ConvertTrianglesToMeshVerticesAndTriangles(triangulation, vertices, out meshVertices, out meshTriangles, out meshColors, false);
            callbackReturnTriangulation(meshVertices, meshTriangles, meshColors);
            yield return new WaitForSeconds(stepTime);
        }

        int[] orderVerticesToBeInserted = GetRandomNumberArray(0, vertices.Count - 1);
        
        for (int i = 0; i < orderVerticesToBeInserted.Length; i++) {
            int pointIndex = orderVerticesToBeInserted[i];
            GameObject.Instantiate(_verticePrefab, vertices[pointIndex].position, Quaternion.identity);
            if (stepTime != 0) {
                ConvertTrianglesToMeshVerticesAndTriangles(triangulation, vertices, out meshVertices, out meshTriangles, out meshColors, false);
                callbackReturnTriangulation(meshVertices, meshTriangles, meshColors);
                yield return new WaitForSeconds(stepTime);
            }
            
            //DETECT BAD TRIANGLES (NEW POINT INSIDE THEIR CIRCUMCIRCLE)
            List<Triangle> badTriangles = new List<Triangle>();
            triangulation.ForEach(triangle => {
                if (isPointInsideCircumcircleTriangle(vertices[pointIndex].position, triangle)) {
                    badTriangles.Add(triangle);
                }
            });
            // ----

            if (stepTime != 0) {
                ConvertTrianglesToMeshVerticesAndTriangles(triangulation, vertices, out meshVertices, out meshTriangles, out meshColors, false);
                callbackReturnTriangulation(meshVertices, meshTriangles, meshColors);
                yield return new WaitForSeconds(stepTime);
            }
            // REMOVE ALL BAD TRIANGLES FROM CURRENT TRIANGULATION
            for (int badTriangleIndex = 0; badTriangleIndex < badTriangles.Count; badTriangleIndex++) {
                triangulation.Remove(badTriangles[badTriangleIndex]);
                if (stepTime != 0) {
                    ConvertTrianglesToMeshVerticesAndTriangles(triangulation, vertices, out meshVertices, out meshTriangles, out meshColors, false);
                    callbackReturnTriangulation(meshVertices, meshTriangles, meshColors);
                    yield return new WaitForSeconds(stepTime);
                }
            }
            // ----
            

            // DETECT WHICH IS THE HOLE LEFT BY THE BAD TRIANGLES REMOVED
            List<Edge> polygon = new List<Edge>();
            List<Edge> possibleNotSharedEdges = new List<Edge>();
            badTriangles.ForEach(triangle => {
                possibleNotSharedEdges.Add(triangle.first);
                possibleNotSharedEdges.Add(triangle.second);
                possibleNotSharedEdges.Add(triangle.third);
            });

            possibleNotSharedEdges.ForEach(possibleEdge => {
                if (possibleNotSharedEdges.FindAll(edge => Edge.IsSameVertices(possibleEdge, edge)).Count == 1) {
                    polygon.Add(possibleEdge);
                }
            });
            // ----

            polygon.ForEach(edge => {
                Debug.DrawLine(edge.origin.position, edge.end.position, Color.yellow, 2f);
            });

            if (stepTime != 0) {
                ConvertTrianglesToMeshVerticesAndTriangles(triangulation, vertices, out meshVertices, out meshTriangles, out meshColors, false);
                callbackReturnTriangulation(meshVertices, meshTriangles, meshColors);
                yield return new WaitForSeconds(stepTime);
            }

            // CREATE A NEW TRIANGLE FOR EACH EDGE IN THE HOLE (2 VERTICES FROM EDGE + 1 VERTICE THE ONE BEING INSERTED)
            for (int polygonIndex = 0; polygonIndex < polygon.Count; polygonIndex++) {
                Triangle newTri = new Triangle();
                newTri.first = polygon[polygonIndex];
                newTri.second = new Edge(vertices[pointIndex], polygon[polygonIndex].origin);
                newTri.third = new Edge(polygon[polygonIndex].end, vertices[pointIndex]);

                Vector3 normal = Vector3.Cross(newTri.first.GetEdgeVector(), newTri.second.GetEdgeVector());

                if (Vector3.Dot(normal, plane.normal) < 0) {
                    Edge aux = newTri.first;
                    newTri.first = newTri.third;
                    newTri.third = aux;
                }
                triangulation.Add(newTri);
                if (stepTime != 0) {
                    ConvertTrianglesToMeshVerticesAndTriangles(triangulation, vertices, out meshVertices, out meshTriangles, out meshColors, false);
                    callbackReturnTriangulation(meshVertices, meshTriangles, meshColors);
                    yield return new WaitForSeconds(stepTime);
                }
            }
            // ----
            

        }

        if (stepTime != 0) {
            ConvertTrianglesToMeshVerticesAndTriangles(triangulation, vertices, out meshVertices, out meshTriangles, out meshColors, false);
            callbackReturnTriangulation(meshVertices, meshTriangles, meshColors);
            yield return new WaitForSeconds(stepTime);
        }

        List<Triangle> finaltriangulation = new List<Triangle>();
        triangulation.ForEach(triangle => {
            if (!isSharedVerticeSuperTriangle(triangle)) {
                finaltriangulation.Add(triangle);
            }
        });


        triangulation = finaltriangulation;
        vertices.Remove(superTriangle.first.origin);
        vertices.Remove(superTriangle.second.origin);
        vertices.Remove(superTriangle.third.origin);

        ConvertTrianglesToMeshVerticesAndTriangles(triangulation, vertices, out meshVertices, out meshTriangles, out meshColors, true);
        callbackReturnTriangulation(meshVertices, meshTriangles, meshColors);
        yield return new WaitForSeconds(stepTime);

    }

    private bool isSharedVerticeSuperTriangle(Triangle triangle) {
        return triangle.first.origin.index == -1 || triangle.second.origin.index == -1 || triangle.third.origin.index == -1 ||
               triangle.first.origin.index == -2 || triangle.second.origin.index == -2 || triangle.third.origin.index == -2 ||
               triangle.first.origin.index == -3 || triangle.second.origin.index == -3 || triangle.third.origin.index == -3;
    }

    private bool isPointInsideCircumcircleTriangle(Vector3 point, Triangle triangle)
    {
        Vector3 ac = triangle.third.origin.position - triangle.first.origin.position;
        Vector3 ab = triangle.second.origin.position - triangle.first.origin.position;
        Vector3 abXac = Vector3.Cross(ab, ac);

        Vector3 toCircumsphereCenter = (Vector3.Cross(abXac, ab) * len2(ac) + Vector3.Cross(ac, abXac) * len2(ab)) / (2f * len2(abXac));
        float circumradius = toCircumsphereCenter.magnitude;
        Vector3 circumcenter = triangle.first.origin.position + toCircumsphereCenter;

        float distancePointToCircumcenter = Vector3.Distance(point, circumcenter);
        bool isPointInside = distancePointToCircumcenter < circumradius;

        return isPointInside;
    }

    private float len2(Vector3 v)
    {
        return v.x * v.x + v.y * v.y + v.z * v.z;
    }


}
