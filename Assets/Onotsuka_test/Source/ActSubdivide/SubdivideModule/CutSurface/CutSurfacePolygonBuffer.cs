using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class CutSurfacePolygonBuffer {

    /// <summary>
    /// ローカル座標系の切断平面
    /// </summary>
    private Plane _localPlane;

    /// <summary>
    /// 切断平面上の連結頂点のリスト
    /// 図形的には非単調である可能性のある多角形がリストとして保存される
    /// </summary>
    private LinkedVertexList _linkedVertexList;

    ///// <summary>
    ///// 切断平面上の図形を y 単調な多角形に分割した後の連結頂点のリスト
    ///// 図形的には y 単調な多角形がリストとして保存される
    ///// </summary>
    //private LinkedMonotoneGeometryVertexList _linkedMonotoneGeometryVertexList;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="localPlane"> ローカル座標系の切断平面 </param>
    public CutSurfacePolygonBuffer(Plane localPlane) {
        _localPlane = localPlane;
        _linkedVertexList = new LinkedVertexList();
        //_linkedMonotoneGeometryVertexList = new LinkedMonotoneGeometryVertexList();
    }

    /// <summary>
    /// 切断平面上の頂点を連結頂点リストに追加するメソッド
    /// </summary>
    /// <param name="towardPosition"></param>
    /// <param name="awayPosition"></param>
    public void AddVertex(
        Vector3 towardPosition,
        Vector3 awayPosition
    ) {
        NonConvexMonotoneCutSurfaceVertex towardCutSurfaceVertex = new(_localPlane, towardPosition);
        NonConvexMonotoneCutSurfaceVertex awayCutSurfaceVertex = new(_localPlane, awayPosition);

        _linkedVertexList.Add(towardCutSurfaceVertex, awayCutSurfaceVertex);
    }

    public void MakeCutSurfacePolygon(
        MeshContainer frontsideMesh,
        MeshContainer backsideMesh, 
        bool addCutSurfaceMaterial = false
    ) {
        MakeMonotoneGeometry();

        //_linkedMonotoneGeometryVertexList.MakePolygon(
        //    _localPlane,
        //    frontsideMesh,
        //    backsideMesh,
        //    addCutSurfaceMaterial
        //);
    }

    public void MakeMonotoneGeometry() {
        if (_linkedVertexList.Count == 0) {
            Debug.LogWarning("CutSurfacePolygonBuffer: No vertices to process.");
            return;
        }
        // すべての非単調な多角形が閉じたパス (図形) として保存しているリストから，y に単調な多角形に分割するための対角線を生成して，すべてのパスを保存する生成系をインスタンス化する
        DiagonalEdgeGenerator diagonalEdgeGenerator = new DiagonalEdgeGenerator(_linkedVertexList);

        HashSet<(NonConvexMonotoneCutSurfaceVertex, NonConvexMonotoneCutSurfaceVertex)> diagonalSet = diagonalEdgeGenerator.GetDiagonalSet();

        // すべての頂点に対して，接続する図形へのマッピングを行う
        Dictionary<NonConvexMonotoneCutSurfaceVertex, List<NonConvexMonotoneCutSurfaceEdge>> map = new();

        /**
         * ここで，_linkedVertexList の重複頂点削除が済んでいるかチェックする必要あり．
         * diagonalEdgeGenerator で重複頂点削除は行っているはずだが，参照渡ししてないので要確認
         */

        // 元の辺を追加する
        foreach (var linkedVertex in _linkedVertexList) {
            var currentNode = linkedVertex.First;
            while (currentNode != null && currentNode.Next != null) {
                AddEdgeToMap(map, currentNode.Value, linkedVertex.TorusNext(currentNode).Value);
                currentNode = currentNode.Next;
            }
        }

        // 対角線を追加する
        foreach (var diagonal in diagonalSet) {
            AddEdgeToMap(map, diagonal.Item1, diagonal.Item2);
            AddEdgeToMap(map, diagonal.Item2, diagonal.Item1);
        }

        // パス探索で訪問済みの辺を追跡するための集合
        HashSet<NonConvexMonotoneCutSurfaceEdge> visitedEdges = new();

        foreach (var startVertex in map.Keys) {

            // 未訪問の頂点を始点とする辺から新しいパスを探索する
            foreach (var initialEdge in map[startVertex]) {

                // 既に訪問済みの辺はスキップする
                if (visitedEdges.Contains(initialEdge))
                    continue;

                // 新しいパスの探索用
                LinkedList<NonConvexMonotoneCutSurfaceVertex> currentPath = new();
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
                    if (!map.ContainsKey(current))
                        break;

                    foreach (var nextEdge in map[current]) {
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
                        Debug.LogWarning($"CutSurfacePolygonBuffer: No next vertex found for current vertex {current.PlanePosition}. Path may be incomplete.");
                    }
                }

                // パスの最終チェックを行い，
                if (current != null && current.Equals(initialEdge.Start) && currentPath.Count > 2) {
                    //// 閉じたパスを_linkedMonotoneGeometryVertexListに追加
                    //// 実際にはLinkedMonotoneGeometryVertexList.Add(始点, 終点) のループで追加する必要がある
                    //// ここではLinkedMonotoneGeometryVertexがLinkedList<NonConvexMonotoneCutSurfaceVertex>のような構造であると仮定
                    //LinkedMonotoneGeometryVertex newMonotonePoly = new LinkedMonotoneGeometryVertex();
                    //foreach (var v in currentPath) {
                    //    newMonotonePoly.AddNode(v); // 適切なAddメソッドに置き換える
                    //}
                    //_linkedMonotoneGeometryVertexList.Add(newMonotonePoly); // LinkedList全体を追加するメソッドがあれば

                    //// または、LinkedMonotoneGeometryVertexList の Add が辺を受け取ることを利用
                    //for (int k = 0; k < currentPath.Count - 1; k++) {
                    //    _linkedMonotoneGeometryVertexList.Add(currentPath.ElementAt(k), currentPath.ElementAt(k + 1));
                    //}
                }
            }
        }

        //// ここで_linkedMonotoneGeometryVertexListは正しくモノトーン多角形を構成する連結リストになっているはず
        //// ログ出力部分はデバッグ用なので残しておきます
        //foreach (var linkedVertex in _linkedMonotoneGeometryVertexList) {
        //    linkedVertex.DeleteLastElement(); // これはなぜ必要なのか要確認
        //}

        //for (int i = 0; i < _linkedMonotoneGeometryVertexList.Count; i++) {
        //    Debug.Log($"CutSurfacePolygonBuffer: LinkedMonotoneGeometryVertexList[{i}] -");
        //    foreach (var vertex in _linkedMonotoneGeometryVertexList[i]) {
        //        Debug.Log($"  Vertex: {vertex.PlanePosition}, Type: {vertex.VertexType}");
        //    }
        //}
    }

    /// <summary>
    /// グラフに辺を追加するヘルパーメソッド
    /// 既に同じ辺が存在しないかチェックする（双方向の重複も考慮）
    /// </summary>
    private void AddEdgeToMap(
        Dictionary<NonConvexMonotoneCutSurfaceVertex, List<NonConvexMonotoneCutSurfaceEdge>> map,
        NonConvexMonotoneCutSurfaceVertex v1,
        NonConvexMonotoneCutSurfaceVertex v2) {
        if (!map.ContainsKey(v1)) {
            map[v1] = new List<NonConvexMonotoneCutSurfaceEdge>();
        }
        if (!map.ContainsKey(v2)) {
            map[v2] = new List<NonConvexMonotoneCutSurfaceEdge>();
        }

        NonConvexMonotoneCutSurfaceEdge edge = new NonConvexMonotoneCutSurfaceEdge(v1, v2);
        // 双方向の辺を考慮し、既に存在しないかチェック
        // EdgeのEqualsとGetHashCodeが正しく実装されていることが前提
        if (!map[v1].Any(e => e.Equals(edge) || e.Equals(new NonConvexMonotoneCutSurfaceEdge(v2, v1)))) {
            map[v1].Add(edge);
        }
    }
}
