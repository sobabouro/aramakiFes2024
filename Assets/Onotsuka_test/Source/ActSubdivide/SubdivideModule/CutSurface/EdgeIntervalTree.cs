using System.Collections.Generic;
using System.Linq;
using Jamarino.IntervalTree;
using UnityEngine;

/// <summary>
/// interval tree を使用して，辺用に機能改良したクラス
/// interval (区間) ごとの走査を高速に行える自己平衡二分探索木のデータ構造
/// </summary>
public class EdgeIntervalTree {

    private const float Epsilon = 0.0001f;

    /// <summary>
    /// y 座標を基準にした区間木
    /// </summary>
    private readonly IIntervalTree<float, NonConvexMonotoneCutSurfaceEdge> _tree = new LightIntervalTree<float, NonConvexMonotoneCutSurfaceEdge>();

    /// <summary>
    /// EdgeIntervalTree に辺を追加するメソッド
    /// </summary>
    /// <param name="edge"> 追加する辺 </param>
    public void AddEdge(NonConvexMonotoneCutSurfaceEdge edge) {
        _tree.Add(edge.MinY, edge.MaxY, edge);
    }

    /// <summary>
    /// EdgeIntervalTree から辺を削除するメソッド
    /// </summary>
    /// <param name="edge"> 削除する辺 </param>
    public void RemoveEdge(NonConvexMonotoneCutSurfaceEdge edge) {
        _tree.Remove(edge);
    }

    /// <summary>
    /// 入力された y 座標の水平線が通過する辺のリストを取得するメソッド
    /// </summary>
    /// <param name="y"> 水平線の y 座標 </param>
    /// <returns> y 座標の水平線が通過する辺のリスト </returns>
    public List<NonConvexMonotoneCutSurfaceEdge> GetEdgesPassThroughHorizon(float y) {
        var edges = _tree.Query(y);

        return edges
            .Where(edge => (y > edge.MinY + Epsilon && y < edge.MaxY - Epsilon))
            .ToList();
    }
}
