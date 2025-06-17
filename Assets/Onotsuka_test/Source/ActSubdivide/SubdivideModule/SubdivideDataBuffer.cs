using System.Collections;
using UnityEngine;

/// <summary>
/// 切断処理が適用されるデータのバッファクラス
/// </summary>
public class SubdivideDataBuffer {

    private bool _isReady = false;

    /// <summary>
    /// 法線方向ごとにポリゴンの情報を保持するバッファ
    /// </summary>
    private EquivalentNormalPolygonBuffer _polygonBuffer = new();

    private LinkedVertexList _linkedVertexList = new();

    /// <summary>
    /// 新しいポリゴン情報を追加するメソッド
    /// </summary>
    /// <param name="submeshGroupNumber"> 所属しているサブメッシュのグループ番号 </param>
    /// <param name="polygonNormal"> 追加するポリゴンの法線 </param>
    /// <param name="input"> 新情報生成用に整形されたデータ (SideIndexInfo) </param>
    public void AddData(int submeshGroupNumber, Vector3 polygonNormal, SideIndexInfo input) {
        NewVertex toward = new(
            input.FrontAwayIndex,
            input.BackTowardIndex
        );
        NewVertex away = new(
            input.FrontTowardIndex,
            input.BackAwayIndex
        );
        NewPolygon polygon = new TrianglePolygon(
            submeshGroupNumber,
            toward,
            away,
            input.IsFrontSideEdge
        );
        _polygonBuffer.Add(polygonNormal, polygon);
    }

    public void MakePolygon(Plane localPlane, int[] trackerArray, MeshContainer originMesh, MeshContainer frontsideMesh, MeshContainer backsideMesh) {
        _polygonBuffer.MakePolygon(localPlane, trackerArray, originMesh, frontsideMesh, backsideMesh, _linkedVertexList);

        for (int i = 0; i < _linkedVertexList.Count; i++) {
            Debug.Log($"Linked Vertex {i}: ");
            var linkedVertex = _linkedVertexList[i];
            linkedVertex.Display();
        }
    }
}
