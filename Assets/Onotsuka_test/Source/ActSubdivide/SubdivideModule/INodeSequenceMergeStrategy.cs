using System.Collections.Generic;

/// <summary>
/// 連結された要素の管理クラスの、マージメソッドのストラテジーインターフェース
/// </summary>
/// <typeparam name="T"> NodeSequence が管理する要素の型 </typeparam>
public interface INodeSequenceMergeStrategy<T>
    where T : class {

    /// <summary>
    /// ノードの先頭を取得するプロパティ
    /// </summary>
    LinkedListNode<T>? First {
        get;
    }

    /// <summary>
    /// ノードの末尾を取得するプロパティ
    /// </summary>
    LinkedListNode<T>? Last {
        get;
    }

    /// <summary>
    /// 連結要素 (LinkedList) の後ろに、他の連結要素をマージする
    /// </summary>
    /// <param name="other"></param>
    void MergeAfter(AbstractNodeSequence<T> other);

    /// <summary>
    /// 連結要素 (LinkedList) の前に、他の連結要素をマージする
    /// </summary>
    /// <param name="other"></param>
    void MergeBefore(AbstractNodeSequence<T> other);
}
