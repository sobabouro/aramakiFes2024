using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Union-Find アルゴリズムデータクラス
/// </summary>
public class UnionFind {

    /// <summary>
    /// 各ノードツリーの根ノード (自分が親なら自分自身)
    /// </summary>
    private Dictionary<int, int> _parent = new();

    /// <summary>
    /// 各ノードツリーのランク
    /// </summary>
    private Dictionary<int, int> _rank = new();


    /// <summary>
    /// ノード未登録なら登録する
    /// </summary>
    /// <param name="x">登録するノード</param>
    private void InitialRegister(int x) {
        if (!_parent.ContainsKey(x)) {
            _parent[x] = x;
            _rank[x] = 0;
        }
    }

    /// <summary>
    /// 指定したノードの根ノードを取得する
    /// </summary>
    /// <param name="x">指定ノード</param>
    /// <returns>親ノード</returns>
    public int Find(int x) {
        InitialRegister(x);
        if (_parent[x] != x) {
            _parent[x] = Find(_parent[x]);
        }
        return _parent[x];
    }

    /// <summary>
    /// 指定したノード同士を結合する
    /// </summary>
    /// <param name="x">結合するノード</param>
    /// <param name="y">結合するノード</param>
    public void Union(int x, int y) {
        InitialRegister(x);
        InitialRegister(y);
        int rootX = Find(x);
        int rootY = Find(y);

        if (rootX != rootY) {
            if (_rank[rootX] < _rank[rootY]) {
                _parent[rootX] = rootY;
            }
            else {
                _parent[rootY] = rootX;
                if (_rank[rootX] == _rank[rootY]) {
                    _rank[rootX]++;
                }
            }
        }
    }

    /// <summary>
    /// 指定したノード同士が同じ根のツリーに属していれば true を返す
    /// </summary>
    /// <param name="x"> 指定ノード</param>
    /// <param name="y"> 指定ノード</param>
    /// <returns>同じグループに属していれば true</returns>
    public bool IsConnected(int x, int y) {
        InitialRegister(x);
        InitialRegister(y);
        return Find(x) == Find(y);
    }

    /// <summary>
    /// すべてのノードツリーを (key: 根, value: ツリー) とした辞書にして返す
    /// </summary>
    /// <returns>すべてのノードツリーの辞書</returns>
    public Dictionary<int, List<int>> GetGroups() {
        Dictionary<int, List<int>> groups = new();
        foreach (var key in _parent.Keys) {
            int root = Find(key);
            if (!groups.ContainsKey(root)) {
                groups[root] = new List<int>();
            }
            groups[root].Add(key);
        }
        return groups;
    }

    /// <summary>
    /// デバッグ用
    /// </summary>
    public void PrintGroups() {
        Dictionary<int, List<int>> groups = GetGroups();
        foreach (var group in groups) {
            Debug.Log($"Group {group.Key}: {string.Join(", ", group.Value)}");
        }
    }
}
