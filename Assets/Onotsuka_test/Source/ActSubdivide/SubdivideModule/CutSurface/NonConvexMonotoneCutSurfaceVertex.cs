using System;
using UnityEngine;

public class NonConvexMonotoneCutSurfaceVertex : AbstractCutSurfaceVertex, IEquatable<NonConvexMonotoneCutSurfaceVertex> {

    /// <summary>
    /// 頂点の種類
    /// </summary>
    public VertexType VertexType;

    /// <summary>
    /// ヘルパー頂点
    /// this 頂点と，ヘルパー頂点とをリンクするために保持する
    /// </summary>
    public NonConvexMonotoneCutSurfaceVertex Helper;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="localPlane"> 切断平面 </param>
    /// <param name="localPosition"> 頂点の空間座標 </param>
    public NonConvexMonotoneCutSurfaceVertex(Plane localPlane, Vector3 localPosition)
        : base(localPlane, localPosition) {
    }

    /// <summary>
    /// 2D 平面上の辺と点との距離を計算するメソッド
    /// </summary>
    /// <param name="point"> 対象の頂点 </param>
    /// <param name="edgeStart"> 辺の始点 </param>
    /// <param name="edgeEnd"> 辺の終点 </param>
    /// <returns> 対象の頂点から辺までの距離 </returns>
    public static float DistancePointToEdge(
        NonConvexMonotoneCutSurfaceVertex point, 
        NonConvexMonotoneCutSurfaceVertex edgeStart, 
        NonConvexMonotoneCutSurfaceVertex edgeEnd
    ) {
        Vector2 edgeStartPosition = edgeStart.PlanePosition;
        Vector2 edgeEndPosition = edgeEnd.PlanePosition;
        Vector2 pointPosition = point.PlanePosition;
        float edgeLengthSquared = (edgeEndPosition - edgeStartPosition).sqrMagnitude;

        if (edgeLengthSquared < 0.0001f) {
            return (pointPosition - edgeStartPosition).magnitude;
        }
        float LinearFactor = Vector2.Dot(pointPosition - edgeStartPosition, edgeEndPosition - edgeStartPosition) / edgeLengthSquared;
        LinearFactor = Mathf.Clamp01(LinearFactor);

        Vector2 closestPoint = edgeStartPosition + LinearFactor * (edgeEndPosition - edgeStartPosition);

        return (pointPosition - closestPoint).magnitude;
    }

    /// <summary>
    /// 指定された点が、指定された辺の左側にあるかどうかを判定するメソッド
    /// </summary>
    /// <param name="point"> 指定された頂点 </param>
    /// <param name="edgeStart"> ある辺の始点 </param>
    /// <param name="edgeEnd"> ある辺の終点 </param>
    /// <returns> 指定された辺が右側にあれば true, そうでなければ false </returns>
    public static bool IsEdgeLeftOfPoint(
        NonConvexMonotoneCutSurfaceVertex point, 
        NonConvexMonotoneCutSurfaceVertex edgeStart,
        NonConvexMonotoneCutSurfaceVertex edgeEnd
    ) {
        float epsilon = 0.0001f;

        // 辺が水平の場合
        if (Math.Abs(edgeStart.PlanePosition.y - edgeEnd.PlanePosition.y) < epsilon) {
            return Math.Max(edgeStart.PlanePosition.x, edgeEnd.PlanePosition.x) < point.PlanePosition.x - epsilon;
        }
        // 辺が垂直や傾いている場合
        // point.y が edgeStart.y と edgeEnd.y の間のどこに位置するかを比率として算出する
        float ratio = (point.PlanePosition.y - edgeStart.PlanePosition.y) / (edgeEnd.PlanePosition.y - edgeStart.PlanePosition.y);
        // 比率を使って，point.y の位置に対応する edge 上の x 座標を算出する
        float lineSegmentIntersectionX = edgeStart.PlanePosition.x + ratio * (edgeEnd.PlanePosition.x - edgeStart.PlanePosition.x);
        return lineSegmentIntersectionX < point.PlanePosition.x - epsilon;
    }

    /// <summary>
    /// Equals メソッドのオーバーライド
    /// </summary>
    /// <param name="other"> AbstractCutSurfaceVertex 型オブジェクト </param>
    /// <returns> 等しければ true, そうでなければ false </returns>
    public override bool Equals(AbstractCutSurfaceVertex other) {
        if (other is NonConvexMonotoneCutSurfaceVertex otherVertex) {
            return LocalPosition.Equals(otherVertex.LocalPosition);
        }
        return false;
    }

    /// <summary>
    /// Equals メソッドのオーバーライド
    /// </summary>
    /// <param name="other"> NonConvexMonotoneCutSurfaceVertex 型オブジェクト </param>
    /// <returns> 等しければ true, そうでなければ false </returns>
    public bool Equals(NonConvexMonotoneCutSurfaceVertex other) {
        if (other == null)
            return false;
        return LocalPosition.Equals(other.LocalPosition);
    }

    /// <summary>
    /// GetHashCode メソッドのオーバーライド
    /// Equala メソッドで利用されるハッシュコードを返す
    /// </summary>
    /// <returns> インスタンスの参照に基づいた数値 </returns>
    public override int GetHashCode() {
        return base.GetHashCode();
    }
}
