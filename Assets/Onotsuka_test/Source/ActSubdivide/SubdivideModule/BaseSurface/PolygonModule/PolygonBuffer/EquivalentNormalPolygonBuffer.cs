using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 法線方向ごとにポリゴンの情報を保持するクラス
/// </summary>
public class EquivalentNormalPolygonBuffer {

    /// <summary>
    /// 法線の向きが同じ連結ポリゴン情報を管理するための辞書
    /// </summary>
    private Dictionary<int, LinkedPolygonList> _polygonDictionary = new();

    

    /// <summary>
    /// 同じ法線のポリゴンを、マージ判定リストに追加するメソッド
    /// </summary>
    /// <param name="normal"> ポリゴンの法線ベクトル (正規化) </param>
    /// <param name="polygon"> ポリゴン情報 </param>
    public void Add(Vector3 normal, NewPolygon polygon) {
        int castedNormal = NormalizedVector3ToInt(normal);

        // 登録されている法線の場合は、同じ法線の連結ポリゴンリストに追加する
        if (_polygonDictionary.TryGetValue(castedNormal, out LinkedPolygonList linkedPolygonList)) {
            linkedPolygonList.Add(polygon);
        }
        // 登録されていない法線の場合は、新規登録する
        else {
            _polygonDictionary[castedNormal] = new LinkedPolygonList();
            _polygonDictionary[castedNormal].Add(polygon);
        }
    }

    /// <summary>
    /// リスト内の連結ポリゴンに対して、ポリゴン生成を行うメソッド
    /// </summary>
    /// <param name="localPlane"> 切断平面 </param>
    /// <param name="trackerArray"> 切断後の配属頂点インデックスを格納する配列 </param>
    /// <param name="originMesh"> 切断前メッシュ (MeshContainer) </param>
    /// <param name="frontsideMesh"> 切断後法線側メッシュ (MeshContainer) </param>
    /// <param name="backsideMesh"> 切断後反法線側メッシュ (MeshContainer) </param>
    public void MakeBaseSurfacePolygon(
        Plane localPlane,
        int[] trackerArray,
        MeshContainer originMesh,
        MeshContainer frontsideMesh,
        MeshContainer backsideMesh,
        CutSurfacePolygonBuffer cutSurfacePolygonBuffer
    ) {
        foreach (var pair in _polygonDictionary) {
            int normal = pair.Key;

            LinkedPolygonList linkedPolygonList = pair.Value;
            linkedPolygonList.MakePolygon(
                localPlane,
                trackerArray,
                originMesh,
                frontsideMesh,
                backsideMesh,
                cutSurfacePolygonBuffer
            );
        }
    }

    /// <summary>
    /// 法線ベクトルを int 型に圧縮するメソッド
    /// 0.0 ~ 1.0 の範囲のベクトルをスケーリングして、上位 22bit を制限し、(x, y, z)10bit ずつの情報に圧縮する
    /// およそ各小数点以下第二位までの精度となる (第三位まで許容したければ、(long) にして 14bit(16384) シフトして対応する上位ビットを制限する)
    /// </summary>
    /// <param name="vector"> 法線ベクトル </param>
    /// <returns> int 型へ圧縮した法線 </returns>
    private int NormalizedVector3ToInt(Vector3 vector) {
        // 0 ~ 1023 の範囲に制限する (下位 10bit のみが対象)
        int filter = 0x000003FF;
        int amp = 1 << 10;

        int x = ((int)(vector.x * amp) & filter) << 20;
        int y = ((int)(vector.y * amp) & filter) << 10;
        int z = ((int)(vector.z * amp) & filter);

        return x | y | z;
    }
}
