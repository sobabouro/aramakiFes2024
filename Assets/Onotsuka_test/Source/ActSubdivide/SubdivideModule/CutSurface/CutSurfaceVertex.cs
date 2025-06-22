using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 切断平面上の頂点に関する情報を保持するクラス
/// </summary>
public class CutSurfaceVertex : IEquatable<CutSurfaceVertex> {

    /// <summary>
    /// 頂点のローカル座標 (unity)
    /// </summary>
    public readonly Vector3 LocalPosition;

    /// <summary>
    /// 切断平面上を座標系とした頂点の座標 2D 座標 (直交座標系)
    /// </summary>
    public readonly Vector2 PlanePosition;

    /// <summary>
    /// 頂点の種類
    /// </summary>
    public VertexType VertexType;

    public CutSurfaceVertex helper;

    /// <summary>
    /// 切断平面上の頂点情報を初期化するコンストラクタ
    /// </summary>
    /// <param name="localPlane"> 切断平面 (ローカル座標) </param>
    /// <param name="localPosition"> 頂点座標 </param>
    public CutSurfaceVertex(Plane localPlane, Vector3 localPosition) {
        LocalPosition = localPosition;

        Vector3 axisX = Vector3.Cross(localPlane.normal, Vector3.up).normalized;
        if (axisX.sqrMagnitude < 0.0001f) {
            axisX = Vector3.Cross(localPlane.normal, Vector3.right).normalized;
        }
        Vector3 axisY = Vector3.Cross(localPlane.normal, axisX).normalized;
        Vector3 anchor = localPlane.normal * localPlane.distance;

        PlanePosition = new Vector2(
            Vector3.Dot(localPosition - anchor, axisX),
            Vector3.Dot(localPosition - anchor, axisY)
        );
    }

    /// <summary>
    /// Equals メソッドのオーバーライド
    /// 内容比較
    /// </summary>
    /// <param name="other"> CutSurfaceVertex 型オブジェクト </param>
    /// <returns> 等しければ true, そうでなければ false </returns>
    public bool Equals(CutSurfaceVertex? other) {
        if (other == null)
            return false;
        return LocalPosition == other.LocalPosition;
    }

    /// <summary>
    /// Equals メソッドのオーバーライド
    /// 型比較
    /// </summary>
    /// <param name="obj"> オブジェクト型 </param>
    /// <returns> 等しければ true, そうでなければ false </returns>
    public override bool Equals(object? obj) {
        if (obj is CutSurfaceVertex other)
            return Equals(other);
        return false;
    }

    /// <summary>
    /// GetHashCode メソッドのオーバーライド
    /// Equala メソッドで利用されるハッシュコードを返す
    /// </summary>
    /// <returns> インスタンスの参照に基づいた数値 </returns>
    public override int GetHashCode() {
        return LocalPosition.GetHashCode();
    }
}
