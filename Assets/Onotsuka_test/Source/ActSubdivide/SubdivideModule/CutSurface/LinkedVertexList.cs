using UnityEngine;

/// <summary>
/// 連結辺シーケンス (LinkedVertex) のリストを管理するクラス
/// </summary>
public class LinkedVertexList : AbstractNodeSequenceList<LinkedVertex, NewVertex> {

    /// <summary>
    /// 連結辺シーケンスに頂点を追加するメソッド
    /// </summary>
    /// <param name="args"> (NewVertex 始点, NewVertex 終点) </param>
    public override void Add(params object[] args) {
        if (args.Length != 2 || !(args[0] is NewVertex toward) || !(args[1] is NewVertex away)) {
            Debug.LogError("Add for LinkedVertexList requires two NewVertex arguments.");
            return;
        }

        for (int i = 0; i < _nodeSequenceList.Count; i++) {
            var current = _nodeSequenceList[i];

            if (current.TryAppend(toward, away)) {
                TryMergeAfter(i, away);
                return;
            }
            if (current.TryPrepend(toward, away)) {
                TryMergeBefore(i, toward);
                return;
            }
        }

        // どの連結辺にも追加できなかった場合は新しい連結辺を作成する
        var newLinked = new LinkedVertex();
        newLinked.TryAppend(toward, away);
        _nodeSequenceList.Add(newLinked);
    }

    /// <summary>
    /// シーケンス同士のマージ可能性を判定するメソッド
    /// </summary>
    /// <param name="target"> あるシーケンスの頂点 </param>
    /// <param name="key"> Add() によって追加した新有向辺の対応する頂点 </param>
    /// <param name="isAfter"> 前後のどちらに対してマージを試みるかを示すフラグ </param>
    /// <returns></returns>
    protected override bool CheckMerge(NewVertex target, NewVertex key, bool isAfter) {
        if (target == null)
            return false;
        return target.Position == key.Position;
    }
}
