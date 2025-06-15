using System.Collections.Generic;

/// <summary>
/// パスへの直接的なアクセスを提供するインターフェース
/// </summary>
internal interface IPathAcceser {

    /// <summary>
    /// from -> to の有向辺辞書を取得するメソッド
    /// </summary>
    /// <returns> from -> to の有向辺辞書 </returns>
    internal Dictionary<int, int> GetNextDictionary();

    /// <summary>
    /// to -> from の有向辺辞書を取得するメソッド
    /// </summary>
    /// <returns> to -> from の有向辺辞書 </returns>
    internal Dictionary<int, int> GetPrevDictionary();

    /// <summary>
    /// ノードの集合を取得するメソッド
    /// </summary>
    internal HashSet<int> GetNodes();
}
