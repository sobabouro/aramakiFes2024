using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 連結辺シーケンス (LinkedVertex) のリストを管理するクラス
/// </summary>
public class LinkedVertexList : AbstractNodeSequenceList<LinkedVertex, NonConvexMonotoneCutSurfaceVertex> {

    /// <summary>
    /// インデクサー
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public LinkedVertex this[int index] {
        get {
            return _nodeSequenceList[index];
        }
    }

    /// <summary>
    /// 連結辺シーケンスに頂点を追加するメソッド
    /// </summary>
    /// <param name="args"> (NewVertex 始点, NewVertex 終点) </param>
    public override void Add(params object[] args) {

        if (args.Length != 2 || !(args[0] is NonConvexMonotoneCutSurfaceVertex toward) || !(args[1] is NonConvexMonotoneCutSurfaceVertex away)) {
            Debug.LogError("Add for LinkedVertexList requires two NewVertex arguments.");
            return;
        }

        for (int i = 0; i < _nodeSequenceList.Count; i++) {
            var current = _nodeSequenceList[i];

            if (current.TryAppend(toward, away)) {
                TryMergeAfter(i, away);
                return;
            }
            if (current.TryPrepend(toward, away)) {
                TryMergeBefore(i, toward);
                return;
            }
        }
        // どの連結辺にも追加できなかった場合は新しい連結辺を作成する
        LinkedVertex newLinked = new();
        newLinked.TryAppend(toward, away);
        _nodeSequenceList.Add(newLinked);
    }

    /// <summary>
    /// シーケンス同士のマージ可能性を判定するメソッド
    /// </summary>
    /// <param name="target"> あるシーケンスの頂点 </param>
    /// <param name="key"> Add() によって追加した新有向辺の対応する頂点 </param>
    /// <param name="isAfter"> 前後のどちらに対してマージを試みるかを示すフラグ </param>
    /// <returns></returns>
    protected override bool CheckMerge(NonConvexMonotoneCutSurfaceVertex target, NonConvexMonotoneCutSurfaceVertex key, bool isAfter) {
        if (target == null)
            return false;
        return target.Equals(key);
    }

    /// <summary>
    /// 連結辺シーケンスのリスト内のすべての新頂点たちに頂点種類のラベル付与を行う
    /// </summary>
    public void ClusteringVertexType() {
        foreach (var linkedVertex in _nodeSequenceList) {
            linkedVertex.ClusteringVertexType();
        }
    }

    /// <summary>
    /// this リスト内のすべての連結辺シーケンスに対して，総じた以下の処理を行う
    /// 辺を構成する頂点すべてを planePosition.y の降順でソートする
    /// planePositon.y が等しい場合には， planePosition.x の昇順でソートする
    /// </summary>
    /// <returns> ソート結果のリスト </returns>
    public List<NonConvexMonotoneCutSurfaceVertex> GetAllVertexSortedPlanePositionY() {


        foreach (var vertex in _nodeSequenceList) {
            foreach (var v in vertex.Vertices) {
                Debug.Log($"LinkedVertexList: Vertex Position = {v.LocalPosition}, PlanePosition = {v.PlanePosition}.");
            }
        }

        List<NonConvexMonotoneCutSurfaceVertex> sortedPlanePositionY = _nodeSequenceList
            .SelectMany(linkedVertex => linkedVertex.Vertices)
            .OrderByDescending(vertex => vertex.PlanePosition.y)
            .ThenBy(vertex => vertex.PlanePosition.x)
            .ToList();

        for (int i = 0; i < sortedPlanePositionY.Count; i++) {
            Debug.Log($"LinkedVertexList: Sorted Vertex {i} - Position = {sortedPlanePositionY[i].LocalPosition}, PlanePosition = {sortedPlanePositionY[i].PlanePosition}.");
        }

        return sortedPlanePositionY;
    }

    /// <summary>
    /// 指定された頂点の最も右側にある辺の始点を取得するメソッド
    /// </summary>
    /// <param name="vertex"> 指定された頂点 </param>
    /// <returns> 右側にある辺の始点 </returns>
    /// <exception cref="InvalidOperationException"> 右隣の辺が見つからない場合 </exception>
    public NonConvexMonotoneCutSurfaceVertex GetEdgeCoordinateNeighboring(NonConvexMonotoneCutSurfaceVertex vertex) {
        NonConvexMonotoneCutSurfaceVertex vertexOfNeighboringEdge = null;
        // 右隣の辺との距離の二乗
        float minSqrDistance = float.MaxValue;

        foreach (var linkedVertex in _nodeSequenceList) {
            // 連結辺が2つ未満の場合はスキップ (ここに入ることは想定しなくてよい)
            if (linkedVertex.Count < 2)
                continue;

            var currentNode = linkedVertex.First;

            for (int i = 0; i < linkedVertex.Count; i++) {
                var start = currentNode.Value;
                var end = currentNode.Next?.Value;

                // start が末尾の場合，先頭との辺を形成する
                if (currentNode.Next == null) {
                    end = linkedVertex.First?.Value;

                    // 無効な入力の場合，スキップ (ここに入ることは想定しなくてよい)
                    if (start == null || end == null || start.Equals(end)) {
                        currentNode = currentNode.Next;
                        continue;
                    }

                    // 前提条件 1: 辺の y 座標成分が増加もしく減少していること
                    bool isNonHorizonEdge = start.PlanePosition.y != end.PlanePosition.y;

                    // 前提条件 2: 入力頂点の Y 座標が辺の Y 範囲内にあること
                    bool isWithinRange = vertex.PlanePosition.y >= start.PlanePosition.y && vertex.PlanePosition.y < end.PlanePosition.y || vertex.PlanePosition.y <= start.PlanePosition.y && vertex.PlanePosition.y > end.PlanePosition.y;

                    // 条件を満たす場合のみ、左側の辺を探す
                    if (isNonHorizonEdge && isWithinRange) {
                        // 判定条件 1: 辺が入力頂点の左側にあること
                        bool isLeftSide = NonConvexMonotoneCutSurfaceVertex.IsEdgeLeftOfPoint(vertex, start, end);

                        if (isLeftSide) {
                            float currentSqrDistance = NonConvexMonotoneCutSurfaceVertex.DistancePointToEdge(vertex, start, end);

                            // 見つけた辺がより近ければ更新する
                            if (currentSqrDistance < minSqrDistance) {
                                minSqrDistance = currentSqrDistance;
                                vertexOfNeighboringEdge = start;
                            }
                        }
                    }
                    currentNode = currentNode.Next;
                }
            }
        }
        if (vertexOfNeighboringEdge != null)
            return vertexOfNeighboringEdge;
        throw new InvalidOperationException("No neighboring edge found for the given vertex.");
    }
}
