using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Text;

public class TCPServer
{
    private TcpListener _tcpListener = null;
    private TcpClient _tcpClient = null;
    private NetworkStream _stream = null;

    private const int MAX_BUFFER_SIZE = 1024;
    private byte[] _bytes = new byte[MAX_BUFFER_SIZE];

    public bool send_flag = false;
    public TCPServer(int port)
    {
        _tcpListener = new TcpListener(IPAddress.Any, port);
        _tcpListener.Start();
        //�R�[���o�b�N�ݒ�@�������̓R�[���o�b�N�֐��ɓn�����
        _tcpListener.BeginAcceptSocket(DoAcceptTcpClientCallback, _tcpListener);
        Debug.Log($"TCPServer Wait port:{port}");
    }

    // �N���C�A���g����̐ڑ�����
    private void DoAcceptTcpClientCallback(IAsyncResult ar)
    {
        // �n���ꂽ���̂����o��
        TcpListener tcpListener = (TcpListener)ar.AsyncState;
        _tcpClient = tcpListener.EndAcceptTcpClient(ar);
        Debug.Log("Connect: " + _tcpClient.Client.RemoteEndPoint);

        // �ڑ������l�Ƃ̃l�b�g���[�N�X�g���[�����擾
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
                //Debug.Log($"ReceivedMessage {buffer[0]}");

                // �f�[�^�𑗐M
                await _stream.WriteAsync(_bytes, 0, _bytes.Length);
                _stream.Flush();
                send_flag = true;
                //Debug.Log("SendMessage");
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
            }
        }
        Debug.Log("TcpServer Close");
    }


    public void WrightMessage(byte[] bytes)
    {
        send_flag = false;
        _bytes = bytes;
    }

    // �I������
    protected virtual void OnApplicationQuit()
    {
        _stream?.Dispose();
        _tcpListener?.Stop();
        _tcpClient?.Close();
    }
}