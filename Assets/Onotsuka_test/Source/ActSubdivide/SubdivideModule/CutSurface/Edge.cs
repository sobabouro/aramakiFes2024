using System;

/// <summary>
/// 辺に関する情報を保持する構造体
/// </summary>
/// <typeparam name="T"> AbstractCutSurfaceVertex の継承型 </typeparam>
public struct Edge<T> 
    where T : AbstractCutSurfaceVertex {

    /// <summary>
    /// 辺の始点
    /// </summary>
    public readonly T Start;

    /// <summary>
    /// 辺の終点
    /// </summary>
    public readonly T End;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="start"> 辺の始点 </param>
    /// <param name="end"> 辺の終点 </param>
    public Edge(T start, T end) {
        Start = start;
        End = end;
    }

    /// <summary>
    /// オーバーライドされた Equals メソッド
    /// 辺の始点と終点が同じかどうかを比較する
    /// </summary>
    /// <param name="obj"> Edge<T> 型 </param>
    /// <returns> 等しければ true, そうでなければ false </returns>
    public override bool Equals(object? obj) => obj is Edge<T> other && Start.Equals(other.Start) && End.Equals(other.End);

    /// <summary>
    /// GetHashCode メソッドのオーバーライド
    /// start, end のハッシュコードを組み合わせて一意のハッシュコードを生成する
    /// </summary>
    /// <returns> ハッシュコード </returns>
    public override int GetHashCode() => HashCode.Combine(Start, End);
}
