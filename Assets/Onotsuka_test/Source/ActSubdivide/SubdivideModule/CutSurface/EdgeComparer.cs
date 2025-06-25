using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ある y の水平線に対して，辺の x 座標を比較するためのクラス
/// </summary>
/// <typeparam name="T"> AbstractCutSurfaceVertex の継承型 </typeparam>
public class EdgeComparer<T> : IComparer<Edge<T>>
    where T : AbstractCutSurfaceVertex {

    /// <summary>
    /// 比較の際に使用する、浮動小数点数の誤差を表す値
    /// </summary>
    private static float s_epsilon = 0.0001f;

    /// <summary>
    /// 水平線の y 座標
    /// </summary>
    private readonly float _horizonY;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="thresholdY"> 水平線の y 座標 </param>
    public EdgeComparer(float thresholdY) {
        _horizonY = thresholdY;
    }

    /// <summary>
    /// 辺 1,2 の x 座標を比較するメソッド
    /// </summary>
    /// <param name="edge1"> 辺 1 </param>
    /// <param name="edge2"> 辺 2 </param>
    /// <returns> 辺 1 < 辺 2 の場合は負の値，辺 1 == 辺 2 の場合は 0，辺 1 > 辺 2 の場合は正の値を返す </returns>
    public int Compare(Edge<T> edge1, Edge<T> edge2) {
        float x1 = GetXPositionAtYPosition(edge1, _horizonY);
        float x2 = GetXPositionAtYPosition(edge2, _horizonY);

        if (Mathf.Abs(x1 - x2) < s_epsilon) {
            return HashCode.Combine(edge1).CompareTo(HashCode.Combine(edge2));
        }
        return x1.CompareTo(x2);
    }

    /// <summary>
    /// 指定された辺の y 座標に対する x 座標を取得するメソッド
    /// 水平線が辺の y 範囲内に存在することが前提である
    /// </summary>
    /// <param name="edge1"> 辺 </param>
    /// <param name="y"> 水平線の y 座標 </param>
    /// <returns> 辺上の水平線との交点の x 座標 </returns>
    private float GetXPositionAtYPosition(Edge<T> edge1, float y) {

        if (Mathf.Abs(edge1.Start.PlanePosition.y - edge1.End.PlanePosition.y) < s_epsilon) {
            return Mathf.Min(edge1.Start.PlanePosition.x, edge1.End.PlanePosition.x);
        }
        return edge1.Start.PlanePosition.x +
               (y - edge1.Start.PlanePosition.y) *
               (edge1.End.PlanePosition.x - edge1.Start.PlanePosition.x) /
               (edge1.End.PlanePosition.y - edge1.Start.PlanePosition.y);
    }
}
