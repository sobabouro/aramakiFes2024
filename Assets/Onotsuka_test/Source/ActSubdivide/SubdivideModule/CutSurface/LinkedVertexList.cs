using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedVertexList {

    /// <summary>
    /// 連結頂点のリスト
    /// </summary>
    private readonly List<LinkedVertex> _linkedVertexList = new List<LinkedVertex>();

    /// <summary>
    /// 連結ポリゴンの数を取得するプロパティ
    /// </summary>
    public int Count {
        get {
            return _linkedVertexList.Count;
        }
    }
    /// <summary>
    /// リストに新しいポリゴンを追加するメソッド
    /// </summary>
    /// <param name="target"> 追加するポリゴン </param>
    public void Add(
        NewVertex toward,
        NewVertex away
    ) {
        for (int i = 0; i < _linkedVertexList.Count; i++) {
            var current = _linkedVertexList[i];

            if (current.TryAppend(toward, away)) {
                TryMergeAfter(i, away);
                return;
            }
            if (current.TryPrepend(toward, away)) {
                TryMergeBefore(i, toward);
                return;
            }
        }

        // どの連結ポリゴンにも追加できなかった場合は新しい連結ポリゴンを作成する
        var newLinked = new LinkedVertex();
        newLinked.TryAppend(toward, away);
        _linkedVertexList.Add(newLinked);
    }

    /// <summary>
    /// リスト中の指定されたインデックスの連結辺に対して、後ろにマージできる連結辺を探してマージを試みるメソッド
    /// Add() メソッドの前ループで、TryPrepend() で、前への連結判定は済んでいるので、処理対象の連結辺よりも後ろの要素のみマージ判定を行えばよい
    /// </summary>
    /// <param name="index"> 連結辺のインデックス </param>
    /// <param name="away"> 連結辺の終点側頂点 </param>
    /// 
    private void TryMergeAfter(int index, NewVertex away) {
        for (int j = _linkedVertexList.Count - 1; j > index; j--) {
            if (_linkedVertexList[j].First?.Value.Position == away.Position) {
                _linkedVertexList[index].MergeAfter(_linkedVertexList[j]);
                _linkedVertexList.RemoveAt(j);
            }
        }
    }

    /// <summary>
    /// リスト中の指定されたインデックスの連結辺に対して、前にマージできる連結辺を探してマージを試みるメソッド
    /// Add() メソッドの前ループで、TryAppend() で、後ろへの連結判定は済んでいるので、処理対象の連結辺よりも後ろの要素のみマージ判定を行えばよい
    /// </summary>
    /// <param name="index"> 連結辺のインデックス </param>
    /// <param name="toward"> 連結辺の始点側頂点 </param>
    private void TryMergeBefore(int index, NewVertex toward) {
        for (int j = _linkedVertexList.Count - 1; j > index; j--) {
            if (_linkedVertexList[j].Last?.Value.Position == toward.Position) {
                _linkedVertexList[index].MergeBefore(_linkedVertexList[j]);
                _linkedVertexList.RemoveAt(j);
            }
        }
    }
}
