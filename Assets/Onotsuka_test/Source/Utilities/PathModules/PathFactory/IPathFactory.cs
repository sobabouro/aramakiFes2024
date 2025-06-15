using System.Collections.Generic;

/// <summary>
/// 有向パス管理機能のインターフェース
/// </summary>
public interface IPathFactory {

    /// <summary>
    /// 新しいパスインスタンスを生成するメソッド
    /// </summary>
    /// <returns> IPathFactory 継承先のインスタンス </returns>
    public IPathFactory CreateInstance();

    /// <summary>
    /// 有向辺を登録するメソッド
    /// </summary>
    /// <param name="from"> 始点ノード </param>
    /// <param name="to"> 終点ノード </param>
    /// <returns> 追加に成功した場合は true、既に連結済みの場合は false </returns>
    public bool AddEdge(int from, int to);

    /// <summary>
    /// 次のノードを取得するメソッド
    /// </summary>
    /// <param name="node"> 現在のノード </param>
    /// <param name="next"> 次のノード </param>
    /// <returns> ノードがあれば true, なければ false </returns>
    public bool TryGetNext(int node, out int next);

    /// <summary>
    /// 前のノードを取得するメソッド
    /// </summary>
    /// <param name="node"> 現在のノード </param>
    /// <param name="prev"> 前のノード </param>
    /// <returns> ノードがあれば true, なければ false </returns>
    public bool TryGetPrev(int node, out int prev);

    /// <summary>
    /// 指定したノードがパスに含まれているかを確認するメソッド
    /// </summary>
    /// <returns> パスに含まれている場合は true、含まれていない場合は false </returns>
    public bool IsContain(int node);

    /// <summary>
    /// 有向辺ノードリストの始点を取得するメソッド
    /// </summary>
    /// <returns> 始点ノード </returns>
    public int GetStartNode();

    /// <summary>
    /// 有向辺ノードリストの終点を取得するメソッド
    /// </summary>
    /// <returns> 終点ノード </returns>
    public int GetEndNode();

    /// <summary>
    /// ノードのリストを取得するメソッド
    /// </summary>
    /// <returns> ノードのリスト </returns>
    public List<int> GetPath();
}
