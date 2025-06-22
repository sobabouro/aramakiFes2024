using UnityEngine;

/// <summary>
/// 連結ポリゴンの情報を保持するクラス
/// 連結ポリゴンは切断辺の始点から終点の順を正方向として連結する
/// </summary>
public class LinkedVertex : AbstractNodeSequence<CutSurfaceVertex> {

    public LinkedVertex() : base(new DeleteDuplicateMergeStrategy<CutSurfaceVertex>()) { }
    public LinkedVertex(INodeSequenceMergeStrategy<CutSurfaceVertex> mergeStrategy) : base(mergeStrategy) { }

    /// <summary>
    /// 対象の辺を連結辺に対して後ろに追加するメソッド
    /// </summary>
    /// <param name="args"> (CutSurfaceVertex 始点, CutSurfaceVertex 終点) の引数 </param>
    /// <returns> 追加に成功した場合は true, 失敗した場合は false </returns>
    public override bool TryAppend(params object[] args) {
        if (args.Length != 2 || !(args[0] is CutSurfaceVertex toward) || !(args[1] is CutSurfaceVertex away)) {
            Debug.LogError("TryAppend for LinkedVertex requires two CutSurfaceVertex arguments.");
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
        if (args.Length != 2 || !(args[0] is CutSurfaceVertex toward) || !(args[1] is CutSurfaceVertex away)) {
            Debug.LogError("TryAppend for LinkedVertex requires two CutSurfaceVertex arguments.");
            return false;
        }
        var value = _nodeSequence.First?.Value;
        if (value != null && value.Equals(away)) {
            _nodeSequence.AddFirst(toward);

            return true;
        }
        return false;
    }

    public void Display() {
        foreach (var vertex in _nodeSequence) {
            Debug.Log($"LinkedVertex: local position - {vertex.LocalPosition}, plane position - {vertex.PlanePosition}");
        }
    }
}
