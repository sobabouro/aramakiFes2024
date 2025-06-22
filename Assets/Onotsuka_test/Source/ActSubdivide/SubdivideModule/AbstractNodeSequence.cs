using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 連結した要素の管理を行う抽象基底クラス
/// </summary>
/// <typeparam name="T"> ノードとして管理するオブジェクト </typeparam>
public abstract class AbstractNodeSequence<T>
    where T : class {

    private INodeSequenceMergeStrategy<T> _mergeStrategy;

    /// <summary>
    /// 連結リストを保持する双方向リスト
    /// 派生クラスからはアクセス可
    /// </summary>
    protected readonly LinkedList<T> _nodeSequence = new();

    /// <summary>
    /// 連結シーケンスの先頭の要素を取得するプロパティ
    /// </summary>
    public LinkedListNode<T>? First => _nodeSequence.First;

    /// <summary>
    /// 連結シーケンスの末尾の要素を取得するプロパティ
    /// </summary>
    public LinkedListNode<T>? Last => _nodeSequence.Last;

    /// <summary>
    /// 連結要素のシーケンスを取得するプロパティ
    /// </summary>
    protected IEnumerable<T> GetItemsEnumerable() => _nodeSequence;

    public INodeSequenceMergeStrategy<T> MergeStrategy {
        get => _mergeStrategy;
        set => _mergeStrategy = value
            ?? throw new ArgumentNullException(nameof(value), "Merge strategy cannot be null.");
    }

    protected AbstractNodeSequence(INodeSequenceMergeStrategy<T> mergeStrategy) {
        _mergeStrategy = mergeStrategy;
    }

    /// <summary>
    /// 連結要素の後ろに要素を追加できるか試みる (抽象メソッド)
    /// </summary>
    public abstract bool TryAppend(params object[] args);

    /// <summary>
    /// 連結要素の前に要素を追加できるか試みる (抽象メソッド)
    /// </summary>
    public abstract bool TryPrepend(params object[] args);

    /// <summary>
    /// この連結要素の後ろに他の連結要素をマージする
    /// </summary>
    /// <param name="other">マージする他の要素</param>
    public void MergeAfter(AbstractNodeSequence<T> other) {
        if (_mergeStrategy == null) {
            throw new InvalidOperationException("Merge strategy is not set.");
        }
        Debug.Log($"AbstractNodeSequence: call MergeAfter()");
        _mergeStrategy.MergeAfterStrategy(_nodeSequence, other.GetItemsEnumerable(), other.First, other.Last);
    }

    /// <summary>
    /// この連結要素の前に他の連結要素をマージする
    /// </summary>
    /// <param name="other">マージする他の要素</param>
    public void MergeBefore(AbstractNodeSequence<T> other) {
        if (_mergeStrategy == null) {
            throw new InvalidOperationException("Merge strategy is not set.");
        }
        Debug.Log($"AbstractNodeSequence: call MergeBefore()");
        _mergeStrategy.MergeBeforeStrategy(_nodeSequence, other.GetItemsEnumerable(), other.First, other.Last);
    }

    /// <summary>
    /// 連結要素のシーケンスから指定されたノードを削除するメソッド
    /// </summary>
    /// <param name="nodeToDelete"> 削除するノード </param>
    public void RemoveNode(LinkedListNode<T> nodeToDelete) {
        _nodeSequence.Remove(nodeToDelete);
    }
}
