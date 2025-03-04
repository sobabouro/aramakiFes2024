using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Text;


#if UNITY_EDITOR
using System.Net.Sockets;
#else
using System.Threading.Tasks;
using System.IO;
using Windows.Networking;
using Windows.Networking.Sockets;
#endif


public class TCPCliantManager : MonoBehaviour
{

    [SerializeField, Tooltip("ポート番号")] private List<int> ports = new List<int>() { 50000, 50001, 50002 };
    [SerializeField, Tooltip("IPアドレス")] private string ip = "192.168.20.44";
    public List<TCPCliant> tcpCcliants = new List<TCPCliant>();

    public static TCPCliantManager instance;
    private void Awake()
    {
        // シングルトンの呪文
        if (instance == null)
        {
            // 自身をインスタンスとする
            instance = this;
        }
        else
        {
            // インスタンスが複数存在しないように、既に存在していたら自身を消去する
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (var port in ports)
        {
            tcpCcliants.Add(new TCPCliant(port, ip));
        }
    }
}
