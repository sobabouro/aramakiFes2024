using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// 切断平面上の y 単調な多角形 (閉パス) のリストを管理するクラス
/// </summary>
public class MonotoneGeometryPathList {

    /// <summary>
    /// すべての頂点に対しての，接続する図形へのマッピング
    /// </summary>
    private Dictionary<NonConvexMonotoneCutSurfaceVertex, List<NonConvexMonotoneCutSurfaceEdge>> _map;

    /// <summary>
    /// 切断平面上の y 単調な多角形のパスリスト
    /// </summary>
    private List<MonotoneGeometryPath> _pathList;

    /// <summary>
    /// すべての図形の中で最も高い X 座標を持つ頂点の位置
    /// UV 座標の構築に使用する
    /// </summary>
    private BoundingBox _boundingBox;

    /// <summary>
    /// コンストラクタ
    /// ここでパスの生成を行う
    /// </summary>
    /// <param name="linkedVertexList"> 切断平面上の非単調である可能性のある多角形リスト </param>
    /// <param name="diagonalSet"> 切断平面上のすべての非単調多角形を y に単調な多角形に分割するための対角線集合 </param>
    public MonotoneGeometryPathList(
        LinkedVertexList linkedVertexList, 
        HashSet<(NonConvexMonotoneCutSurfaceVertex, NonConvexMonotoneCutSurfaceVertex)> diagonalSet
    ) {
        _map = new Dictionary<NonConvexMonotoneCutSurfaceVertex, List<NonConvexMonotoneCutSurfaceEdge>>();
        _pathList = new List<MonotoneGeometryPath>();
        _boundingBox = new BoundingBox();

        /**
         * ここで，_linkedVertexList の重複頂点削除が済んでいるかチェックする必要あり．
         * diagonalEdgeGenerator で重複頂点削除は行っているはずだが，参照渡ししてないので要確認
         */

        // 元の辺を追加する
        foreach (var linkedVertex in linkedVertexList) {

            if (linkedVertex.First == null)
                continue;

            var firstNode = linkedVertex.First;
            var currNode = firstNode;

            do {
                var nextNode = linkedVertex.TorusNext(currNode);
                AddEdgeToMap(currNode.Value, nextNode.Value);
                UpdateMostHighestLowestPosition(currNode.Value);
                currNode = nextNode;
            }
            while (!currNode.Equals(linkedVertex.First));
        }

        // 対角線を追加する
        foreach (var diagonal in diagonalSet) {
            AddEdgeToMap(diagonal.Item1, diagonal.Item2);
            AddEdgeToMap(diagonal.Item2, diagonal.Item1);
        }

        // パス探索で訪問済みの辺を追跡するための集合
        HashSet<NonConvexMonotoneCutSurfaceEdge> visitedEdgeSet = new();

        foreach (var keyVertex in _map.Keys) {

            // 未訪問の頂点を始点とする辺から新しいパスを探索する
            foreach (var currEdge in _map[keyVertex]) {

                // 既に訪問済みの辺はスキップする
                if (visitedEdgeSet.Contains(currEdge))
                    continue;

                // 新しいパスの探索用
                int limit = 0;

                bool isClosedPath = false;
                MonotoneGeometryPath currPath = new();
                NonConvexMonotoneCutSurfaceVertex startVertex = currEdge.Start;
                NonConvexMonotoneCutSurfaceVertex currVertex = currEdge.Start;
                NonConvexMonotoneCutSurfaceVertex prevVertex = null;

                // パスの構築を始める最初の連結頂点を追加する
                currPath.AddLast(currEdge.Start);
                currPath.AddLast(currEdge.End);
                visitedEdgeSet.Add(currEdge);

                currVertex = currEdge.End;
                prevVertex = currEdge.Start;

                // パスの走査を開始する
                while (!isClosedPath) {
                    limit++;
                    if (limit > 1000) {
                        Debug.LogWarning($"MonotoneGeometryPathList: Path search limit exceeded for vertex {startVertex.PlanePosition}. Possible infinite loop detected.");
                        break;
                    }

                    if (!_map.ContainsKey(currVertex))
                        break;

                    foreach (var nextEdge in _map[currVertex]) {
                        // 直前の頂点に戻る辺 (頂点)、または既に使われた辺 (頂点) ではない，接続する頂点であれば更新する
                        if (nextEdge.Start.Equals(currVertex) && !nextEdge.End.Equals(prevVertex) && !visitedEdgeSet.Contains(nextEdge)) {

                            if (nextEdge.End.Equals(startVertex)) {
                                isClosedPath = true;
                                Debug.Log($"MonotoneGeometryPathList: Closed path found starting from {startVertex.PlanePosition}.");
                            }

                            visitedEdgeSet.Add(nextEdge);
                            currPath.AddLast(nextEdge.End);

                            currVertex = nextEdge.End;
                            prevVertex = nextEdge.Start;
                            break;
                        }
                    }
                }

                // パスの最終チェックを行い，パスリストに閉パスを追加する
                if (currVertex != null && currPath.Count > 2) {
                    _pathList.Add(currPath);
                }
            }
        }
    }

    /// <summary>
    /// マップに辺を追加するメソッド
    /// 既に同じ辺が存在しないかチェックする（双方向の重複も考慮している）
    /// </summary>
    /// <param name="currVertex"> 現在の頂点 </param>
    /// <param name="nextVertex"> 次の頂点 </param>
    private void AddEdgeToMap(
        NonConvexMonotoneCutSurfaceVertex currVertex,
        NonConvexMonotoneCutSurfaceVertex nextVertex
    ) {

        if (!_map.ContainsKey(currVertex)) {
            _map[currVertex] = new List<NonConvexMonotoneCutSurfaceEdge>();
        }

        NonConvexMonotoneCutSurfaceEdge edge = new NonConvexMonotoneCutSurfaceEdge(currVertex, nextVertex);
        // 双方向の辺を考慮し、既に存在しないかチェックする
        if (!_map[currVertex].Contains(edge)) {
            _map[currVertex].Add(edge);
        }
    }

    /// <summary>
    /// 切断平面上の y 単調な多角形のパスからポリゴンを生成するメソッド
    /// </summary>
    /// <param name="localPlane"> ローカル座標系の切断平面 </param>
    /// <param name="addCutSurfaceMaterial"> 切断面に新規マテリアルを割り当てるかどうか </param>
    /// <param name="frontsideMesh"> 切断後の法線側メッシュ </param>
    /// <param name="backsideMesh"> 切断後の反法線側メッシュ </param>
    public void MakePolygon(
        Plane localPlane,
        bool addCutSurfaceMaterial,
        MeshContainer frontsideMesh,
        MeshContainer backsideMesh
    ) {
        foreach (var path in _pathList) {
            if (path.Count < 3)
                continue;

            path.MakePolygon(
                _boundingBox,
                localPlane,
                addCutSurfaceMaterial,
                frontsideMesh,
                backsideMesh
            );
        }
    }

    /// <summary>
    /// 切断面のバウンディングボックスを更新するためのメソッド
    /// x, y 座標の最も高い位置と最も低い位置を更新する
    /// </summary>
    /// <param name="vertex"> 追加する頂点 </param>
    private void UpdateMostHighestLowestPosition(NonConvexMonotoneCutSurfaceVertex vertex) {
        _boundingBox.TryUpdate(vertex.PlanePosition.x, vertex.PlanePosition.y);
    }
}
