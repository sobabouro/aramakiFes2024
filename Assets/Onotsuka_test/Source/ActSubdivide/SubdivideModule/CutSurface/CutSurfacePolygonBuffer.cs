using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CutSurfacePolygonBuffer {

    private LinkedVertexList _linkedVertexList = new();

    private LinkedMonotoneGeometryVertexList _linkedMonotoneGeometryVertexList = new();
    public void AddVertex(
        Plane localPlane,
        Vector3 towardPosition,
        Vector3 awayPosition
    ) {
        NonConvexMonotoneCutSurfaceVertex towardCutSurfaceVertex = new(localPlane, towardPosition);
        NonConvexMonotoneCutSurfaceVertex awayCutSurfaceVertex = new(localPlane, awayPosition);

        _linkedVertexList.Add(towardCutSurfaceVertex, awayCutSurfaceVertex);
    }

    public void MakeCutSurfacePolygon(
        Plane localPlane,
        MeshContainer frontsideMesh,
        MeshContainer backsideMesh, 
        bool addCutSurfaceMaterial = false
    ) {
        MakeMonotoneGeometry();

        //_linkedMonotoneGeometryVertexList.MakePolygon(
        //    localPlane,
        //    frontsideMesh,
        //    backsideMesh,
        //    addCutSurfaceMaterial
        //);
    }

    public void MakeMonotoneGeometry() {
        DiagonalEdgeGenerator diagonalEdgeGenerator = new DiagonalEdgeGenerator(_linkedVertexList);

        List<List<NonConvexMonotoneCutSurfaceEdge>> edgesList = diagonalEdgeGenerator.GetEdgeList();
        List<(NonConvexMonotoneCutSurfaceVertex, NonConvexMonotoneCutSurfaceVertex)> diagonalList = diagonalEdgeGenerator.GetDiagonalEdges();

        // ここで，_linkedVertexList 内の図形を _diagonalSet を用いて分割し，複数の図形に分ける
        // その後，分割された図形それぞれのポリゴンを作成する
        while (diagonalList.Count > 0) {
            var diagonal = diagonalList[diagonalList.Count - 1];
            diagonalList.RemoveAt(diagonalList.Count - 1);

            _linkedMonotoneGeometryVertexList.Add(diagonal.Item1, diagonal.Item2);
            bool isFound = false;

            foreach (var edges in edgesList) {
                foreach (var edge in edges) {
                    if (edge.End.Equals(diagonal.Item1)) {
                        _linkedMonotoneGeometryVertexList.Add(edge.Start, edge.End);
                        edges.Remove(edge);
                        isFound = true;
                        break;
                    }
                }
                if (isFound)
                    break;
            }
        }

        foreach (var edges in edgesList) {
            foreach (var edge in edges) {
                _linkedMonotoneGeometryVertexList.Add(edge.Start, edge.End);
            }
        }

        foreach (var linkedVertex in _linkedMonotoneGeometryVertexList) {
            linkedVertex.DeleteLastElement();
        }

        for (int i = 0; i < _linkedMonotoneGeometryVertexList.Count; i++) {
            Debug.Log($"CutSurfacePolygonBuffer: LinkedMonotoneGeometryVertexList[{i}] -");

            foreach (var vertex in _linkedMonotoneGeometryVertexList[i]) {
                Debug.Log($"  Vertex: {vertex.PlanePosition}, Type: {vertex.VertexType}");
            }
        }
    }
}
