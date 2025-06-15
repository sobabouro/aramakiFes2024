using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 連結ポリゴン (LinkedPolygon) のリストを管理するクラス
/// </summary>
public class LinkedPolygonList {

    /// <summary>
    /// 連結ポリゴンのリスト
    /// </summary>
    private readonly List<LinkedPolygon> _linkedPolygonList = new List<LinkedPolygon>();

    /// <summary>
    /// 連結ポリゴンの数を取得するプロパティ
    /// </summary>
    public int Count {
        get {
            return _linkedPolygonList.Count;
        }
    }
    /// <summary>
    /// リストに新しいポリゴンを追加するメソッド
    /// </summary>
    /// <param name="target"> 追加するポリゴン </param>
    public void Add(NewPolygon target) {
        for (int i = 0; i < _linkedPolygonList.Count; i++) {
            var current = _linkedPolygonList[i];

            if (current.TryAppend(target)) {
                TryMergeAfter(i, target.NewEdgeAway.Domein);
                return;
            }
            if (current.TryPrepend(target)) {
                TryMergeBefore(i, target.NewEdgeToward.Domein);
                return;
            }
        }

        // どの連結ポリゴンにも追加できなかった場合は新しい連結ポリゴンを作成する
        var newLinked = new LinkedPolygon();
        newLinked.TryAppend(target);
        _linkedPolygonList.Add(newLinked);
    }

    /// <summary>
    /// 連結ポリゴンに対して、ポリゴン生成を行うメソッド
    /// </summary>
    /// <param name="localPlane"> 切断平面 </param>
    /// <param name="trackerArray"> 切断後の配属頂点インデックスを格納する配列 </param>
    /// <param name="originMesh"> 切断前メッシュ (MeshContainer) </param>
    /// <param name="frontsideMesh"> 切断後法線側メッシュ (MeshContainer) </param>
    /// <param name="backsideMesh"> 切断後反法線側メッシュ (MeshContainer) </param>
    public void MakePolygon(
        Plane localPlane,
        int[] trackerArray,
        MeshContainer originMesh,
        MeshContainer frontsideMesh,
        MeshContainer backsideMesh,
        CutSurfaceVertexBuffer cutSurfaceVertexBuffer
    ) {
        foreach (var linkedPolygon in _linkedPolygonList) {
            if (linkedPolygon.First == null || linkedPolygon.Last == null)
                continue;
            linkedPolygon.MakePolygon(
                localPlane,
                trackerArray,
                originMesh,
                frontsideMesh,
                backsideMesh,
                cutSurfaceVertexBuffer
            );
        }
    }

    /// <summary>
    /// リスト中の指定されたインデックスの連結ポリゴンに対して、後ろにマージできる連結ポリゴンを探してマージを試みるメソッド
    /// Add() メソッドの前ループで、TryPrepend() で、追加ポリゴンの前への連結判定は済んでいるので、処理対象の連結ポリゴンよりも後ろの要素のみマージ判定を行えばよい
    /// </summary>
    /// <param name="index"> 連結ポリゴンのリストインデックス </param>
    /// <param name="edgeEnd"> 連結ポリゴンの末尾の終点頂点ドメイン </param>
    private void TryMergeAfter(int index, uint edgeEnd) {
        for (int j = _linkedPolygonList.Count - 1; j > index; j--) {
            if (_linkedPolygonList[j].First?.Value.NewEdgeToward.Domein == edgeEnd) {
                _linkedPolygonList[index].MergeAfter(_linkedPolygonList[j]);
                _linkedPolygonList.RemoveAt(j);
            }
        }
    }

    /// <summary>
    /// リスト中の指定されたインデックスの連結ポリゴンに対して、前にマージできる連結ポリゴンを探してマージを試みるメソッド
    /// Add() メソッドの前ループで、TryAppend() で、追加ポリゴンの後ろへの連結判定は済んでいるので、処理対象の連結ポリゴンよりも後ろの要素のみマージ判定を行えばよい
    /// </summary>
    /// <param name="index"> 連結ポリゴンのインデックス </param>
    /// <param name="edgeStart"> 連結ポリゴンの先頭の始点頂点ドメイン </param>
    private void TryMergeBefore(int index, uint edgeStart) {
        for (int j = _linkedPolygonList.Count - 1; j > index; j--) {
            if (_linkedPolygonList[j].Last?.Value.NewEdgeAway.Domein == edgeStart) {
                _linkedPolygonList[index].MergeBefore(_linkedPolygonList[j]);
                _linkedPolygonList.RemoveAt(j);
            }
        }
    }
}
