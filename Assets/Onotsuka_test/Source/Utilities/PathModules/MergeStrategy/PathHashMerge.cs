using UnityEngine;

/// <summary>
/// パス同士をマージするメソッドの実装
/// HashSetを使用して、パスの始点と終点を比較し、連結可能な場合はパスを直接マージする
/// 実行速度・メモリパフォーマンスは高いが、循環や破壊的なマージが発生する可能性があるため、注意が必要
/// </summary>
public class PathHashMerge : IMergeStrategy {

    /// <summary>
    /// パス同士をマージするメソッド
    /// PathInspiredLinkedList 型のパスに対しての実装
    /// </summary>
    /// <param name="inputOnePath"> 一方のパス </param>
    /// <param name="inputOtherPath"> もう一方のパス </param>
    /// <exception cref="System.ArgumentException"> パスの型が異なる場合 </exception>
    public void Merge(IPathFactory inputOnePath, IPathFactory inputOtherPath) {
        if (inputOnePath is not PathInspiredLinkedList onePath || inputOtherPath is not PathInspiredLinkedList otherPath ) {
            throw new System.ArgumentException("Path <type> must be <PathInspiredLinkedList>.");
        }

        int oneStart = onePath.GetStartNode();
        int oneEnd = onePath.GetEndNode();
        int otherStart = otherPath.GetStartNode();
        int otherEnd = otherPath.GetEndNode();

        // パス A(one) の終点とパス B(other) の始点が連結可能な場合
        if (!onePath.IsContain(otherStart) && !otherPath.IsContain(oneEnd) && oneEnd != otherStart) {
            if (onePath.AddEdge(oneEnd, otherStart)) {
                Update(onePath, otherPath);
            } 
            else {
                Debug.LogWarning("Paths are not connected and cannot be merged.");
            }
        }

        // パス B(other) の終点とパス A(one) の始点が連結可能な場合
        else if (!onePath.IsContain(otherEnd) && !otherPath.IsContain(oneStart) && oneStart != otherEnd) {
            if (onePath.AddEdge(otherEnd, oneStart)) {
                Update(onePath, otherPath);
            } 
            else {
                Debug.LogWarning("Paths are not connected and cannot be merged.");
            }
        }
    }

    /// <summary>
    /// ノードを更新するメソッド
    /// </summary>
    /// <param name="parent"> 親となる方のパス </param>
    /// <param name="child"> 異なる方のパス </param>
    private void Update(PathInspiredLinkedList parent, PathInspiredLinkedList child) {
        var p = (IPathAcceser)parent;
        var c = (IPathAcceser)child;

        // _next 辞書を更新する
        foreach (var node in c.GetNextDictionary())
            p.GetNextDictionary()[node.Key] = node.Value;
        // _prev 辞書を更新する
        foreach (var node in p.GetPrevDictionary())
            c.GetPrevDictionary()[node.Key] = node.Value;
        // _nodes ハッシュセットを更新する
        foreach (var node in c.GetNodes())
            p.GetNodes().Add(node);
    }
}
