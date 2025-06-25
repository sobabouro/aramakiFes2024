using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 連結辺の情報を保持するクラス
/// 連結辺は切断辺の始点から終点の順を正方向として連結する
/// </summary>
public class LinkedVertex : AbstractNodeSequence<NonConvexMonotoneCutSurfaceVertex> {

    /// <summary>
    /// 連結辺シーケンスのコレクションを取得するプロパティ
    /// </summary>
    public IEnumerable<NonConvexMonotoneCutSurfaceVertex> Vertices => GetItemsEnumerable();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public LinkedVertex() : base(new DeleteDuplicateMergeStrategy<NonConvexMonotoneCutSurfaceVertex>()) { }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="mergeStrategy"> マージメソッド戦略 </param>
    public LinkedVertex(INodeSequenceMergeStrategy<NonConvexMonotoneCutSurfaceVertex> mergeStrategy) : base(mergeStrategy) { }

    /// <summary>
    /// 対象の辺を連結辺に対して後ろに追加するメソッド
    /// </summary>
    /// <param name="args"> (NonConvexMonotoneCutSurfaceVertex 始点, NonConvexMonotoneCutSurfaceVertex 終点) の引数 </param>
    /// <returns> 追加に成功した場合は true, 失敗した場合は false </returns>
    public override bool TryAppend(params object[] args) {
        if (args.Length != 2 || !(args[0] is NonConvexMonotoneCutSurfaceVertex toward) || !(args[1] is NonConvexMonotoneCutSurfaceVertex away)) {
            Debug.LogError("TryAppend for LinkedVertex requires two NonConvexMonotoneCutSurfaceVertex arguments.");
            return false;
        }
        var value = _nodeSequence.Last?.Value;
        if (value != null && value.Equals(toward)) {
            _nodeSequence.AddLast(away);

            return true;
        }
        if (First == null) {
            _nodeSequence.AddFirst(toward);
            _nodeSequence.AddLast(away);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 対象の辺を連結辺に対して前に追加するメソッド
    /// </summary>
    /// <param name="args"> (NewVertex 始点, NewVertex 終点) の引数 </param>
    /// <returns> 追加に成功した場合は true, 失敗した場合は false </returns>
    public override bool TryPrepend(params object[] args) {
        if (args.Length != 2 || !(args[0] is NonConvexMonotoneCutSurfaceVertex toward) || !(args[1] is NonConvexMonotoneCutSurfaceVertex away)) {
            Debug.LogError("TryAppend for LinkedVertex requires two NonConvexMonotoneCutSurfaceVertex arguments.");
            return false;
        }
        var value = _nodeSequence.First?.Value;
        if (value != null && value.Equals(away)) {
            _nodeSequence.AddFirst(toward);

            return true;
        }
        return false;
    }

    /// <summary>
    /// 連結辺シーケンスのリスト内のすべての新頂点たちに頂点種類のラベル付与を行う
    /// </summary>
    public void ClusteringVertexType() {
        // シーケンスの最後の要素を削除する (始点と終点が重複しているため)
        _nodeSequence.Remove(_nodeSequence.Last);

        var currentNode = _nodeSequence.First;
        while (currentNode != null) {
            // 循環リストのようなイメージの走査を行うため，先頭と末尾は特別扱いする
            NonConvexMonotoneCutSurfaceVertex prevVertex = currentNode.Previous?.Value ?? _nodeSequence.Last.Value;
            NonConvexMonotoneCutSurfaceVertex currVertex = currentNode.Value;
            NonConvexMonotoneCutSurfaceVertex nextVertex = currentNode.Next?.Value ?? _nodeSequence.First.Value;

            SetVertexType(prevVertex, currVertex, nextVertex);
            currentNode = currentNode.Next;
        }
    }

    /// <summary>
    /// 頂点の種類を設定するメソッド
    /// </summary>
    /// <param name="prevVertex"> 連続する頂点 1 </param>
    /// <param name="currVertex"> 連続する頂点 2 </param>
    /// <param name="nextVertex"> 連続する頂点 3 </param>
    private void SetVertexType(NonConvexMonotoneCutSurfaceVertex prevVertex, NonConvexMonotoneCutSurfaceVertex currVertex, NonConvexMonotoneCutSurfaceVertex nextVertex) {
        float prevY = prevVertex.PlanePosition.y;
        float currY = currVertex.PlanePosition.y;
        float nextY = nextVertex.PlanePosition.y;

        if (currY > prevY && currY >= nextY) {
            currVertex.VertexType = IsAcuteOrObtuseAngle(prevVertex, currVertex, nextVertex)
                ? VertexType.Start
                : VertexType.Split;
        } else if (currY < prevY && currY <= nextY) {
            currVertex.VertexType = IsAcuteOrObtuseAngle(prevVertex, currVertex, nextVertex)
                ? VertexType.End
                : VertexType.Merge;
        } else {
            currVertex.VertexType = VertexType.Regular;
        }
    }

    /// <summary>
    /// 対象の頂点 2 (current) を中心とした，二辺の成す角が鋭角か鈍角かを判定するメソッド
    /// </summary>
    /// <param name="prevVertex"> 連続する頂点 1 </param>
    /// <param name="currVertex"> 連続する頂点 2 </param>
    /// <param name="nextVertex"> 連続する頂点 3 </param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"> 連続する三頂点が平行の時 </exception>
    private bool IsAcuteOrObtuseAngle(NonConvexMonotoneCutSurfaceVertex prevVertex, NonConvexMonotoneCutSurfaceVertex currVertex, NonConvexMonotoneCutSurfaceVertex nextVertex) {
        Vector2 v1 = (prevVertex.PlanePosition - currVertex.PlanePosition).normalized;
        Vector2 v2 = (prevVertex.PlanePosition - currVertex.PlanePosition).normalized;

        float dot = Vector2.Dot(v1, v2);

        // 鋭角
        if (dot > 0)
            return true;
        // 鈍角
        if (dot < 0)
            return false;
        // 平行の場合は例外を投げる
        throw new InvalidOperationException("The angle is exactly 90 degrees.");
    }

    public void Display() {
        foreach (var vertex in _nodeSequence) {
            Debug.Log($"LinkedVertex: local position - {vertex.LocalPosition}, plane position - {vertex.PlanePosition}");
        }
    }
}
