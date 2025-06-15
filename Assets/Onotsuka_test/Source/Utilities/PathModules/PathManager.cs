using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 有向パスグループを管理するクラス
/// </summary>
public class PathManager {

    /// <summary>
    /// 格納するパスの型
    /// </summary>
    private IPathFactory _pathFactory;

    /// <summary>
    /// パスの結合方法
    /// </summary>
    private IMergeStrategy _mergeStrategy;

    /// <summary>
    /// パスのリスト
    /// </summary>
    public List<IPathFactory> Path { 
        get; private set; 
    } = new List<IPathFactory>();

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="pathFactory"> 格納するパスの型 </param>
    /// <param name="mergeStrategy"> パスの結合方法 </param>
    public PathManager(IPathFactory pathFactory, IMergeStrategy mergeStrategy) {
        _pathFactory = pathFactory;
        _mergeStrategy = mergeStrategy;
    }

    /// <summary>
    /// パスにエッジを追加するメソッド
    /// </summary>
    /// <param name="from"> 始点 </param>
    /// <param name="to"> 終点 </param>
    public void AddEdge(int from, int to) {
        IPathFactory fromPath = null;
        IPathFactory toPath = null;

        foreach (var path in Path) {
            if (path.IsContain(from)) fromPath = path;
            if (path.IsContain(to)) toPath = path;
        }

        // どちらのパスにも含まれない場合、新しいパスを作成する
        if (fromPath == null && toPath == null) {
            var newPath = _pathFactory.CreateInstance();
            newPath.AddEdge(from, to);
            Path.Add(newPath);
        }
        // fromPath のみに含まれる場合、fromPath に追加 
        else if (fromPath != null && toPath == null) {
            fromPath.AddEdge(from, to);
        }
        // toPath のみに含まれる場合、toPath に追加 
        else if (fromPath == null && toPath != null) {
            toPath.AddEdge(from, to);
        }
        // 両方のパスに含まれる場合、パスをマージする
        else if (fromPath != null && toPath != null && fromPath != toPath) {
            try {
                _mergeStrategy.Merge(fromPath, toPath);
                fromPath.AddEdge(from, to);
                Path.Remove(toPath);
            } catch (System.Exception ex) {
                Debug.LogWarning($"[PathManager] Merge failed: {ex.Message}");
            }
        }
        // 同一パスに含まれている場合
        else {
            fromPath.AddEdge(from, to);
        }
    }
}
