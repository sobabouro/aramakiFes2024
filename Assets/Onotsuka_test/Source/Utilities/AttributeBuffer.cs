using System;
using System.Collections.Generic;

/// <summary>
/// 要素を管理するクラス
/// </summary>
/// <typeparam name="T"></typeparam>
public class AttributeBuffer<T> {

    /// <summary>
    /// リスト型
    /// </summary>
    private List<T>? _list;

    /// <summary>
    /// 配列型
    /// </summary>
    private T[]? _array;

    /// <summary>
    /// 最終値が確定したかどうか
    /// </summary>
    private bool _isFinalized = false;

    /// <summary>
    /// 動的な要素を管理するためのコンストラクタ
    /// </summary>
    public AttributeBuffer() {
        _list = new List<T>();
    }

    /// <summary>
    /// 静的な要素を管理するためのコンストラクタ
    /// </summary>
    /// <param name="array"> 初期配列 </param>
    public AttributeBuffer(T[] array) {
        _array = array;
        _isFinalized = true;
    }

    /// <summary>
    /// 要素の追加を行うメソッド (単一)
    /// </summary>
    /// <param name="item"> 追加する要素 </param>
    public void Add(T item) {
        if (_isFinalized)
            throw new InvalidOperationException("AttributeBuffer is finalized.");
        _list!.Add(item);
    }

    /// <summary>
    /// 要素の追加を行うメソッド (複数)
    /// </summary>
    /// <param name="items"> 追加する要素のコレクション </param>
    public void AddRange(IEnumerable<T> items) {
        if (_isFinalized)
            throw new InvalidOperationException("AttributeBuffer is finalized.");
        _list!.AddRange(items);
    }

    /// <summary>
    /// 配列に変換するメソッド
    /// </summary>
    public T[] ToArray() {
        if (_isFinalized) {
            return _array!;
        } else {
            return _list!.ToArray();
        }
    }

    /// <summary>
    /// 要素リストを最終決定するメソッド (以降変更不可)
    /// </summary>
    public void FinalizeElement() {
        if (_isFinalized)
            return;
        _array = _list!.ToArray();
        _list = null;
        _isFinalized = true;
    }

    /// <summary>
    /// 要素数を取得するプロパティ
    /// </summary>
    public int Count => _isFinalized ? _array!.Length : _list!.Count;

    /// <summary>
    /// アクセサ
    /// </summary>
    public T this[int index] {
        get {
            if (_isFinalized) {
                return _array![index];
            } else {
                return _list![index];
            }
        }
        set {
            if (_isFinalized) {
                throw new InvalidOperationException("AttributeBuffer is finalized.");
            }
            _list![index] = value;
        }
    }
}
