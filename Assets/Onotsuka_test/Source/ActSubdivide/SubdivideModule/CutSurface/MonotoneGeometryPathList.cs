using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    private BoundingBox _boundingBox = new BoundingBox();

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

        /**
         * ここで，_linkedVertexList の重複頂点削除が済んでいるかチェックする必要あり．
         * diagonalEdgeGenerator で重複頂点削除は行っているはずだが，参照渡ししてないので要確認
         */

        // 元の辺を追加する
        foreach (var linkedVertex in linkedVertexList) {
            var currentNode = linkedVertex.First;
            while (currentNode != null && currentNode.Next != null) {
                AddEdgeToMap(currentNode.Value, linkedVertex.TorusNext(currentNode).Value);
                AddEdgeToMap(linkedVertex.TorusNext(currentNode).Value, currentNode.Value);

                UpdateMostHighestLowestPosition(currentNode.Value);
                currentNode = currentNode.Next;
            }
        }

        // 対角線を追加する
        foreach (var diagonal in diagonalSet) {
            AddEdgeToMap(diagonal.Item1, diagonal.Item2);
            AddEdgeToMap(diagonal.Item2, diagonal.Item1);
        }

        // パス探索で訪問済みの辺を追跡するための集合
        HashSet<NonConvexMonotoneCutSurfaceEdge> visitedEdges = new();

        foreach (var startVertex in _map.Keys) {

            // 未訪問の頂点を始点とする辺から新しいパスを探索する
            foreach (var initialEdge in _map[startVertex]) {

                // 既に訪問済みの辺はスキップする
                if (visitedEdges.Contains(initialEdge))
                    continue;

                // 新しいパスの探索用
                MonotoneGeometryPath currentPath = new();
                NonConvexMonotoneCutSurfaceVertex current = initialEdge.Start;
                NonConvexMonotoneCutSurfaceVertex previous = null;

                // パスの構築を始める最初の連結頂点を追加する
                currentPath.AddLast(initialEdge.Start);
                currentPath.AddLast(initialEdge.End);
                visitedEdges.Add(initialEdge);

                current = initialEdge.End;
                previous = initialEdge.Start;

                // パスの走査を開始する
                while (current != null && !current.Equals(initialEdge.Start)) {

                    bool foundNext = false;
                    if (!_map.ContainsKey(current))
                        break;

                    foreach (var nextEdge in _map[current]) {
                        var nextVertex = nextEdge.Start.Equals(current) ? nextEdge.End : nextEdge.Start;

                        // 直前の頂点に戻る辺 (頂点)、または既に使われた辺 (頂点) はスキップ
                        if (nextVertex.Equals(previous) || visitedEdges.Contains(nextEdge) || visitedEdges.Contains(nextEdge.GetReverseEdge())) {
                            continue;
                        }

                        // パスがループしたかチェックする (始点が重複したタイミングで発火する)
                        if (currentPath.Contains(nextVertex)) {

                            if (nextVertex.Equals(initialEdge.Start)) {
                                foundNext = true;
                                break;
                            }
                            // 始点に戻る閉パスではないが，閉じたパスをキャッチする
                            continue;
                        }

                        // 新しい頂点をパスに追加する
                        currentPath.AddLast(nextVertex);
                        visitedEdges.Add(nextEdge);

                        // 走査用情報を更新する
                        previous = current;
                        current = nextVertex;
                        foundNext = true;
                        break;
                    }

                    // 走査によってパスが閉じなかった場合は警告処理を行う
                    if (!foundNext) {
                        Debug.LogWarning($"MonotoneGeometryPathList: No next vertex found for current vertex {current.PlanePosition}. Path may be incomplete.");
                    }
                }

                // パスの最終チェックを行い，パスリストに閉パスを追加する
                if (current != null && current.Equals(initialEdge.Start) && currentPath.Count > 2) {

                    MonotoneGeometryPath newPath = new();

                    foreach (var vertex in currentPath) {
                        newPath.AddLast(vertex);
                    }
                    _pathList.Add(newPath);
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
