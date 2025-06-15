using System.Collections.Generic;
using System.Data;

/// <summary>
/// 有向パスの管理に双方向リストを意識したクラス
/// </summary>
public class PathInspiredLinkedList : IPathFactory, IPathAcceser {

    /// <summary>
    /// start -> end の有向辺を追加する
    /// </summary>
    private Dictionary<int, int> _next = new Dictionary<int, int>();

    /// <summary>
    /// end -> start の有向辺を追加する
    /// </summary>
    private Dictionary<int, int> _prev = new Dictionary<int, int>();

    /// <summary>
    /// ハッシュセットで、パスに含まれるノードを管理する
    /// </summary>
    private HashSet<int> _nodes = new HashSet<int>();

    /// <summary>
    /// インスタンス生成メソッド
    /// </summary>
    /// <returns> PathInspiredLinkedList 型インスタンス </returns>
    public IPathFactory CreateInstance() {
        return new PathInspiredLinkedList();
    }

    /// <summary>
    /// 有向辺を追加するメソッド
    /// </summary>
    /// <param name="from"> 始点ノードのID </param>
    /// <param name="to"> 終点ノードのID </param>
    /// <returns> 追加に成功した場合は true、既に連結済みの場合は false </returns>
    public bool AddEdge(int from, int to) {
        // 既に連結済みの場合
        if (_next.ContainsKey(from) || _prev.ContainsKey(to))
            return false;
        _next[from] = to;
        _prev[to] = from;
        _nodes.Add(from);
        _nodes.Add(to);
        return true;
    }

    /// <summary>
    /// 次のノードを取得するメソッド
    /// </summary>
    /// <param name="node"> 現在のノード </param>
    /// <param name="next"> 次のノード </param>
    /// <returns> ノードがあれば true, なければ false </returns>
    public bool TryGetNext(int node, out int next) => _next.TryGetValue(node, out next);

    /// <summary>
    /// 前のノードを取得するメソッド
    /// </summary>
    /// <param name="node"> 現在のノード </param>
    /// <param name="prev"> 前のノード </param>
    /// <returns> ノードがあれば true, なければ false </returns>
    public bool TryGetPrev(int node, out int prev) => _prev.TryGetValue(node, out prev);

    /// <summary>
    /// 有向辺の始点を取得するメソッド
    /// </summary>
    /// <returns> 始点ノードのID </returns>
    /// <exception cref="DataException"> 始点ノードが見つからない場合 </exception>"
    public int GetStartNode() {
        foreach (var node in _nodes) {
            if (!_prev.ContainsKey(node)) {
                return node;
            }
        }
        throw new DataException("Nothing start node in the path.");
    }

    /// <summary>
    /// 有向辺の終点を取得するメソッド
    /// </summary>
    /// <returns> 終点ノードのID </returns>
    /// <exception cref="DataException"> 終点ノードが見つからない場合 </exception>"
    public int GetEndNode() {
        foreach (var node in _nodes) {
            if (!_next.ContainsKey(node)) return node;
        }
        throw new DataException("Nothing end node in path.");
    }

    /// <summary>
    /// 連結した有向辺のパスを取得するメソッド
    /// </summary>
    /// <returns> 連結した有向辺のパスを表すノードIDのリスト </returns>
    public List<int> GetPath() {
        List<int> path = new List<int>();
        int currentNode = GetStartNode();
        while (_next.ContainsKey(currentNode)) {
            path.Add(currentNode);
            currentNode = _next[currentNode];
        }
        path.Add(currentNode);
        return path;
    }

    /// <summary>
    /// ノード (頂点) がパスに含まれているかを確認するメソッド
    /// </summary>
    /// <param name="node"> ノード (頂点) のID </param>
    /// <returns> パスに含まれている場合は true、含まれていない場合は false </returns>
    public bool IsContain(int node) => _nodes.Contains(node);

    /// <summary>
    /// 限定アクセサ
    /// </summary>
    /// <returns> from -> to の辞書 </returns>
    Dictionary<int, int> IPathAcceser.GetNextDictionary() => _next;

    /// <summary>
    /// 限定アクセサ
    /// </summary>
    /// <returns> to -> from の辞書 </returns>
    Dictionary<int, int> IPathAcceser.GetPrevDictionary() => _prev;

    /// <summary>
    /// 限定アクセサ
    /// </summary>
    /// <returns> パスに含まれるノードのハッシュセット </returns>
    HashSet<int> IPathAcceser.GetNodes() => _nodes;
}
