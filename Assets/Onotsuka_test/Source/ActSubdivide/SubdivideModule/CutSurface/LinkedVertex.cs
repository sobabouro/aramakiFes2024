using UnityEngine;

/// <summary>
/// 連結ポリゴンの情報を保持するクラス
/// 連結ポリゴンは切断辺の始点から終点の順を正方向として連結する
/// </summary>
public class LinkedVertex :AbstractNodeSequence<NewVertex> {

    /// <summary>
    /// 対象の辺を連結辺に対して後ろに追加するメソッド
    /// </summary>
    /// <param name="args"> (NewVertex 始点, NewVertex 終点) の引数 </param>
    /// <returns> 追加に成功した場合は true, 失敗した場合は false </returns>
    public override bool TryAppend(params object[] args) {
        if (args.Length != 2 || !(args[0] is NewVertex toward) || !(args[1] is NewVertex away)) {
            Debug.LogError("TryAppend for LinkedVertex requires two NewVertex arguments.");
            return false;
        }
        if (_nodeSequence.Last?.Value.Position == toward.Position) {
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
        if (args.Length != 2 || !(args[0] is NewVertex toward) || !(args[1] is NewVertex away)) {
            Debug.LogError("TryAppend for LinkedVertex requires two NewVertex arguments.");
            return false;
        }
        if (_nodeSequence.First?.Value.Position == away.Position) {
            _nodeSequence.AddFirst(toward);

            return true;
        }
        return false;
    }
}
