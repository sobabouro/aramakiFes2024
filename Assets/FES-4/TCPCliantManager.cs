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


public class TCPCliantManagaer : MonoBehaviour
{

    [SerializeField, Tooltip("�|�[�g�ԍ�")] private int port = 50000;
    [SerializeField, Tooltip("IP�A�h���X")] private string ip = "192.168.20.14";
    public List<TCPCliant> tcpCcliants = new List<TCPCliant>();

    public static TCPCliantManagaer instance;
    private void Awake()
    {
        // �V���O���g���̎���
        if (instance == null)
        {
            // ���g���C���X�^���X�Ƃ���
            instance = this;
        }
        else
        {
            // �C���X�^���X���������݂��Ȃ��悤�ɁA���ɑ��݂��Ă����玩�g����������
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        tcpCcliants.Add(new TCPCliant(port, ip));
    }
}
