/// <summary>
/// 有向辺パスどうしの管理を行う機能のインターフェース
/// </summary>
public interface IMergeStrategy {

    /// <summary>
    /// 対応する有向辺が存在するパスグループへ有向辺を追加するメソッド
    /// 親子関係も考慮して、パスをマージする
    /// </summary>
    /// <param name="onePath"> 一方のパス </param>
    /// <param name="otherPath"> もう一方のパス </param>
    void Merge(IPathFactory onePath, IPathFactory otherPath);
}
