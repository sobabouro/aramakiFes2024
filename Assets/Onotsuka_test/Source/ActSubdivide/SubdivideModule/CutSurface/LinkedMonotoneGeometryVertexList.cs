using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 連結図形辺シーケンスのリスト (LinkedMonotoneGeometryVertex) を管理するクラス
/// </summary>
public class LinkedMonotoneGeometryVertexList : AbstractNodeSequenceList<LinkedMonotoneGeometryVertex, NonConvexMonotoneCutSurfaceVertex> {

    /// <summary>
    /// すべての図形の中で最も高い X 座標を持つ頂点の位置
    /// UV 座標の構築に使用する
    /// </summary>
    private BoundingBox _boundingBox = new BoundingBox();

    /// <summary>
    /// 連結図形辺シーケンスに頂点を追加するメソッド
    /// </summary>
    /// <param name="args"> (NewVertex 始点, NewVertex 終点) </param>
    public override void Add(params object[] args) {

        if (args.Length != 2 || !(args[0] is NonConvexMonotoneCutSurfaceVertex toward) || !(args[1] is NonConvexMonotoneCutSurfaceVertex away)) {
            Debug.LogError("Add for LinkedVertexList requires two NewVertex arguments.");
            return;
        }

        bool isUpdated = false;

        for (int i = 0; i < _nodeSequenceList.Count; i++) {
            var current = _nodeSequenceList[i];

            if (current.TryAppend(toward, away)) {

                isUpdated = TryUpdateMostHighestLowestPosition(away);

                TryMergeAfter(i, away);
                return;
            }
            if (current.TryPrepend(toward, away)) {

                isUpdated = TryUpdateMostHighestLowestPosition(toward);

                TryMergeBefore(i, toward);
                return;
            }
        }
        // どの連結辺にも追加できなかった場合は新しい連結辺を作成する
        LinkedMonotoneGeometryVertex newLinked = new();
        newLinked.TryAppend(toward, away);
        _nodeSequenceList.Add(newLinked);

        if (!isUpdated) {
            TryUpdateMostHighestLowestPosition(toward);
            TryUpdateMostHighestLowestPosition(away);
        }
    }

    /// <summary>
    /// シーケンス同士のマージ可能性を判定するメソッド
    /// </summary>
    /// <param name="target"> あるシーケンスの頂点 </param>
    /// <param name="key"> Add() によって追加した新有向辺の対応する頂点 </param>
    /// <param name="isAfter"> 前後のどちらに対してマージを試みるかを示すフラグ </param>
    /// <returns> マージ可能であれば，true, そうでなければ false </returns>
    protected override bool CheckMerge(NonConvexMonotoneCutSurfaceVertex target, NonConvexMonotoneCutSurfaceVertex key, bool isAfter) {

        if (target == null)
            return false;
        return target.Equals(key);
    }

    /// <summary>
    /// 連結図形辺シーケンスに対して、ポリゴン生成を行うメソッド
    /// </summary>
    /// <param name="localPlane"> 切断平面 </param>
    /// <param name="frontsideMesh"> 切断後法線側メッシュ (MeshContainer) </param>
    /// <param name="backsideMesh"> 切断後反法線側メッシュ (MeshContainer) </param>
    /// <param name="addCutSurfaceMaterial"> 追加する切断面のマテリアルがあるかどうか (あるならば true) </param>
    public void MakePolygon(
        Plane localPlane,
        MeshContainer frontsideMesh,
        MeshContainer backsideMesh,
        bool addCutSurfaceMaterial = false
    ) {
        foreach (var linkedMonotoneGeometryVertex in _nodeSequenceList) {
            if (linkedMonotoneGeometryVertex.First == null || linkedMonotoneGeometryVertex.Last == null)
                continue;
            linkedMonotoneGeometryVertex.MakePolygon(
                _boundingBox,
                localPlane,
                frontsideMesh,
                backsideMesh,
                addCutSurfaceMaterial
            );
        }
    }

    /// <summary>
    /// 切断面のバウンディングボックスを更新するためのメソッド
    /// x, y 座標の最も高い位置と最も低い位置を更新する
    /// </summary>
    /// <param name="vertex"> 追加する頂点 </param>
    /// <returns> 更新されれば ture, そうでなければ false </returns>
    private bool TryUpdateMostHighestLowestPosition(NonConvexMonotoneCutSurfaceVertex vertex) {
        if (_boundingBox.TryUpdate(vertex.PlanePosition.x, vertex.PlanePosition.y))
            return true;
        return false;
    }
}
