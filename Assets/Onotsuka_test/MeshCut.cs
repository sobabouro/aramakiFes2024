using System;
using System.Collections.Generic;
using UnityEngine;


public class MeshCut : MonoBehaviour {
    static Mesh _targetMesh;
    static Vector3[] _targetVertices;
    static Vector3[] _targetNormals;
    static Vector2[] _targetUVs;   //この3つはめっちゃ大事でこれ書かないと10倍くらい重くなる(for文中で使うから参照渡しだとやばい)

    //平面の方程式はn・r=h(nは法線,rは位置ベクトル,hはconst(=_planeValue)) 
    static Vector3 _planeNormal; //nに当たるもの
    static float _planeValue;    //rに当たるもの


    static bool[] _isFront;//頂点が切断面に対して表にあるか裏にあるか
    static int[] _trackedArray;//切断後のMeshでの切断前の頂点の番号

    static bool _makeCutSurface;

    static Dictionary<int, (int, int)> newVertexDic = new Dictionary<int, (int, int)>(101);


    static FragmentList fragmentList = new FragmentList();
    static RoopFragmentCollection roopCollection = new RoopFragmentCollection();


    static List<Vector3> _frontVertices = new List<Vector3>();
    static List<Vector3> _backVertices = new List<Vector3>();
    static List<Vector3> _frontNormals = new List<Vector3>();
    static List<Vector3> _backNormals = new List<Vector3>();
    static List<Vector2> _frontUVs = new List<Vector2>();
    static List<Vector2> _backUVs = new List<Vector2>();

    static List<List<int>> _frontSubmeshIndices = new List<List<int>>();
    static List<List<int>> _backSubmeshIndices = new List<List<int>>();

    /// <summary>
    /// <para>gameObjectを切断して2つのMeshにして返します.1つ目のMeshが切断面の法線に対して表側, 2つ目が裏側です.</para>
    /// <para>何度も切るようなオブジェクトでも頂点数が増えないように処理をしてあるほか, 簡単な物体なら切断面を縫い合わせることもできます</para>
    /// </summary>
    /// <param name="targetMesh">切断するMesh</param>
    /// <param name="targetTransform">切断するMeshのTransform</param>
    /// <param name="planeAnchorPoint">切断面上のワールド空間上での1点</param>
    /// <param name="planeNormalDirection">切断面のワールド空間上での法線</param>
    /// <param name="makeCutSurface">切断後にMeshを縫い合わせるか否か</param>
    /// <param name="addNewMeshIndices">新しいsubMeshを作るか(切断面に新しいマテリアルを割り当てる場合にはtrue, すでに切断面のマテリアルがRendererについてる場合はfalse)</param>
    /// <returns></returns>
    public static (Mesh frontside, Mesh backside) CutMesh(Mesh targetMesh, Transform targetTransform, Vector3 planeAnchorPoint, Vector3 planeNormalDirection, bool makeCutSurface = true, bool addNewMeshIndices = false) {
        if (planeNormalDirection == Vector3.zero) {
            Debug.LogError("the normal vector magnitude is zero!");

            Mesh empty = new Mesh();
            empty.vertices = new Vector3[] { };
            return (null, null);
        }

        //初期化
        {
            _targetMesh = targetMesh; //Mesh情報取得
            //for文で_targetMeshから呼ぶのは非常に重くなるのでここで配列に格納してfor文ではここから渡す(Mesh.verticesなどは参照ではなくて毎回コピーしたものを返してるっぽい)
            _targetVertices = _targetMesh.vertices;
            _targetNormals = _targetMesh.normals;
            _targetUVs = _targetMesh.uv;


            int verticesLength = _targetVertices.Length;
            _makeCutSurface = makeCutSurface;

            _trackedArray = new int[verticesLength];
            _isFront = new bool[verticesLength];
            newVertexDic.Clear();
            roopCollection.Clear();
            fragmentList.Clear();

            _frontVertices.Clear();
            _frontNormals.Clear();
            _frontUVs.Clear();
            _frontSubmeshIndices.Clear();

            _backVertices.Clear();
            _backNormals.Clear();
            _backUVs.Clear();
            _backSubmeshIndices.Clear();

            Vector3 scale = targetTransform.localScale;//localscaleに合わせてPlaneに入れるnormalに補正をかける
            _planeNormal = Vector3.Scale(scale, targetTransform.InverseTransformDirection(planeNormalDirection)).normalized;
        }



        //最初に頂点の情報だけを入力していく

        Vector3 anchor = targetTransform.transform.InverseTransformPoint(planeAnchorPoint);
        _planeValue = Vector3.Dot(_planeNormal, anchor);
        {
            float pnx = _planeNormal.x;
            float pny = _planeNormal.y;
            float pnz = _planeNormal.z;

            float ancx = anchor.x;
            float ancy = anchor.y;
            float ancz = anchor.z;

            int frontCount = 0;
            int backCount = 0;
            for (int i = 0; i < _targetVertices.Length; i++) {
                Vector3 pos = _targetVertices[i];
                //planeの表側にあるか裏側にあるかを判定.(たぶん表だったらtrue)
                if (_isFront[i] = (pnx * (pos.x - ancx) + pny * (pos.y - ancy) + pnz * (pos.z - ancz)) > 0) {
                    //頂点情報を追加していく
                    _frontVertices.Add(pos);
                    _frontNormals.Add(_targetNormals[i]);
                    _frontUVs.Add(_targetUVs[i]);
                    //もとのMeshのn番目の頂点が新しいMeshで何番目になるのかを記録(面を貼るときに使う)
                    _trackedArray[i] = frontCount++;
                } else {
                    _backVertices.Add(pos);
                    _backNormals.Add(_targetNormals[i]);
                    _backUVs.Add(_targetUVs[i]);

                    _trackedArray[i] = backCount++;
                }
            }



            if (frontCount == 0 || backCount == 0)//片側に全部寄った場合はここで終了
            {
                return (null, null);
            }
        }







        //次に, 三角ポリゴンの情報を追加していく
        int submeshCount = _targetMesh.subMeshCount;

        for (int sub = 0; sub < submeshCount; sub++) {

            int[] indices = _targetMesh.GetIndices(sub);


            //ポリゴンを形成する頂点の番号を入れるintの配列を作っている.(submeshごとに追加)
            List<int> frontIndices = new List<int>();
            _frontSubmeshIndices.Add(frontIndices);
            List<int> backIndices = new List<int>();
            _backSubmeshIndices.Add(backIndices);




            //ポリゴンの情報は頂点3つで1セットなので3つ飛ばしでループ
            for (int i = 0; i < indices.Length; i += 3) {
                int p1, p2, p3;
                p1 = indices[i];
                p2 = indices[i + 1];
                p3 = indices[i + 2];


                //予め計算しておいた結果を持ってくる(ここで計算すると同じ頂点にたいして何回も同じ計算をすることになるから最初にまとめてやっている(そのほうが処理時間が速かった))
                bool side1 = _isFront[p1];
                bool side2 = _isFront[p2];
                bool side3 = _isFront[p3];



                if (side1 && side2 && side3)//3つとも表側, 3つとも裏側のときはそのまま出力
                {
                    //indicesは切断前のMeshの頂点番号が入っているので_trackedArrayを通すことで切断後のMeshでの番号に変えている
                    frontIndices.Add(_trackedArray[p1]);
                    frontIndices.Add(_trackedArray[p2]);
                    frontIndices.Add(_trackedArray[p3]);
                } else if (!side1 && !side2 && !side3) {
                    backIndices.Add(_trackedArray[p1]);
                    backIndices.Add(_trackedArray[p2]);
                    backIndices.Add(_trackedArray[p3]);
                } else  //三角ポリゴンを形成する各点で面に対する表裏が異なる場合, つまり切断面と重なっている平面は分割する.
                  {
                    Sepalate(new bool[3] { side1, side2, side3 }, new int[3] { p1, p2, p3 }, sub);
                }

            }

        }





        fragmentList.MakeTriangle();//切断されたポリゴンはここでそれぞれのMeshに追加される

        if (makeCutSurface) {
            if (addNewMeshIndices) {
                _frontSubmeshIndices.Add(new List<int>());//submeshが増えるのでリスト追加
                _backSubmeshIndices.Add(new List<int>());
            }
            roopCollection.MakeCutSurface(_frontSubmeshIndices.Count - 1, targetTransform);
        }

        //2つのMeshを新規に作ってそれぞれに情報を追加して出力
        Mesh frontMesh = new Mesh();
        frontMesh.name = "Split Mesh front";


        frontMesh.vertices = _frontVertices.ToArray();
        frontMesh.normals = _frontNormals.ToArray();
        frontMesh.uv = _frontUVs.ToArray();



        frontMesh.subMeshCount = _frontSubmeshIndices.Count;
        for (int i = 0; i < _frontSubmeshIndices.Count; i++) {
            frontMesh.SetIndices(_frontSubmeshIndices[i].ToArray(), MeshTopology.Triangles, i, false);
        }


        Mesh backMesh = new Mesh();
        backMesh.name = "Split Mesh back";
        backMesh.vertices = _backVertices.ToArray();
        backMesh.normals = _backNormals.ToArray();
        backMesh.uv = _backUVs.ToArray();

        backMesh.subMeshCount = _backSubmeshIndices.Count;
        for (int i = 0; i < _backSubmeshIndices.Count; i++) {
            backMesh.SetIndices(_backSubmeshIndices[i].ToArray(), MeshTopology.Triangles, i, false);
        }



        return (frontMesh, backMesh);
    }

    /// <summary>
    /// Meshを切断します. 
    /// 1つ目のGameObjectが法線の向いている方向で新しくInstantiateしたもの, 1つ目のGameObjectが法線と反対方向で入力したものを返します
    /// </summary>
    /// <param name="targetGameObject">切断されるGameObject</param>
    /// <param name="planeAnchorPoint">切断平面上のどこか1点(ワールド座標)</param>
    /// <param name="planeNormalDirection">切断平面の法線(ワールド座標)</param>
    /// <param name="makeCutSurface">切断面を作るかどうか</param>
    /// <param name="cutSurfaceMaterial">切断面に割り当てるマテリアル(nullの場合は適当なマテリアルを割り当てる)</param>
    /// <returns></returns>
    public static (GameObject copy_normalside, GameObject original_anitiNormalside) CutMesh(GameObject targetGameObject, Vector3 planeAnchorPoint, Vector3 planeNormalDirection, bool makeCutSurface = true, Material cutSurfaceMaterial = null) {
        if (!targetGameObject.GetComponent<MeshFilter>()) {
            Debug.LogError("引数のオブジェクトにはMeshFilterをアタッチしろ!");
            return (null, null);
        } else if (!targetGameObject.GetComponent<MeshRenderer>()) {
            Debug.LogError("引数のオブジェクトにはMeshrendererをアタッチしろ!");
            return (null, null);
        }

        Mesh mesh = targetGameObject.GetComponent<MeshFilter>().mesh;
        Transform transform = targetGameObject.transform;
        bool addNewMaterial;

        MeshRenderer renderer = targetGameObject.GetComponent<MeshRenderer>();
        //materialにアクセスするとその瞬間にmaterialの個別のインスタンスが作られてマテリアル名に(instance)がついてしまうので読み込みはsharedMaterialで行う
        Material[] mats = renderer.sharedMaterials;
        if (makeCutSurface && cutSurfaceMaterial != null) {
            if (mats[mats.Length - 1]?.name == cutSurfaceMaterial.name)//すでに切断マテリアルが追加されているときはそれを使うので追加しない
            {
                addNewMaterial = false;
            } else {
                addNewMaterial = true;
            }
        } else {
            addNewMaterial = false;
        }

        (Mesh fragMesh, Mesh originMesh) = CutMesh(mesh, transform, planeAnchorPoint, planeNormalDirection, makeCutSurface, addNewMaterial);


        if (originMesh == null || fragMesh == null) {
            return (null, null);

        }
        if (addNewMaterial) {
            int matLength = mats.Length;
            Material[] newMats = new Material[matLength + 1];
            mats.CopyTo(newMats, 0);
            newMats[matLength] = cutSurfaceMaterial;


            renderer.sharedMaterials = newMats;
        }


        targetGameObject.GetComponent<MeshFilter>().mesh = originMesh;

        //GameObject fragment = new GameObject("Fragment", typeof(MeshFilter), typeof(MeshRenderer));
        Transform originTransform = targetGameObject.transform;
        GameObject fragment = Instantiate(targetGameObject, originTransform.position, originTransform.rotation, originTransform.parent);
        fragment.transform.parent = null;
        fragment.GetComponent<MeshFilter>().mesh = fragMesh;
        fragment.GetComponent<MeshRenderer>().sharedMaterials = targetGameObject.GetComponent<MeshRenderer>().sharedMaterials;

        if (targetGameObject.GetComponent<MeshCollider>()) {
            //頂点が1点に重なっている場合にはエラーが出るので, 直したい場合はmesh.RecalculateBoundsのあとでmesh.bounds.size.magnitude<0.00001などで条件分けして対処してください
            targetGameObject.GetComponent<MeshCollider>().sharedMesh = originMesh;
            fragment.GetComponent<MeshCollider>().sharedMesh = fragMesh;
        }



        return (fragment, targetGameObject);

    }



    //ポリゴンを切断
    //ポリゴンは切断面の表側と裏側に分割される.
    //このとき三角ポリゴンを表面から見て, なおかつ切断面の表側にある頂点が下に来るように見て,
    //三角形の左側の辺を形成する点をf0,b0, 右側にある辺を作る点をf1,b1とする.(fは表側にある点でbは裏側)(頂点は3つなので被りが存在する)
    //ここでポリゴンの向きを決めておくと後々とても便利
    //以降左側にあるものは0,右側にあるものは1をつけて扱う(例外はあるかも)
    //(ひょっとすると実際の向きは逆かもしれないけどvertexIndicesと同じまわり方で出力してるので逆でも問題はない.ここでは3点が時計回りで並んでいると仮定して全部の)
    private static void Sepalate(bool[] sides, int[] vertexIndices, int submesh) {
        int f0 = 0, f1 = 0, b0 = 0, b1 = 0; //頂点のindex番号を格納するのに使用
        bool twoPointsInFrontSide;//どちらがに頂点が2つあるか

        //ポリゴンの向きを揃える
        if (sides[0]) {
            if (sides[1]) {
                f0 = vertexIndices[1];
                f1 = vertexIndices[0];
                b0 = b1 = vertexIndices[2];
                twoPointsInFrontSide = true;
            } else {
                if (sides[2]) {
                    f0 = vertexIndices[0];
                    f1 = vertexIndices[2];
                    b0 = b1 = vertexIndices[1];
                    twoPointsInFrontSide = true;
                } else {
                    f0 = f1 = vertexIndices[0];
                    b0 = vertexIndices[1];
                    b1 = vertexIndices[2];
                    twoPointsInFrontSide = false;
                }
            }
        } else {
            if (sides[1]) {
                if (sides[2]) {
                    f0 = vertexIndices[2];
                    f1 = vertexIndices[1];
                    b0 = b1 = vertexIndices[0];
                    twoPointsInFrontSide = true;
                } else {
                    f0 = f1 = vertexIndices[1];
                    b0 = vertexIndices[2];
                    b1 = vertexIndices[0];
                    twoPointsInFrontSide = false;
                }
            } else {
                f0 = f1 = vertexIndices[2];
                b0 = vertexIndices[0];
                b1 = vertexIndices[1];
                twoPointsInFrontSide = false;
            }
        }

        //切断前のポリゴンの頂点の座標を取得(そのうち2つはかぶってる)
        Vector3 frontPoint0, frontPoint1, backPoint0, backPoint1;
        if (twoPointsInFrontSide) {
            frontPoint0 = _targetVertices[f0];
            frontPoint1 = _targetVertices[f1];
            backPoint0 = backPoint1 = _targetVertices[b0];
        } else {
            frontPoint0 = frontPoint1 = _targetVertices[f0];
            backPoint0 = _targetVertices[b0];
            backPoint1 = _targetVertices[b1];
        }

        //ベクトル[backPoint0 - frontPoint0]を何倍したら切断平面に到達するかは以下の式で表される
        //平面の式: dot(r,n)=A ,Aは定数,nは法線, 
        //今回    r =frontPoint0+k*(backPoint0 - frontPoint0), (0 ≦ k ≦ 1)
        //これは, 新しくできる頂点が2つの頂点を何対何に内分してできるのかを意味している
        float dividingParameter0 = (_planeValue - Vector3.Dot(_planeNormal, frontPoint0)) / (Vector3.Dot(_planeNormal, backPoint0 - frontPoint0));
        //Lerpで切断によってうまれる新しい頂点の座標を生成
        Vector3 newVertexPos0 = Vector3.Lerp(frontPoint0, backPoint0, dividingParameter0);


        float dividingParameter1 = (_planeValue - Vector3.Dot(_planeNormal, frontPoint1)) / (Vector3.Dot(_planeNormal, backPoint1 - frontPoint1));
        Vector3 newVertexPos1 = Vector3.Lerp(frontPoint1, backPoint1, dividingParameter1);

        //新しい頂点の生成, ここではNormalとUVは計算せず後から計算できるように頂点のindex(_trackedArray[f0], _trackedArray[b0],)と内分点の情報(dividingParameter0)を持っておく
        NewVertex vertex0 = new NewVertex(_trackedArray[f0], _trackedArray[b0], dividingParameter0, newVertexPos0);
        NewVertex vertex1 = new NewVertex(_trackedArray[f1], _trackedArray[b1], dividingParameter1, newVertexPos1);


        //切断でできる辺(これが同じポリゴンは後で結合して頂点数の増加を抑える)
        Vector3 cutLine = (newVertexPos1 - newVertexPos0).normalized;
        int KEY_CUTLINE = MakeIntFromVector3_ErrorCut(cutLine);//Vector3だと処理が重そうなのでintにしておく, ついでに丸め誤差を切り落とす

        //切断情報を含んだFragmentクラス
        Fragment fragment = new Fragment(vertex0, vertex1, twoPointsInFrontSide, KEY_CUTLINE, submesh);
        //Listに追加してListの中で同一平面のFragmentは結合とかする
        fragmentList.Add(fragment, KEY_CUTLINE);

    }

    class RoopFragment {
        public RoopFragment next; //右隣のやつ
        public Vector3 rightPosition;//右側の座標(左側の座標は左隣のやつがもってる)
        public RoopFragment(Vector3 _rightPosition) {
            next = null;
            rightPosition = _rightPosition;
        }
    }
    class RooP {
        public RoopFragment start, end; //startが左端, endが右端
        public Vector3 startPos, endPos;//左端,右端のPosition 
        public int verticesCount;//含まれる頂点数(ループが閉じるまでは頂点数-1の値になる)
        public Vector3 sum_rightPosition; //Positionの和.これをcountで割ると図形の中点が得られる.
        public RooP(RoopFragment _left, RoopFragment _right, Vector3 _startPos, Vector3 _endPos, Vector3 rightPos) {
            start = _left;
            end = _right;
            startPos = _startPos;
            endPos = _endPos;
            verticesCount = 1;
            sum_rightPosition = rightPos;
        }
    }

    public class RoopFragmentCollection {


        Dictionary<Vector3, RooP> leftPointDic = new Dictionary<Vector3, RooP>();
        Dictionary<Vector3, RooP> rightPointDic = new Dictionary<Vector3, RooP>();

        /// <summary>
        /// 切断辺を追加していって隣り合う辺がすでにあればくっつける.最終的にループを作る
        /// </summary>
        /// <param name="leftVertexPos">切断辺の左の点のPosition</param>
        /// <param name="rightVertexPos">切断辺の右の点のPosition</param>
        public void Add(Vector3 leftVertexPos, Vector3 rightVertexPos) {

            RoopFragment target = new RoopFragment(rightVertexPos);//新しいループ辺を作る. 


            RooP roop1 = null;
            bool find1;
            if (find1 = rightPointDic.ContainsKey(leftVertexPos)) //自分の左手とくっつくのは相手の右手なので右手disctionaryの中に既にleftVertexPosのkeyが入っていないかチェック
            {
                roop1 = rightPointDic[leftVertexPos];

                roop1.end.next = target;//roopの右端(終端)の右(next)にtargetをくっつける
                roop1.end = target;//roopの右端をtargetに変える(roopは左端と右端の情報だけを持っている)
                roop1.endPos = rightVertexPos;

                rightPointDic.Remove(leftVertexPos);//roop1をリストから外す(あとで右手Listの自分の右手indexの場所に移すため)

            }


            RooP roop2 = null;
            bool find2;

            if (find2 = leftPointDic.ContainsKey(rightVertexPos)) {
                roop2 = leftPointDic[rightVertexPos];
                if (roop1 == roop2) {
                    roop1.verticesCount++;
                    roop1.sum_rightPosition += rightVertexPos;
                    return;
                }//roop1==roop2のとき, roopが完成したのでreturn

                target.next = roop2.start;//targetの右にroop2の左端(始端)をくっつける
                roop2.start = target;//roop2の左端をtargetに変更
                roop2.startPos = leftVertexPos;

                leftPointDic.Remove(rightVertexPos);
            }


            if (find1) {
                if (find2)//2つのroopがくっついたとき 
                {
                    //roop1+target+roop2の順でつながっているはずなのでroop1にroop2の情報を追加する
                    roop1.end = roop2.end;//終端を変更
                    roop1.endPos = roop2.endPos;
                    roop1.verticesCount += roop2.verticesCount + 1;//+1はtargetの分
                    roop1.sum_rightPosition += roop2.sum_rightPosition + rightVertexPos;
                    var key = roop2.endPos;
                    rightPointDic[key] = roop1;//dictionaryに入れておかないと最後に面を貼れないので適当に突っ込んでおく.



                } else//自分の左手とroopの右手がくっついたとき, 右手dictionaryの自分の右手Positionの場所にroopをついか
                  {
                    roop1.verticesCount++;
                    roop1.sum_rightPosition += rightVertexPos;
                    if (leftPointDic.ContainsKey(leftVertexPos)) {
                        return;
                    } //既にdictionaryに追加されている場合はreturn (変なポリゴン構成になっているとそうなる)
                    rightPointDic.Add(rightVertexPos, roop1);
                }
            } else {
                if (find2) {
                    roop2.verticesCount++;
                    roop2.sum_rightPosition += rightVertexPos;
                    if (leftPointDic.ContainsKey(leftVertexPos)) {
                        return;
                    } //既にdictionaryに追加されている場合はreturn (変なポリゴン構成になっているとそうなる)
                    leftPointDic.Add(leftVertexPos, roop2);
                } else//どこにもくっつかなかったとき, roopを作成, 追加
                  {
                    RooP newRoop = new RooP(target, target, leftVertexPos, rightVertexPos, rightVertexPos);
                    rightPointDic.Add(rightVertexPos, newRoop);
                    leftPointDic.Add(leftVertexPos, newRoop);
                }
            }
        }


        public void MakeCutSurface(int submesh, Transform targetTransform) {
            Vector3 scale = targetTransform.localScale;
            Vector3 world_Up = Vector3.Scale(scale, targetTransform.InverseTransformDirection(Vector3.up)).normalized;//ワールド座標の上方向をオブジェクト座標に変換
            Vector3 world_Right = Vector3.Scale(scale, targetTransform.InverseTransformDirection(Vector3.right)).normalized;//ワールド座標の右方向をオブジェクト座標に変換


            Vector3 uVector, vVector; //オブジェクト空間上でのUVのU軸,V軸
            uVector = Vector3.Cross(world_Up, _planeNormal); //U軸は切断面の法線とY軸との外積
            uVector = (uVector.sqrMagnitude != 0) ? uVector.normalized : world_Right; //切断面の法線がZ軸方向のときはuVectorがゼロベクトルになるので場合分け

            vVector = Vector3.Cross(_planeNormal, uVector).normalized; //V軸はU軸と切断平面のノーマルとの外積
            if (Vector3.Dot(vVector, world_Up) < 0) {
                vVector *= -1;
            } //v軸の方向をワールド座標上方向に揃える.

            float u_min, u_max, u_range;
            float v_min, v_max, v_range;



            foreach (RooP roop in leftPointDic.Values) {
                {


                    u_min = u_max = Vector3.Dot(uVector, roop.startPos);
                    v_min = v_max = Vector3.Dot(vVector, roop.startPos);
                    RoopFragment fragment = roop.start;


                    int count = 0;
                    do {
                        float u_value = Vector3.Dot(uVector, fragment.rightPosition);
                        u_min = Mathf.Min(u_min, u_value);
                        u_max = Mathf.Max(u_max, u_value);

                        float v_value = Vector3.Dot(vVector, fragment.rightPosition);
                        v_min = Mathf.Min(v_min, v_value);
                        v_max = Mathf.Max(v_max, v_value);


                        if (count > 1000) //何かあったときのための安全装置(while文こわい)
                        {
                            Debug.LogError("Something is wrong?");
                            break;
                        }
                        count++;

                    }
                    while ((fragment = fragment.next) != null);

                    u_range = u_max - u_min;
                    v_range = v_max - v_min;

                }




                //roopFragmentのnextをたどっていくことでroopを一周できる

                MakeVertex(roop.sum_rightPosition / roop.verticesCount, out int center_f, out int center_b);//切断面の中心に頂点を追加して頂点番号を返す

                RoopFragment nowFragment = roop.start;

                MakeVertex(nowFragment.rightPosition, out int first_f, out int first_b);//ループの始端の頂点を追加して頂点番号を返す
                int previous_f = first_f;
                int previous_b = first_b;


                while (nowFragment.next != null)//終端に達するまでループ
                {
                    nowFragment = nowFragment.next;


                    MakeVertex(nowFragment.rightPosition, out int index_f, out int index_b);//新しく頂点を追加して頂点番号を返す

                    _frontSubmeshIndices[submesh].Add(center_f);
                    _frontSubmeshIndices[submesh].Add(index_f);
                    _frontSubmeshIndices[submesh].Add(previous_f);

                    _backSubmeshIndices[submesh].Add(center_b);
                    _backSubmeshIndices[submesh].Add(previous_b);
                    _backSubmeshIndices[submesh].Add(index_b);

                    previous_f = index_f;
                    previous_b = index_b;



                }
                _frontSubmeshIndices[submesh].Add(center_f);
                _frontSubmeshIndices[submesh].Add(first_f);
                _frontSubmeshIndices[submesh].Add(previous_f);

                _backSubmeshIndices[submesh].Add(center_b);
                _backSubmeshIndices[submesh].Add(previous_b);
                _backSubmeshIndices[submesh].Add(first_b);

            }

            void MakeVertex(Vector3 vertexPos, out int findex, out int bindex) {
                findex = _frontVertices.Count;
                bindex = _backVertices.Count;
                Vector2 uv;
                { //positionをUVに変換
                    float uValue = Vector3.Dot(uVector, vertexPos);
                    float normalizedU = (uValue - u_min) / u_range;
                    float vValue = Vector3.Dot(vVector, vertexPos);
                    float normalizedV = (vValue - v_min) / v_range;

                    uv = new Vector2(normalizedU, normalizedV);

                }
                _frontVertices.Add(vertexPos);
                _frontNormals.Add(-_planeNormal);
                _frontUVs.Add(uv);

                _backVertices.Add(vertexPos);
                _backNormals.Add(_planeNormal);
                _backUVs.Add(new Vector2(1 - uv.x, uv.y));//UVを左右反転する

            }
        }






        public void Clear() {
            leftPointDic.Clear();
            rightPointDic.Clear();
        }
    }

    public class Fragment {
        public NewVertex vertex0, vertex1;
        public int KEY_CUTLINE;
        public int submesh;//submesh番号(どのマテリアルを当てるか)
        public Point firstPoint_f, lastPoint_f, firstPoint_b, lastPoint_b; //ポリゴンの4つ(3つ)の頂点の情報
        public int count_f, count_b;//front側,back側の頂点数

        public Fragment(NewVertex _vertex0, NewVertex _vertex1, bool _twoPointsInFrontSide, int _KEY_CUTLINE, int _submesh) {
            vertex0 = _vertex0;
            vertex1 = _vertex1;
            KEY_CUTLINE = _KEY_CUTLINE;
            submesh = _submesh;



            if (_twoPointsInFrontSide) {
                firstPoint_f = new Point(_vertex0.frontsideindex_of_frontMesh);
                lastPoint_f = new Point(_vertex1.frontsideindex_of_frontMesh);
                firstPoint_f.next = lastPoint_f;
                firstPoint_b = new Point(vertex0.backsideindex_of_backMash);
                lastPoint_b = firstPoint_b;
                count_f = 2;
                count_b = 1;
            } else {
                firstPoint_f = new Point(_vertex0.frontsideindex_of_frontMesh);
                lastPoint_f = firstPoint_f;
                firstPoint_b = new Point(vertex0.backsideindex_of_backMash);
                lastPoint_b = new Point(vertex1.backsideindex_of_backMash);
                firstPoint_b.next = lastPoint_b;
                count_f = 1;
                count_b = 2;
            }
        }

        public void AddTriangle() {
            (int findex0, int bindex0) = vertex0.GetIndex(); //Vertexの中で新しく生成された頂点を登録してその番号だけを返している
            (int findex1, int bindex1) = vertex1.GetIndex();

            Point point = firstPoint_f;
            int preIndex = point.index;

            int count = count_f;
            int halfcount = count_f / 2;
            for (int i = 0; i < halfcount; i++) {
                point = point.next;
                int index = point.index;
                _frontSubmeshIndices[submesh].Add(index);
                _frontSubmeshIndices[submesh].Add(preIndex);
                _frontSubmeshIndices[submesh].Add(findex0);
                preIndex = index;
            }
            _frontSubmeshIndices[submesh].Add(preIndex);
            _frontSubmeshIndices[submesh].Add(findex0);
            _frontSubmeshIndices[submesh].Add(findex1);
            int elseCount = count_f - halfcount - 1;
            for (int i = 0; i < elseCount; i++) {
                point = point.next;
                int index = point.index;
                _frontSubmeshIndices[submesh].Add(index);
                _frontSubmeshIndices[submesh].Add(preIndex);
                _frontSubmeshIndices[submesh].Add(findex1);
                preIndex = index;
            }


            point = firstPoint_b;
            preIndex = point.index;
            count = count_b;
            halfcount = count_b / 2;

            for (int i = 0; i < halfcount; i++) {
                point = point.next;
                int index = point.index;
                _backSubmeshIndices[submesh].Add(index);
                _backSubmeshIndices[submesh].Add(bindex0);
                _backSubmeshIndices[submesh].Add(preIndex);
                preIndex = index;
            }
            _backSubmeshIndices[submesh].Add(preIndex);
            _backSubmeshIndices[submesh].Add(bindex1);
            _backSubmeshIndices[submesh].Add(bindex0);
            elseCount = count_b - halfcount - 1;
            for (int i = 0; i < elseCount; i++) {
                point = point.next;
                int index = point.index;
                _backSubmeshIndices[submesh].Add(index);
                _backSubmeshIndices[submesh].Add(bindex1);
                _backSubmeshIndices[submesh].Add(preIndex);
                preIndex = index;
            }

            if (_makeCutSurface) {
                roopCollection.Add(vertex0.position, vertex1.position);//切断平面を形成する準備
            }

        }
    }

    //新しい頂点のNormalとUVは最後に生成するので, もともとある頂点をどの比で混ぜるかをdividingParameterが持っている
    public class NewVertex {
        public int frontsideindex_of_frontMesh; //frontVertices,frontNormals,frontUVsでの頂点の番号(frontsideindex_of_frontMeshとbacksideindex_of_backMashでできる辺の間に新しい頂点ができる)
        public int backsideindex_of_backMash;
        public float dividingParameter;//新しい頂点の(frontsideindex_of_frontMeshとbacksideindex_of_backMashでできる辺に対する)内分点
        public int KEY_VERTEX;
        public Vector3 position;

        public NewVertex(int front, int back, float parameter, Vector3 vertexPosition) {
            frontsideindex_of_frontMesh = front;
            backsideindex_of_backMash = back;
            KEY_VERTEX = (front << 16) | back;
            dividingParameter = parameter;
            position = vertexPosition;
        }

        public (int findex, int bindex) GetIndex() {
            //法線とUVの情報はここで生成する
            Vector3 frontNormal, backNormal;
            Vector2 frontUV, backUV;

            frontNormal = _frontNormals[frontsideindex_of_frontMesh];
            frontUV = _frontUVs[frontsideindex_of_frontMesh];

            backNormal = _backNormals[backsideindex_of_backMash];
            backUV = _backUVs[backsideindex_of_backMash];



            Vector3 newNormal = Vector3.Lerp(frontNormal, backNormal, dividingParameter);
            Vector2 newUV = Vector2.Lerp(frontUV, backUV, dividingParameter);

            int findex, bindex;
            (int, int) trackNumPair;
            //同じ2つの点の間に生成される頂点は1つにまとめたいのでDictionaryを使う
            if (newVertexDic.TryGetValue(KEY_VERTEX, out trackNumPair)) {
                findex = trackNumPair.Item1;//新しい頂点が表側のMeshで何番目か
                bindex = trackNumPair.Item2;
            } else {

                findex = _frontVertices.Count;
                _frontVertices.Add(position);
                _frontNormals.Add(newNormal);
                _frontUVs.Add(newUV);

                bindex = _backVertices.Count;
                _backVertices.Add(position);
                _backNormals.Add(newNormal);
                _backUVs.Add(newUV);

                newVertexDic.Add(KEY_VERTEX, (findex, bindex));

            }

            return (findex, bindex);
        }
    }

    public class Point {
        public Point next;
        public int index;
        public Point(int _index) {
            index = _index;
            next = null;
        }
    }


    public class FragmentList {

        Dictionary<int, List<Fragment>> cutLineDictionary = new Dictionary<int, List<Fragment>>();//同じ切断辺をもったFragmentをリストにまとめる

        /// <summary>
        /// 切断片を追加して隣り合う切断片はくっつけてしまう
        /// </summary>
        /// <param name="fragment">追加する切断片</param>
        /// <param name="KEY_CUTLINE">切断された辺の向きをintに変換したもの</param>
        public void Add(Fragment fragment, int KEY_CUTLINE) {

            List<Fragment> flist;
            if (!cutLineDictionary.TryGetValue(KEY_CUTLINE, out flist)) //各切断辺の1つめのポリゴンについては新しくkey-valueを追加
            {
                flist = new List<Fragment>();
                cutLineDictionary.Add(KEY_CUTLINE, flist);
            }


            bool connect = false;
            //格納されているFragmentからくっつけられるやつを探す
            for (int i = flist.Count - 1; i >= 0; i--) {
                Fragment compareFragment = flist[i];
                if (fragment.KEY_CUTLINE == compareFragment.KEY_CUTLINE)//同じ切断辺をもつか判断
                {
                    Fragment left, right;
                    if (fragment.vertex0.KEY_VERTEX == compareFragment.vertex1.KEY_VERTEX)//fragmentがcompareFragmentに右側からくっつく場合
                    {
                        right = fragment;
                        left = compareFragment;
                    } else if (fragment.vertex1.KEY_VERTEX == compareFragment.vertex0.KEY_VERTEX)//左側からくっつく場合
                      {
                        left = fragment;
                        right = compareFragment;
                    } else {
                        continue;//どっちでもないときは次のループへ
                    }

                    //Pointクラスのつなぎ合わせ. 
                    //firstPoint.nextがnullということは頂点を1つしか持っていない. 
                    //またその頂点はleftのlastPointとかぶっているので頂点が増えることはない
                    //(left.lastPoint_fとright.lastPoint_fは同じ点を示すが別のインスタンスなのでnextがnullのときに入れ替えるとループが途切れてしまう)
                    if ((left.lastPoint_f.next = right.firstPoint_f.next) != null) {
                        left.lastPoint_f = right.lastPoint_f;
                        left.count_f += right.count_f - 1;
                    }
                    if ((left.lastPoint_b.next = right.firstPoint_b.next) != null) {
                        left.lastPoint_b = right.lastPoint_b;
                        left.count_b += right.count_b - 1;
                    }


                    //結合を行う
                    //Fragmentがより広くなるように頂点情報を変える
                    left.vertex1 = right.vertex1;
                    right.vertex0 = left.vertex0;

                    //connectがtrueになっているということは2つのFragmentのあいだに新しいやつがはまって3つが1つになったということ
                    //connect==trueのとき, rightもleftもListにすでに登録されてるやつなのでどっちかを消してやる
                    if (connect) {
                        flist.Remove(right);

                        break;
                    }

                    flist[i] = left;
                    fragment = left;
                    connect = true;
                }
            }

            if (!connect) {
                flist.Add(fragment);
            }
        }


        public void MakeTriangle() {
            int sum = 0;
            foreach (List<Fragment> list in cutLineDictionary.Values) {
                foreach (Fragment f in list) {
                    f.AddTriangle();
                    sum++;
                }
            }
        }

        public void Clear() {
            cutLineDictionary.Clear();
        }


    }




    const int filter = 0x000003FF;

    const int amp = 1 << 10;//丸め誤差を落とすためにやや低めの倍率がかかっている
    public static int MakeIntFromVector3_ErrorCut(Vector3 vec)//Vector3から丸め誤差を落としてintに変換
    {
        int cutLineX = ((int)(vec.x * amp) & filter) << 20;
        int cutLineY = ((int)(vec.y * amp) & filter) << 10;
        int cutLineZ = ((int)(vec.z * amp) & filter);

        return cutLineX | cutLineY | cutLineZ;
    }
}
