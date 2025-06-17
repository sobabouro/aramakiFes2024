using System.Collections.Generic;

/// <summary>
/// 連結した要素を管理するシーケンスを、リストで管理する抽象基底クラス
/// </summary>
/// <typeparam name="TNodeSequence"> シーケンスの型 </typeparam>
/// <typeparam name="TItem"> シーケンスが管理するオブジェクトの型 </typeparam>
public abstract class AbstractNodeSequenceList<TNodeSequence, TItem>
    where TNodeSequence : AbstractNodeSequence<TItem>, new()
    where TItem : class {

    /// <summary>
    /// 連結した要素のシーケンスを保持するリスト
    /// </summary>
    protected readonly List<TNodeSequence> _nodeSequenceList = new();

    /// <summary>
    /// シーケンスのリストの要素数を取得するプロパティ
    /// </summary>
    public int Count => _nodeSequenceList.Count;

    /// <summary>
    /// リストの各シーケンスに対して、新しい連結要素の追加判定を行い、適切なシーケンスに追加を行うメソッド
    /// </summary>
    /// <param name="args"></param>
    public abstract void Add(params object[] args);

    /// <summary>
    /// リスト中の指定されたインデックスのシーケンスに対して、後ろにマージできるシーケンスを探してマージを試みるメソッド
    /// </summary>
    /// <param name="index"> リストのインデックス </param>
    /// <param name="afterKey"> 比較キー </param>
    protected void TryMergeAfter(int index, TItem afterKey) {
        for (int j = _nodeSequenceList.Count - 1; j > index; j--) {
            if (CheckMerge(_nodeSequenceList[j].First?.Value, afterKey, true)) {
                _nodeSequenceList[index].MergeAfter(_nodeSequenceList[j]);
                _nodeSequenceList.RemoveAt(j);
                return;
            }
        }
    }

    /// <summary>
    /// リスト中の指定されたインデックスのシーケンスに対して、前にマージできるシーケンスを探してマージを試みるメソッド
    /// </summary>
    /// <param name="index"> リストのインデックス </param>
    /// <param name="beforeKey"> 比較キー </param>
    protected void TryMergeBefore(int index, TItem beforeKey) {
        for (int j = _nodeSequenceList.Count - 1; j > index; j--) {
            if (CheckMerge(_nodeSequenceList[j].Last?.Value, beforeKey, false)) {
                _nodeSequenceList[index].MergeBefore(_nodeSequenceList[j]);
                _nodeSequenceList.RemoveAt(j);
                return;
            }
        }
    }

    /// <summary>
    /// シーケンスの値と比較キーを使って、マージ可能かどうかを判定する抽象メソッド
    /// </summary>
    /// <param name="target"> あるシーケンスの先頭または末尾の連結要素 </param>
    /// <param name="key"> 比較キー (Add() によって追加された新要素) </param>
    /// <param name="isAfter"> 前後のどちらに対してマージを試みるかを示すフラグ </param>
    /// <returns> true ならマージ可能、false ならマージ不可 </returns>
    protected abstract bool CheckMerge(TItem target, TItem key, bool isAfter);
}
