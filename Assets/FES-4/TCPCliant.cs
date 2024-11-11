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


public class TCPCliant : MonoBehaviour
{
    [SerializeField] private int port = 50000;
    [SerializeField] private string ip = "192.168.20.14";
    [SerializeField] private byte[] request_bytes = { 0x01 };

    public event Action<Message> receiveAction;
    private Queue<Message> _queue = new Queue<Message>();
    private Message _message;

    private object _lockObject = new object();
    public void AddReceiveEvent(Action<Message> action)
    {
        receiveAction += action;
    }

#if UNITY_EDITOR
    private TcpClient _tcpClient = null;
    private NetworkStream _stream = null;

    private void Start()
    {
        try
        {
            Debug.Log($"TcpClient Request port:{port}");
            // TcpClient���쐬���A�T�[�o�[�Ɛڑ�����
            // �ڑ���������܂Ńu���b�L���O����
            _tcpClient = new TcpClient(ip, port);
            // NetworkStream���擾����
            _stream = _tcpClient.GetStream();

            // �ǂݎ��A�������݂̃^�C���A�E�g��10�b�ɂ���
            //�f�t�H���g��Infinite�ŁA�^�C���A�E�g���Ȃ�
            //(.NET Framework 2.0�ȏオ�K�v)
            _stream.ReadTimeout = 10000;
            _stream.WriteTimeout = 10000;

            Thread sendthread = new Thread(() =>
            {
                MessageReceivedTask();
            });
            sendthread.Start();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void Update()
    {
        SendMessage(request_bytes);

        while (_queue.Count > 0)
        {
            lock (_lockObject)
            {
                _message = _queue.Dequeue();
                receiveAction.Invoke(_message);
            }
        }

    }

    public void SendMessage(byte[] bytes)
    {
        if (_tcpClient != null && _tcpClient.Connected)
        {
            Thread sendthread = new Thread(() =>
            {
                // �f�[�^�𑗐M
                _stream.Write(bytes, 0, bytes.Length);
                _stream.Flush();
                //Debug.Log("SendMessage");
            });
            sendthread.Start();
        }
    }
    private async void MessageReceivedTask()
    {
        // �ڑ����؂��܂ő���M���J��Ԃ�
        while (_tcpClient != null && _tcpClient.Connected)
        {
            try
            {
                // �f�[�^����M
                byte[] buffer = new byte[_tcpClient.ReceiveBufferSize];
                await _stream.ReadAsync(buffer, 0, buffer.Length);
                
                lock (_lockObject)
                {
                    _queue.Enqueue(new Message(buffer, System.DateTime.Now));
                }
                //Debug.Log("MessageReceived");
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
            // �N���C�A���g�̐ڑ����؂ꂽ��
            if (_tcpClient.Client.Poll(1000, SelectMode.SelectRead) && (_tcpClient.Client.Available == 0))
            {
                Debug.Log("Disconnect: " + _tcpClient.Client.RemoteEndPoint);
                _tcpClient.Close();
                _stream = null;
            }
        }
        Debug.Log("TcpClient Close"); 
    }
#else
    private StreamSocket _socket = null;
    private Stream _inputStream = null;
    private Stream _outputStream = null;

    private const int MAX_BUFFER_SIZE = 1024;
    private byte[] _buffer = new byte[MAX_BUFFER_SIZE];
    private bool isStopMessageReceivedTask = false;
    private void Start()
    {
        Task.Run(async () =>
        {
            _socket = new StreamSocket();
            UnityEngine.WSA.Application.InvokeOnAppThread(() => { Debug.Log($"TcpClient Request port:{port}"); }, true);
            await _socket.ConnectAsync(new HostName(ip), port.ToString());
            UnityEngine.WSA.Application.InvokeOnAppThread(() => { Debug.Log($"TcpClient Connect"); }, true);
            _inputStream = _socket.InputStream.AsStreamForRead();
            _outputStream = _socket.OutputStream.AsStreamForWrite();
            MessageReceivedTask(_inputStream);
        });

    }

    private void Update()
    {
        SendMessage(request_bytes); 

        lock (_lockObject)
        {
            while (_queue.Count > 0)
            {
                _message = _queue.Dequeue();
                receiveAction.Invoke(_message);
            }
        }
    }

    public void SendMessage(byte[] bytes)
    {
        if (_outputStream != null) Task.Run(async () =>
        {
            await _outputStream.WriteAsync(bytes);
            await _outputStream.FlushAsync();
            // UnityEngine.WSA.Application.InvokeOnAppThread(() => { Debug.Log("SendMessage"); }, true);
        });
    }

    private async void MessageReceivedTask(Stream inputStream)
    {
        while (!isStopMessageReceivedTask)
        {
            if (inputStream == null) break;

            try
            {
                await inputStream.ReadAsync(_buffer, 0, MAX_BUFFER_SIZE);
                lock (_lockObject)
                {
                    _queue.Enqueue(new Message(_buffer, System.DateTime.Now));
                }
                // UnityEngine.WSA.Application.InvokeOnAppThread(() => { Debug.Log("MessageReceived"); }, true);
            }
            catch (Exception e)
            {
                UnityEngine.WSA.Application.InvokeOnAppThread(() => { Debug.Log(e.ToString()); }, true);
                break;
            }
        }
        UnityEngine.WSA.Application.InvokeOnAppThread(() => { Debug.Log("TcpClient Close"); }, true);
    }
    public void OnDestroy()
    {
        isStopMessageReceivedTask = true;
        _socket?.Dispose();
        _inputStream?.Close();
    }
#endif
}