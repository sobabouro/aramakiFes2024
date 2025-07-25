using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CalculationUtils;

/// <summary>
/// 切断平面上の y 単調な多角形のパスを管理するクラス
/// </summary>
public class MonotoneGeometryPath : IEnumerable<NonConvexMonotoneCutSurfaceVertex> {

    /// <summary>
    /// パスの双方向リスト
    /// </summary>
    private LinkedList<NonConvexMonotoneCutSurfaceVertex> _path = new();

    /// <summary>
    /// パスの要素数を取得するプロパティ
    /// </summary>
    public int Count => _path.Count;

    /// <summary>
    /// パスのイテレータを返すメソッド
    /// </summary>
    /// <returns> シーケンスのリストを列挙するためのイテレータ </returns>
    public IEnumerator<NonConvexMonotoneCutSurfaceVertex> GetEnumerator() => _path.GetEnumerator();

    /// <summary>
    /// IEnumerable インターフェースの GetEnumerator メソッドの実装
    /// </summary>
    /// <returns> パスを列挙するためのイテレータ </returns>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// パスの中で最も高い頂点のノード
    /// </summary>
    private LinkedListNode<NonConvexMonotoneCutSurfaceVertex> _mostHighestNode = null;

    /// <summary>
    /// パスの中で最も低い頂点のノード
    /// </summary>
    private LinkedListNode<NonConvexMonotoneCutSurfaceVertex> _mostLowestNode = null;

    /// <summary>
    /// パスの中で最も高い頂点の Y 座標
    /// </summary>
    private float _mostHighestYPosition = float.MinValue;

    /// <summary>
    /// パスの中で最も低い頂点の Y 座標
    /// </summary>
    private float _mostLowestYPosition = float.MaxValue;

    /// <summary>
    /// パスの先頭に頂点を追加するメソッド
    /// </summary>
    /// <param name="vertex"> 追加する頂点 </param>
    public void AddFirst(NonConvexMonotoneCutSurfaceVertex vertex) {
        _path.AddFirst(vertex);

        if (vertex.PlanePosition.y >= _mostHighestYPosition) {
            if (vertex.PlanePosition.y == _mostHighestYPosition && vertex.PlanePosition.x < _mostHighestNode.Value.PlanePosition.x) {
                _mostHighestNode = _path.First;
            }
            _mostHighestYPosition = vertex.PlanePosition.y;
            _mostHighestNode = _path.First;
        }
        if (vertex.PlanePosition.y <= _mostLowestYPosition) {
            if (vertex.PlanePosition.y == _mostLowestYPosition && vertex.PlanePosition.x > _mostLowestNode.Value.PlanePosition.x) {
                _mostLowestNode = _path.First;
            }
            _mostLowestYPosition = vertex.PlanePosition.y;
            _mostLowestNode = _path.First;
        }
    }

    /// <summary>
    /// パスの末尾に頂点を追加するメソッド
    /// </summary>
    /// <param name="vertex"> 追加する頂点 </param>
    public void AddLast(NonConvexMonotoneCutSurfaceVertex vertex) {
        _path.AddLast(vertex);

        if (vertex.PlanePosition.y >= _mostHighestYPosition) {
            if (vertex.PlanePosition.y == _mostHighestYPosition && vertex.PlanePosition.x < _mostHighestNode.Value.PlanePosition.x) {
                _mostHighestNode = _path.Last;
            }
            _mostHighestYPosition = vertex.PlanePosition.y;
            _mostHighestNode = _path.Last;
        }
        if (vertex.PlanePosition.y <= _mostLowestYPosition) {
            if (vertex.PlanePosition.y == _mostLowestYPosition && vertex.PlanePosition.x > _mostLowestNode.Value.PlanePosition.x) {
                _mostLowestNode = _path.Last;
            }
            _mostLowestYPosition = vertex.PlanePosition.y;
            _mostLowestNode = _path.Last;
        }
    }

    /// <summary>
    /// 循環ノードのように各ノードにアクセスするためのメソッド
    /// 次のノードを取得するメソッド
    /// </summary>
    /// <param name="node"> 対象ノード </param>
    /// <returns> 対象ノードの次のノード </returns>
    public LinkedListNode<NonConvexMonotoneCutSurfaceVertex> TorusNext(LinkedListNode<NonConvexMonotoneCutSurfaceVertex> node) {

        if (node == null || _path.Count == 0)
            return null;

        return node.Next ?? _path.First;
    }

    /// <summary>
    /// 循環ノードのように各ノードにアクセスするためのメソッド
    /// 前のノードを取得するメソッド
    /// </summary>
    /// <param name="node"> 対象ノード </param>
    /// <returns> 対象ノードの前のノード </returns>
    public LinkedListNode<NonConvexMonotoneCutSurfaceVertex> TorusPrevious(LinkedListNode<NonConvexMonotoneCutSurfaceVertex> node) {

        if (node == null || _path.Count == 0)
            return null;

        return node.Previous ?? _path.Last;
    }

    public void MakePolygon(
        BoundingBox boundingBox,
        Plane localPlane,
        bool addCutSurfaceMaterial, 
        MeshContainer frontsideMesh,
        MeshContainer backsideMesh
    ) {
        /**
         * # 図形の中で最も y 座標が高い頂点から最も低い頂点まで辿る際の，右側境界の辺群と左側境界の辺群にそれぞれどちらの群 (チェイン) に属するかを設定する 
         * # その後，すべての頂点を y 座標の降順にソートし，スタック (S) を用意し，左側の辺群と右側の辺群を統一した順序 (u[1], u[2], ..., u[n]) で以下の処理を行う
         * 
         * u[1], u[2] をスタックにプッシュする
         * for i <- 3 to n-1
         * - do if u[i] と S の一番上の頂点が異なるチェイン上にある
         * - - then S からすべての頂点をポップする
         * - - - u[i] とポップされたそれぞれの頂点を結ぶ対角線を D に挿入する．ただし，最後の頂点だけは除く
         * - - - u[i-1] と u[i] を S にプッシュする
         * - - else S から一つの頂点をポップする
         * - - - u[i] からの対角線が P の内部にある限り，S から他の頂点をポップする
         * - - - これらの対角線を D に挿入する
         * - - - ポップされた最後の頂点をスタックに戻す
         * - - - u[i] を S にプッシュする
         * 最初と最後の頂点を覗いて，u[n] からスタック上のすべての頂点への対角線を加える
         * 
         * 
         * ※ 以下実装では，対角線をリストに追加するのではなく，ポリゴンを直接生成する
         */
        ClusteringSideType();
        NonConvexMonotoneCutSurfaceVertex[] sortedArray = SortVertexYPosition();
        Stack<NonConvexMonotoneCutSurfaceVertex> stack = new();

        stack.Push(sortedArray[0]);
        stack.Push(sortedArray[1]);

        for (int i = 2; i < sortedArray.Length; i++) {

            if (stack.Peek().SideType != sortedArray[i].SideType) {
                while (stack.Count > 0) {

                    var point1 = stack.Pop();
                    var point2 = stack.Count > 0 ? stack.Pop() : stack.Peek();

                    CreateTriangle(
                        (point1, point2, sortedArray[i]),
                        boundingBox,
                        localPlane,
                        frontsideMesh,
                        backsideMesh,
                        addCutSurfaceMaterial
                    );
                }
                stack.Push(sortedArray[i - 1]);
                stack.Push(sortedArray[i]);
            } else {
                bool isContinue = true;

                while (stack.Count > 0 && isContinue) {
                    isContinue = false;

                    var point1 = stack.Pop();
                    var point2 = stack.Count > 0 ? stack.Peek() : stack.Pop();

                    // 左側境界を走査中に，処理頂点が結ぶ対角線が図形内部にある場合 (直近三頂点が順に時計回りに並ぶ場合) 
                    if (sortedArray[i].SideType == SideType.Left && Calculation.IsClockwise(sortedArray[i].PlanePosition, point2.PlanePosition, point1.PlanePosition)) {
                        CreateTriangle(
                            (sortedArray[i], point2, point1),
                            boundingBox,
                            localPlane,
                            frontsideMesh,
                            backsideMesh,
                            addCutSurfaceMaterial
                        );
                        if (stack.Count == 0)
                            stack.Push(point2);
                        stack.Push(sortedArray[i]);

                        isContinue = true;
                    }
                    // 右側境界を走査中に，処理頂点が結ぶ対角線が図形内部にある場合 (直近三頂点が順に時計回りに並ぶ場合)
                    else if (sortedArray[i].SideType == SideType.Right && Calculation.IsClockwise(sortedArray[i].PlanePosition, point1.PlanePosition, point2.PlanePosition)) {
                        CreateTriangle(
                            (point1, sortedArray[i], point2),
                            boundingBox,
                            localPlane,
                            frontsideMesh,
                            backsideMesh,
                            addCutSurfaceMaterial
                        );
                        if (stack.Count == 0)
                            stack.Push(point2);
                        stack.Push(sortedArray[i]);

                        isContinue = true;
                    }
                    // 図形内部に対角線が引けない場合 (辺が反っていて，辺を弓とすると対角線が弦となる形で図形の外部に結ばれてしまう)
                    else {
                        if (stack.Count == 0)
                            stack.Push(point2);
                        stack.Push(point1);
                        stack.Push(sortedArray[i]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 連結図形辺シーケンスの各頂点に対して，その頂点が属する図形の y 最大地点と最小地点までを結ぶ二つの境界のうち，どちら側に位置するかを設定するメソッド
    /// </summary>
    private void ClusteringSideType() {

        var currentNode = _mostHighestNode;
        currentNode.Value.SideType = SideType.Top;

        currentNode = TorusNext(currentNode);

        while (currentNode != _mostLowestNode) {
            currentNode.Value.SideType = SideType.Left;

            currentNode = TorusNext(currentNode);
        }
        currentNode.Value.SideType = SideType.Bottom;

        currentNode = TorusNext(currentNode);

        while (currentNode != _mostHighestNode) {
            currentNode.Value.SideType = SideType.Right;

            currentNode = TorusNext(currentNode);
        }
    }

    /// <summary>
    /// 連結図形辺シーケンスの頂点を Y 座標の降順にソートするメソッド
    /// </summary>
    /// <returns> ソートされた頂点配列 </returns>
    private NonConvexMonotoneCutSurfaceVertex[] SortVertexYPosition() {

        NonConvexMonotoneCutSurfaceVertex[] sortedArray = _path
            .OrderByDescending(vertex => vertex.PlanePosition.y)
            .ToArray();

        return sortedArray;
    }

    /// <summary>
    /// 三角形を生成するメソッド
    /// </summary>
    /// <param name="triangle"> 三角形を構成する三頂点 </param>
    /// <param name="boundingBox"> UV 座標決定のための外枠 </param>
    /// <param name="localPlane"> 切断平面 </param>
    /// <param name="frontsideMesh"> 法線側メッシュ </param>
    /// <param name="backsideMesh"> 反法線側メッシュ </param>
    /// <param name="addCutSurfaceMaterial"></param>
    private void CreateTriangle(
        (NonConvexMonotoneCutSurfaceVertex, NonConvexMonotoneCutSurfaceVertex, NonConvexMonotoneCutSurfaceVertex) triangle,
        BoundingBox boundingBox,
        Plane localPlane,
        MeshContainer frontsideMesh,
        MeshContainer backsideMesh,
        bool addCutSurfaceMaterial = false
    ) {
        Vector3 triangleNormal = Vector3.Cross(
            triangle.Item2.LocalPosition - triangle.Item1.LocalPosition,
            triangle.Item3.LocalPosition - triangle.Item1.LocalPosition
        ).normalized;

        Vector3 vertex1 = triangle.Item1.LocalPosition;
        Vector3 vertex2 = triangle.Item2.LocalPosition;
        Vector3 vertex3 = triangle.Item3.LocalPosition;

        Vector2 uv1 = new(
            triangle.Item1.PlanePosition.x - boundingBox.MinX / boundingBox.Width,
            triangle.Item1.PlanePosition.y - boundingBox.MinY / boundingBox.Height
        );
        Vector2 uv2 = new(
            triangle.Item2.PlanePosition.x - boundingBox.MinX / boundingBox.Width,
            triangle.Item2.PlanePosition.y - boundingBox.MinY / boundingBox.Height
        );
        Vector2 uv3 = new(
            triangle.Item3.PlanePosition.x - boundingBox.MinX / boundingBox.Width,
            triangle.Item3.PlanePosition.y - boundingBox.MinY / boundingBox.Height
        );

        int materialIndex = 0;

        if (addCutSurfaceMaterial)
            materialIndex = frontsideMesh.SubmeshCount;

        frontsideMesh.AddMesh(
            materialIndex,
            triangleNormal,
            new Vector3[] { vertex1, vertex2, vertex3 },
            new Vector3[] { localPlane.normal, localPlane.normal, localPlane.normal },
            new Vector2[] { uv1, uv2, uv3 }
        );
        backsideMesh.AddMesh(
            materialIndex,
            triangleNormal,
            new Vector3[] { vertex1, vertex3, vertex2 },
            new Vector3[] { -localPlane.normal, -localPlane.normal, -localPlane.normal },
            new Vector2[] { uv1, uv3, uv2 }
        );
    }
}
