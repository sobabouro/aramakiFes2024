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

        var vertex1 = edge.Start;
        var vertex2 = edge.End;

        AddDirectedEdgeToMap(vertex1, vertex2);
        AddDirectedEdgeToMap(vertex2, vertex1);
    }

    /// <summary>
    /// 指定された始点から終点への有向辺をマップに追加する
    /// </summary>
    /// <param name="start"> 始点 </param>
    /// <param name="end"> 終点 </param>
    private void AddDirectedEdgeToMap(NonConvexMonotoneCutSurfaceVertex start, NonConvexMonotoneCutSurfaceVertex end) {

        // 始点のリストが存在しない場合は新規作成する
        if (!_map.ContainsKey(start)) 
            _map[start] = new List<NonConvexMonotoneCutSurfaceEdge>();
        var edge = new NonConvexMonotoneCutSurfaceEdge(start, end);

        // 既に存在する辺でない場合のみ追加する
        if (!_map[start].Contains(edge)) {
            _map[start].Add(edge);
        }
    }

    /// <summary>
    /// 指定された頂点に接続する辺のリストを取得する
    /// リストは最も左折する順でソートされる
    /// </summary>
    /// <param name="currVertex"> 現在の頂点 </param>
    /// <param name="prevVertex"> 前の頂点 </param>
    /// <returns> 最も左折する順でソート済みの辺のリスト </returns>
    public List<NonConvexMonotoneCutSurfaceEdge> GetSortedEdgesFromVertex(
        NonConvexMonotoneCutSurfaceVertex currVertex, 
        NonConvexMonotoneCutSurfaceVertex prevVertex
    ) {
        if (_map.TryGetValue(currVertex, out var edges)) {

            Vector2 incomingVector = prevVertex == null || prevVertex.Equals(currVertex)
                ? Vector2.right
                : (currVertex.PlanePosition - prevVertex.PlanePosition).normalized;

            return edges.OrderByDescending(nextEdge => {
                Vector2 nextVector = nextEdge.End.PlanePosition - nextEdge.Start.PlanePosition;
                return Vector2.SignedAngle(incomingVector, nextVector);
            }).ToList();
        }
        return new List<NonConvexMonotoneCutSurfaceEdge>();
    }

    /// <summary>
    /// グラフ内のすべての頂点を取得する
    /// </summary>
    public IEnumerable<NonConvexMonotoneCutSurfaceVertex> GetAllVertices() {
        return _map.Keys;
    }
}
