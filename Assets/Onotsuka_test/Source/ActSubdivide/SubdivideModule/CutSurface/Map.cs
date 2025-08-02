using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map {

    /// <summary>
    /// 各頂点において，その頂点から接続するすべての辺へのマッピング
    /// </summary>
    private readonly Dictionary<NonConvexMonotoneCutSurfaceVertex, List<NonConvexMonotoneCutSurfaceEdge>> _map;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public Map() {
        _map = new Dictionary<NonConvexMonotoneCutSurfaceVertex, List<NonConvexMonotoneCutSurfaceEdge>>();
    }

    /// <summary>
    /// グラフに無向辺を追加する
    /// </summary>
    /// <param name="edge"> 追加する辺 </param>"
    public void AddEdge(NonConvexMonotoneCutSurfaceEdge edge) {

        AddDirectedEdgeToMap(edge);
        AddDirectedEdgeToMap(edge.GetReverseEdge());
    }

    /// <summary>
    /// 指定された始点から終点への有向辺をマップに追加する
    /// </summary>
    /// <param name="edge"> 追加する有向辺 </param>
    private void AddDirectedEdgeToMap(NonConvexMonotoneCutSurfaceEdge edge) {

        // 始点のリストが存在しない場合は新規作成する
        if (!_map.ContainsKey(edge.Start)) 
            _map[edge.Start] = new List<NonConvexMonotoneCutSurfaceEdge>();

        // 既に存在する辺でない場合のみ追加する
        if (!_map[edge.Start].Contains(edge)) {
            _map[edge.Start].Add(edge);
        }
    }

    /// <summary>
    /// 指定された頂点に接続する辺のリストを取得する
    /// </summary>
    /// <remarks>
    /// リストは最も左折する順，もしくは最も右折する順でソートされる <br/>
    /// 右折と左折の決定は，前の頂点からの入射ベクトルに対して，次の辺がどのように曲折するかで行う
    /// </remarks>
    /// <param name="currVertex"> 現在の頂点 </param>
    /// <param name="prevVertex"> 前の頂点 </param>
    /// <returns> ソート済みの辺のリスト </returns>
    public List<NonConvexMonotoneCutSurfaceEdge> GetSortedEdgesFromIncomingVector(
        NonConvexMonotoneCutSurfaceVertex currVertex,
        NonConvexMonotoneCutSurfaceVertex prevVertex
    ) {
        if (!_map.TryGetValue(currVertex, out var edges)) {
            return new List<NonConvexMonotoneCutSurfaceEdge>();
        }

        Vector2 incomingVector = prevVertex == null || prevVertex.Equals(currVertex)
                ? Vector2.right
                : currVertex.PlanePosition - prevVertex.PlanePosition;

        bool hasLeftTurn = HasLeftTurnEdge(incomingVector, edges, out var edgeListHasAngleTag);

        return SortEdgesByAngle(hasLeftTurn, edgeListHasAngleTag);
    }

    /// <summary>
    /// 指定された入射ベクトルに対して，左折する辺が存在するかを判定する
    /// </summary>
    /// <param name="incomingVector"> 入射ベクトル </param>
    /// <param name="edges"> 判定対象の辺リスト </param>
    /// <param name="edgeListHasAngleTag"> 角度情報を持った辺リスト </param>
    /// <returns> 左折する辺があれば true, そうでなければ false を返す </returns>
    private bool HasLeftTurnEdge(
        Vector2 incomingVector,
        List<NonConvexMonotoneCutSurfaceEdge> edges,
        out List<(float angle, NonConvexMonotoneCutSurfaceEdge edge)> edgeListHasAngleTag
    ) {
        bool hasLeftTurn = false;
        edgeListHasAngleTag = new List<(float angle, NonConvexMonotoneCutSurfaceEdge edge)>();

        foreach (var edge in edges) {
            Vector2 nextVector = edge.End.PlanePosition - edge.Start.PlanePosition;
            float angle = Vector2.SignedAngle(incomingVector, nextVector);

            // 角度が180度の場合は例外として左右判定には使用しない
            if (Mathf.Abs(angle) > 179.9f) {
                edgeListHasAngleTag.Add((angle, edge));
                continue;
            }

            if (angle > 0) {
                hasLeftTurn = true;
            }
            edgeListHasAngleTag.Add((angle, edge));
        }
        return hasLeftTurn;
    }

    /// <summary>
    /// 辺リストを右左折判定情報と角度に基づいてソートする
    /// </summary>
    /// <param name="hasLeftTurn"> 左側に辺を所持するかどうか </param>
    /// <param name="edgeListHasAngleTag"> 角度情報を持った辺リスト </param>
    /// <returns> ソートされた辺リスト </returns>
    private List<NonConvexMonotoneCutSurfaceEdge> SortEdgesByAngle(
        bool hasLeftTurn,
        List<(float angle, NonConvexMonotoneCutSurfaceEdge edge)> edgeListHasAngleTag
    ) {
        if (hasLeftTurn) {
            return edgeListHasAngleTag
                .OrderByDescending(item => Mathf.Abs(item.angle) > 179.9f ? -181f : item.angle)
                .Select(item => item.edge).ToList();
        } 
        else {
            return edgeListHasAngleTag
                .OrderBy(item => Mathf.Abs(item.angle) > 179.9f ? 181f : item.angle)
                .Select(item => item.edge).ToList();
        }
    }

    /// <summary>
    /// グラフ内のすべての頂点を取得する
    /// </summary>
    public IEnumerable<NonConvexMonotoneCutSurfaceVertex> GetAllKeys() {
        return _map.Keys;
    }
}
