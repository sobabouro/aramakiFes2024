using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSurfacePolygonBuffer {

    private LinkedVertexList _linkedVertexList = new();

    /// <summary>
    /// y 座標降順でソートされた，図形平面上すべての頂点リスト
    /// いわゆる，三角形分割のためのイベントポイントとなる頂点が詰まっている
    /// </summary>
    private List<NonConvexMonotoneCutSurfaceVertex> _sortedVertexList = new();

    private Dictionary<NonConvexMonotoneCutSurfaceVertex, (NonConvexMonotoneCutSurfaceVertex, NonConvexMonotoneCutSurfaceVertex)> _diagonalEdgeDictionary = new();

    public void AddVertex(
        Plane localPlane,
        Vector3 towardPosition,
        Vector3 awayPosition
    ) {
        NonConvexMonotoneCutSurfaceVertex towardCutSurfaceVertex = new(localPlane, towardPosition);
        NonConvexMonotoneCutSurfaceVertex awayCutSurfaceVertex = new(localPlane, awayPosition);

        _linkedVertexList.Add(towardCutSurfaceVertex, awayCutSurfaceVertex);
    }

    public void MakeCutSurfacePolygon() {
        _linkedVertexList.ClusteringVertexType();
        _sortedVertexList = _linkedVertexList.GetAllVertexSortedPlanePositionY();
    }

    private void HandleRegularVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        int processedVertexIndex
    ) {
        NonConvexMonotoneCutSurfaceVertex nextVertex = processedVertexIndex < _sortedVertexList.Count - 1
            ? _sortedVertexList[processedVertexIndex + 1]
            : _sortedVertexList[0];

        // 対象の頂点が始点の辺が，y に減少して無ければ処理対象外 (つまり図形の内部が対象頂点の右に無ければ対象外)
        if (vertex.PlanePosition.y < nextVertex.PlanePosition.y)
            return;

        if (_sortedVertexList[processedVertexIndex - 1].Helper.VertexType == VertexType.Merge) {
            _diagonalEdgeDictionary.Add(
                (vertex),
                (vertex, vertex.Helper)
            );
            _diagonalEdgeDictionary.Add(
                (vertex.Helper),
                (vertex.Helper, vertex)
            );
        }

    }

    private void HandleStartVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        int processedVertexIndex
    ) {

    }

    private void HandleMergeVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        int processedVertexIndex
    ) {
        if (_sortedVertexList[processedVertexIndex - 1].Helper.VertexType == VertexType.Merge) {
            _diagonalEdgeDictionary.Add(
                (vertex),
                (vertex, vertex.Helper)
            );
            _diagonalEdgeDictionary.Add(
                (vertex.Helper),
                (vertex.Helper, vertex)
            );
        }
        // すぐ左隣の辺を探す
        NonConvexMonotoneCutSurfaceVertex leftNeighbor = _linkedVertexList.GetEdgeCoordinateNeighboring(vertex);
        if (leftNeighbor != null) {
            if (leftNeighbor.Helper.VertexType == VertexType.Merge) {
                _diagonalEdgeDictionary.Add(
                    (vertex),
                    (vertex, leftNeighbor.Helper)
                );
                _diagonalEdgeDictionary.Add(
                    (leftNeighbor.Helper),
                    (leftNeighbor.Helper, vertex)
                );
            }
            leftNeighbor.Helper = vertex;
        }
    }


    private void HandleSplitVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        int processedVertexIndex
    ) {
        NonConvexMonotoneCutSurfaceVertex leftNeighbor = _linkedVertexList.GetEdgeCoordinateNeighboring(vertex);
        if (leftNeighbor != null) {
            _diagonalEdgeDictionary.Add(
                (vertex),
                (vertex, leftNeighbor.Helper)
            );
            _diagonalEdgeDictionary.Add(
                (leftNeighbor.Helper),
                (leftNeighbor.Helper, vertex)
            );
            leftNeighbor.Helper = vertex;
        }
    }
    private void HandleEndVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        int processedVertexIndex
    ) {
        if (_sortedVertexList[processedVertexIndex - 1].Helper.VertexType == VertexType.Merge) {
            _diagonalEdgeDictionary.Add(
                (vertex),
                (vertex, vertex.Helper)
            );
            _diagonalEdgeDictionary.Add(
                (vertex.Helper),
                (vertex.Helper, vertex)
            );
        }
    }
}
