using System;
using System.Collections.Generic;

public class DiagonalEdgeGenerator {

    /// <summary>
    /// 図形すべての頂点を y 座標でソートした順番のインデックスリスト
    /// 図形ごと (LinkedVertexList ごと) の連結辺の番地に対応する
    /// もとの頂点リストでの頂点を "v" とする
    /// </summary>
    private (int, int)[] _indexBeforeSortY = new (int, int)[] { };

    /// <summary>
    /// 図形の辺のリスト (以下 "e" とする)
    /// v[i] を始点とする辺を e[i] とする (頂点は以下 "v" とする)
    /// </summary>
    private List<List<NonConvexMonotoneCutSurfaceEdge>> _edgeList = new();

    /// <summary>
    /// 辺の走査を行うための木 (以下 "T" とする)
    /// </summary>
    private EdgeIntervalTree _edgeIntervalTree = new();

    /// <summary>
    /// T の中の辺を x 座標でソートした順序を保持するための辞書
    /// </summary>
    private SortedDictionary<NonConvexMonotoneCutSurfaceEdge, NonConvexMonotoneCutSurfaceEdge> _sortedXPositionEdgeInTree = new(new EdgeComparer());

    /// <summary>
    /// 対角線の集合 (以下 "D" とする)
    /// 一つの対角線を追加する際，両方向に分けて二つ追加していく
    /// </summary>
    public HashSet<(NonConvexMonotoneCutSurfaceVertex, NonConvexMonotoneCutSurfaceVertex)> DiagonalSet { 
        get; 
        private set; 
    } = new();

    /// <summary>
    /// コンストラクタ
    /// ここで対角線の生成を行う
    /// </summary>
    /// <param name="linkedVertexList"> 連結辺シーケンスリスト </param>
    public DiagonalEdgeGenerator(LinkedVertexList linkedVertexList) {

        FormattingData(linkedVertexList);
        ProcessSweepLineForMakeDiagonalEdge(linkedVertexList);
    }


    private void FormattingData(LinkedVertexList linkedVertexList) {

        linkedVertexList.Formatting();
        linkedVertexList.ClusteringVertexType();

        for (int i = 0; i < linkedVertexList.Count; i++) {

            List<NonConvexMonotoneCutSurfaceEdge> edges = new();
            var list = linkedVertexList[i];
            var currentNode = list.First;

            for (int j = 0; j < list.Count; j++) {

                var startVertex = currentNode.Value;
                var endVertex = currentNode.Next != null ? currentNode.Next.Value : list.First.Value;

                if (startVertex != null && endVertex != null && !startVertex.Equals(endVertex)) {
                    NonConvexMonotoneCutSurfaceEdge edge = new NonConvexMonotoneCutSurfaceEdge(startVertex, endVertex);
                    edges.Add(edge);
                    _edgeIntervalTree.AddEdge(edge);
                }
                currentNode = currentNode.Next;
            }
            _edgeList.Add(edges);
        }
        _indexBeforeSortY = linkedVertexList.GetAllIndexSortedPlanePositionY();
    }

    /// <summary>
    /// 指定された頂点の最も左側にある辺の始点を取得するメソッド
    /// comparer の HorizonY が既定されていることが前提
    /// </summary>
    /// <param name="vertex"> 頂点 </param>
    /// <returns> 頂点の最も左側にある辺 </returns>
    /// <exception cref="InvalidOperationException"> 隣接頂点がない場合 </exception>
    private NonConvexMonotoneCutSurfaceEdge GetEdgeMostLeftNeighboringFromVertex(NonConvexMonotoneCutSurfaceVertex vertex) {

        NonConvexMonotoneCutSurfaceEdge tmpSearchKey = new(vertex, vertex);
        NonConvexMonotoneCutSurfaceEdge? mostLeftNeighboringEdge = null;

        foreach (var edge in _sortedXPositionEdgeInTree.Keys) {
            int comparisonResult = _sortedXPositionEdgeInTree.Comparer.Compare(edge, tmpSearchKey);

            if (comparisonResult < 0) {
                mostLeftNeighboringEdge = edge;
            } 
            else {
                break;
            }
        }
        if (mostLeftNeighboringEdge == null)
            throw new InvalidOperationException("no neighboring edge found for the vertex.");
        return mostLeftNeighboringEdge;
    }

    /// <summary>
    /// 対角線を生成するための処理
    /// イベントポイント (頂点) の頂点種類によって処理を分岐する
    /// インベントポイントは，図形の頂点リストを y 座標でソートした順で処理される
    /// </summary>
    /// <param name="eventCount"> 現在のイベントカウント </param>
    /// <param name="vertex"> イベントポイント (頂点) </param>
    private void ProcessSweepLineForMakeDiagonalEdge(LinkedVertexList linkedVertexList) {

        for (int i = 0; i < _indexBeforeSortY.Length; i++) {

            var vertex = _edgeList[_indexBeforeSortY[i].Item1][_indexBeforeSortY[i].Item2].Start;
            // 走査線の y 座標を設定する
            EdgeComparer.HorizonY = vertex.PlanePosition.y;
            // 走査対象の頂点に接続する辺を取得する
            var activeEdges = _edgeIntervalTree.GetEdgesPassThroughHorizon(vertex.PlanePosition.y);

            foreach (var edge in activeEdges) {
                _sortedXPositionEdgeInTree.Add(edge, edge);
            }

            switch (vertex.VertexType) {
                case VertexType.Regular:
                    HandleRegularVertex(vertex, _edgeList[_indexBeforeSortY[i].Item1][_indexBeforeSortY[i].Item2]);
                    break;
                case VertexType.Start:
                    HandleStartVertex(vertex, _edgeList[_indexBeforeSortY[i].Item1][_indexBeforeSortY[i].Item2]);
                    break;
                case VertexType.Merge:
                    HandleMergeVertex(vertex, _edgeList[_indexBeforeSortY[i].Item1][_indexBeforeSortY[i].Item2]);
                    break;
                case VertexType.Split:
                    HandleSplitVertex(vertex, _edgeList[_indexBeforeSortY[i].Item1][_indexBeforeSortY[i].Item2]);
                    break;
                case VertexType.End:
                    HandleEndVertex(vertex, _edgeList[_indexBeforeSortY[i].Item1][_indexBeforeSortY[i].Item2]);
                    break;
            }

            foreach (var edge in activeEdges) {
                _sortedXPositionEdgeInTree.Remove(edge);
            }
        }
    }

    /// <summary>
    /// イベントポイントが通常点 (Regular) の場合の処理メソッド
    /// </summary>
    /// <param name="eventCount"></param>
    /// <param name="vertex"></param>
    private void HandleRegularVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        NonConvexMonotoneCutSurfaceEdge edge
    ) {
        /** 
         * if P の内部が v[i] の右にある
         * - then if helper(e[i-1]) が統合点である
         * - - then v[i] と helper(e[i-1]) を結ぶ対角線を D に挿入する
         * - - e[i-1] を T から削除する
         * - - e[i] を T に挿入し，helper(e[i]) を v[i] にする
         * - else T の中を探索して，v[i] のすぐ左にある辺 e[j] を求める
         * - - if helper(e[j]) が統合点である
         * - - - then v[i] と helper(e[j]) を結ぶ対角線を D に挿入する
         * - - helper(e[j]) を v[i] にする
         */

    }

    private void HandleStartVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        NonConvexMonotoneCutSurfaceEdge edge
    ) {
        /**
         * e[i] を T に挿入し，helper(e[i]) を v[i] とする
         */
    }

    private void HandleMergeVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        NonConvexMonotoneCutSurfaceEdge edge
    ) {
        /**
         * if helper(e[i-1]) が統合点である
         * - then v[i] と helper(e[i-1]) を結ぶ対角線を D に挿入する
         * e[i-1] を T から削除する
         * T の中を探索して，v[i] のすぐ左にある辺 e[j] を求める
         * if helper(e[j]) が統合点である
         * - then v[i] と helper(e[j]) を結ぶ対角線を D に挿入する
         * helper(e[j]) を v[i] にする
         */
    }

    private void HandleSplitVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        NonConvexMonotoneCutSurfaceEdge edge
    ) {
        /**
         * T の中を探索して，v[i] のすぐ左にある辺 e[j] を求める
         * v[i] と helper(e[j]) を結ぶ対角線を D に挿入する
         * helper(e[j]) を v[i] にする
         * e[i] を T に挿入し，helper(e[i]) を v[i] とする
         */
    }

    private void HandleEndVertex(
        NonConvexMonotoneCutSurfaceVertex vertex,
        NonConvexMonotoneCutSurfaceEdge edge
    ) {
        /**
         * if helper(e[i-1]) が統合点である
         * - then v[i] と helper(e[i-1]) を結ぶ対角線を D に挿入する
         * e[i-1] を T から削除する
         */
    }

    private void AddDiagonalEdge(
        NonConvexMonotoneCutSurfaceVertex startVertex,
        NonConvexMonotoneCutSurfaceVertex endVertex
    ) {
        DiagonalSet.Add((startVertex, endVertex));
        DiagonalSet.Add((endVertex, startVertex));
    }
}
