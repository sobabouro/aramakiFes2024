using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSurfacePolygonBuffer {

    private LinkedVertexList _linkedVertexList = new();

    public void AddVertex(
        Plane localPlane,
        Vector3 towardPosition,
        Vector3 awayPosition
    ) {
        CutSurfaceVertex towardCutSurfaceVertex = new(localPlane, towardPosition);
        CutSurfaceVertex awayCutSurfaceVertex = new(localPlane, awayPosition);

        _linkedVertexList.Add(towardCutSurfaceVertex, awayCutSurfaceVertex);
    }

    private void ClusteringVertexType() {
        FormatteList();

        // 連結頂点のリストを走査して、同じ位置にある頂点をクラスタリングする
        foreach (var linkedVertex in _linkedVertexList) {
        }

        void FormatteList() {

            // リストの中の各シーケンスの最後の要素を削除する (始点と終点が重複しているため)
            foreach (var linkedVertex in _linkedVertexList) {
                var lastNode = linkedVertex.Last;
                if (lastNode != null) {
                    linkedVertex.RemoveNode(lastNode);
                }
            }
        }

    }
}
