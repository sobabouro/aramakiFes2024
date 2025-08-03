using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 切断辺のソート戦略インターフェース
/// </summary>
public interface IEdgeSortStrategy {

    /// <summary>
    /// 切断辺のソートを行うメソッド
    /// </summary>
    /// <param name="edgesWithAngles"> ソート対象の入射角度差タグを所持した切断辺リスト </param>
    /// <returns> ソートされた切断辺リスト </returns>
    List<NonConvexMonotoneCutSurfaceEdge> Sort(
        IEnumerable<(NonConvexMonotoneCutSurfaceEdge edge, float angle)> edgesWithAngles
    );
}
