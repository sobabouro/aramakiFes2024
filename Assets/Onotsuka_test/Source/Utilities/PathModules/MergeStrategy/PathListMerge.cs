using System;
using System.Collections.Generic;

/// <summary>
/// パス同士をマージするメソッドの実装
/// リストをコピーして代入していく
/// 安全性・保守性は高いが、連続するパスが多い場合はパフォーマンスが低下する可能性がある
/// </summary>
public class PathListMerge : IMergeStrategy {

    /// <summary>
    /// パス同士をマージするメソッド
    /// </summary>
    /// <param name="onePath"> 一方のパス </param>
    /// <param name="otherPath"> もう一方のパス </param>
    public void Merge(IPathFactory onePath, IPathFactory otherPath) {
        if (onePath is PathInspiredLinkedList == false || otherPath is PathInspiredLinkedList == false) {
            throw new ArgumentException("Path <type> must be <PathInspiredLinkedList>.");
        }
        List<int> path = otherPath.GetPath();
        for (int i = 0; i < path.Count - 1; i++) {
            onePath.AddEdge(path[i], path[i + 1]);
        }
    }
}
