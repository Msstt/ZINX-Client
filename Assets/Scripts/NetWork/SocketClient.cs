using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SocketClient {
  private string host;
  private int port;
  private TcpClient tcpClient;

  private const int MAX_BUFFER_SIZE = 8192;
  private byte[] buffer = new byte[MAX_BUFFER_SIZE];

  private ByteBuffer byteBuffer = new(MAX_BUFFER_SIZE);

  public Action<int, byte[]> handleMsg;

  public void Connect(string host, int port) {
    this.host = host;
    this.port = port;

    tcpClient = new() {
      SendTimeout = 1000,
      ReceiveTimeout = 1000,
      NoDelay = true
    };
    try {
      tcpClient.BeginConnect(host, port, OnConnect, null);
    } catch (Exception e) {
      Close();
      Debug.LogError("SocketClient connect " + host + ":" + port + " fail: " + e.Message + "!");
    }
  }

  public bool Connected() {
    return tcpClient != null && tcpClient.Connected;
  }

  private void OnConnect(IAsyncResult ar) {
    if (!tcpClient.Connected) {
      Close();
      Debug.LogError("SocketClient connect " + host + ":" + port + " fail!");
      return;
    }
    Debug.Log("SocketClient connect " + host + ":" + port + " success!");
  }

  public void BeginRead() {
    if (tcpClient == null) {
      return;
    }
    lock (tcpClient.GetStream()) {
      tcpClient.GetStream().BeginRead(buffer, 0, MAX_BUFFER_SIZE, OnRead, null);
    }
  }

  private void OnRead(IAsyncResult ar) {
    if (tcpClient == null || !tcpClient.Connected) {
      return;
    }

    lock (tcpClient.GetStream()) {
      int len = tcpClient.GetStream().EndRead(ar);
      byteBuffer.Write(buffer, len);
    }

    while (byteBuffer.CanRead()) {
      byteBuffer.Read(out int msgId, out byte[] msg);
      handleMsg?.Invoke(msgId, msg);
    }

    lock (tcpClient.GetStream()) {
      tcpClient.GetStream().BeginRead(buffer, 0, MAX_BUFFER_SIZE, OnRead, null);
    }
  }

  public void Write(byte[] array) {
    if (tcpClient == null || !tcpClient.Connected) {
      return;
    }
    lock (tcpClient.GetStream()) {
      tcpClient.GetStream().BeginWrite(array, 0, array.Length, OnWrite, null);
    }
  }

  private void OnWrite(IAsyncResult ar) {
    lock (tcpClient.GetStream()) {
      try {
        tcpClient.GetStream().EndWrite(ar);
      } catch (Exception e) {
        Debug.LogError("SocketClient send message fail: " + e.Message + "!");
      }
    }
  }

  public void Close() {
    if (tcpClient != null) {
      tcpClient.Close();
      tcpClient = null;
    }
  }
}
