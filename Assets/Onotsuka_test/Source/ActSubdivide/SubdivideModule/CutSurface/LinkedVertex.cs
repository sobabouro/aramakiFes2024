using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 連結ポリゴンの情報を保持するクラス
/// 連結ポリゴンは切断辺の始点から終点の順を正方向として連結する
/// </summary>
public class LinkedVertex {

    /// <summary>
    /// 連結頂点の情報を保持するための双方向リスト
    /// </summary>
    private readonly LinkedList<NewVertex> _linkedVertex = new LinkedList<NewVertex>();

    /// <summary>
    /// 連結ポリゴンの先頭を取得するプロパティ
    /// </summary>
    public LinkedListNode<NewVertex>? First => _linkedVertex.First;

    /// <summary>
    /// 連結ポリゴンの末尾を取得するプロパティ
    /// </summary>
    public LinkedListNode<NewVertex>? Last => _linkedVertex.Last;

    /// <summary>
    /// 対象の辺を連結辺に対して後ろに追加するメソッド
    /// </summary>
    /// <param name="toward"> 追加したい辺の始点 </param>
    /// <param name="away"> 追加したい辺の終点 </param>
    /// <returns> 追加に成功した場合は true, 失敗した場合は false </returns>
    public bool TryAppend(NewVertex toward, NewVertex away) {
        if (_linkedVertex.Last?.Value.Position == toward.Position || First == null) {
            _linkedVertex.AddLast(away);

            return true;
        }
        return false;
    }

    /// <summary>
    /// 対象の辺を連結辺に対して前に追加するメソッド
    /// </summary>
    /// <param name="toward"> 追加したい辺の始点 </param>
    /// <param name="away"> 追加したい辺の終点 </param>
    /// <returns> 追加に成功した場合は true, 失敗した場合は false </returns>
    public bool TryPrepend(NewVertex toward, NewVertex away) {
        if (_linkedVertex.First?.Value.Position == away.Position) {
            _linkedVertex.AddFirst(toward);

            return true;
        }
        return false;
    }

    /// <summary>
    /// この連結辺の後ろに他の連結辺をマージするメソッド
    /// </summary>
    /// <param name="other"></param>
    public void MergeAfter(LinkedVertex other) {
        foreach (var otherPolygon in other._linkedVertex)
            _linkedVertex.AddLast(otherPolygon);
    }

    /// <summary>
    /// この連結辺の前に他の連結辺をマージするメソッド
    /// </summary>
    /// <param name="other"></param>
    public void MergeBefore(LinkedVertex other) {
        var node = other._linkedVertex.Last;
        while (node != null) {
            _linkedVertex.AddFirst(node.Value);
            node = node.Previous;
        }
    }
}
