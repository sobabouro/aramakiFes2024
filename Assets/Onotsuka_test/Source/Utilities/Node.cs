/// <summary>
/// ノードクラス
/// </summary>
/// <typeparam name="T"> 値の型 </typeparam>
public class Node<T> {

    /// <summary>
    /// 値
    /// </summary>
    public T Value {
        get; private set;
    }

    /// <summary>
    /// ノード
    /// </summary>
    public Node<T> Next {
        get; set;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="value"> 値 </param>
    /// <param name="node"> ノード </param>
    public Node(T value, Node<T> node) {
        Value = value;
        Next = node;
    }
}
